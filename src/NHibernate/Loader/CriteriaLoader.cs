using System.Collections;
using System.Data;
using System.Text;
using Iesi.Collections;

using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.Impl;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// A <c>Loader</c> for <see cref="ICriteria"/> queries. 
	/// </summary>
	/// <remarks>
	/// Note that criteria
	/// queries are more like multi-object <c>Load()</c>s than like HQL queries.
	/// </remarks>
	internal class CriteriaLoader : AbstractEntityLoader
	{
		private CriteriaImpl criteria;
		private ISet querySpaces = new HashedSet();
		private IType[] resultTypes;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		/// <param name="criteria"></param>
		public CriteriaLoader( IOuterJoinLoadable persister, ISessionFactoryImplementor factory, CriteriaImpl criteria ) : base( persister, factory )
		{
			this.criteria = criteria;

			AddAllToPropertySpaces( persister.PropertySpaces );

			resultTypes = new IType[ 1 ];
			resultTypes[ 0 ] = NHibernateUtil.Entity( persister.MappedClass );

			StringBuilder orderByBuilder = new StringBuilder( 60 );

			bool commaNeeded = false;

			IEnumerator iter = criteria.IterateOrderings();
			while( iter.MoveNext() )
			{
				Order ord = ( Order ) iter.Current;

				if( commaNeeded )
				{
					orderByBuilder.Append( StringHelper.CommaSpace );
				}
				commaNeeded = true;

				orderByBuilder.Append( ord.ToStringForSql( factory, criteria.PersistentClass, Alias ) );
			}

			IList associations = WalkTree( persister, Alias, factory );
			InitClassPersisters( associations );
			// TODO: H2.1 SYNCH - new Hashtable() is a HACK until it is all up to H2.1 code
			InitStatementString( associations, criteria.Expression.ToSqlString( factory, criteria.PersistentClass, Alias, new Hashtable() ), orderByBuilder.ToString(), factory );

			PostInstantiate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <returns></returns>
		public IList List( ISessionImplementor session )
		{
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();

			IEnumerator iter = criteria.IterateExpressions();
			while( iter.MoveNext() )
			{
				Expression.ICriterion expr = ( Expression.ICriterion ) iter.Current;
				// TODO: h2.1 SYNCH - the new Hashtable() is a HACK
				TypedValue[ ] tv = expr.GetTypedValues(  session.Factory, criteria.PersistentClass,  new Hashtable() );
				for( int i = 0; i < tv.Length; i++ )
				{
					values.Add( tv[ i ].Value );
					types.Add( tv[ i ].Type );
				}
			}
			object[ ] valueArray = values.ToArray();
			IType[ ] typeArray = ( IType[ ] ) types.ToArray( typeof( IType ) );

			// TODO: 2.1 Uncomment once CriteriaImpl up to standard
			/*
			RowSelection selection = new RowSelection();
			selection.FirstRow = criteria.FirstResult;
			selection.MaxRows = criteria.MaxResults;
			selection.Timeout = criteria.Timeout;
			selection.FetchSize = criteria.FetchSize;

			QueryParameters qp = new QueryParameters( typeArray, valueArray, criteria.LockModes, selection );
			qp.Cacheable = criteria.Cacheable;
			qp.CacheRegion = criteria.CacheRegion;
			*/

			QueryParameters qp = new QueryParameters( typeArray, valueArray, criteria.LockModes, criteria.Selection );

			return List( session, qp, querySpaces, resultTypes );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="row"></param>
		/// <param name="rs"></param>
		/// <param name="session"></param>
		/// <returns></returns>
		protected override object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			//return criteria.ResultTransformer.TransformTuple( row, EntityAliases );
			return row[ row.Length - 1 ];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="config"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		protected override JoinType GetJoinType(
			IAssociationType type,
			OuterJoinFetchStrategy config,
			string path,
			string table,
			string[] foreignKeyColumns,
			ISessionFactoryImplementor factory )
		{
			if ( criteria.IsJoin( path ) )
			{
				return JoinType.InnerJoin;
			}
			else
			{
				FetchMode fm = criteria.GetFetchMode( path );
				//fm==null ||  - an Enum can't be null
				if( fm == FetchMode.Default )
				{
					return base.GetJoinType( type, config, path, table, foreignKeyColumns, factory );
				}
				else
				{
					return fm == FetchMode.Eager ? JoinType.LeftOuterJoin : JoinType.None;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>Uses the discriminator, to narrow the select to instances of the queried subclass.</remarks>
		protected override SqlString GetWhereFragment( )
		{
			return ( (IQueryable) Persister).QueryWhereFragment( Alias, true, true );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="className"></param>
		/// <param name="n"></param>
		/// <param name="path"></param>
		/// <param name="isLinkTable"></param>
		/// <returns></returns>
		protected override string GenerateTableAlias( string className, int n, string path, bool isLinkTable )
		{
			if ( !isLinkTable )
			{
				string userDefinedAlias = criteria.GetAlias( path );
				if ( userDefinedAlias != null )
				{
					return userDefinedAlias;
				}
			}
			return base.GenerateTableAlias( className, n, path, isLinkTable );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		protected override string GenerateRootAlias( string tableName )
		{
			return CriteriaImpl.RootAlias;
		}

		/// <summary>
		/// 
		/// </summary>
		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="space"></param>
		protected override void AddToPropertySpaces( object space )
		{
			querySpaces.Add( space );
		}

		protected override SqlString ApplyLocks( SqlString sqlSelectString, IDictionary lockModes, Dialect.Dialect dialect )
		{
			if ( lockModes == null || lockModes.Count == 0 )
			{
				return sqlSelectString;
			}
			else
			{
				ForUpdateFragment fragment = new ForUpdateFragment( lockModes );
				return sqlSelectString.Append( fragment.ToSqlStringFragment( dialect ) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lockModes"></param>
		/// <returns></returns>
		protected override LockMode[] GetLockModes( IDictionary lockModes )
		{
			string[] aliases = EntityAliases;
			int size = aliases.Length;
			LockMode[] lockModesArray = new LockMode[ size ];
			for( int i = 0; i < size; i++ )
			{
				LockMode lockMode = (LockMode) lockModes[ aliases[ i ] ];
				lockModesArray[ i ] = lockMode == null ? LockMode.None : lockMode;
			}
			return lockModesArray;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		protected override IList GetResultList( IList results )
		{
			// TODO: 2.1 Transform the results
			//return criteria.ResultTransformer.TransformList( results );
			return results;
		}
	}
}