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

		public CriteriaLoader( IOuterJoinLoadable persister, ISessionFactoryImplementor factory, CriteriaImpl criteria )
			: base( persister, factory )
		{
			this.criteria = criteria;

			AddAllToPropertySpaces( persister.PropertySpaces );

			resultTypes = new IType[ 1 ];
			resultTypes[ 0 ] = NHibernateUtil.Entity( persister.MappedClass );

			SqlStringBuilder condition = new SqlStringBuilder( 10 );

			bool foundCriterion = false;

			foreach( CriteriaImpl.CriterionEntry ee in criteria.IterateExpressionEntries() )
			{
				if( foundCriterion )
				{
					condition.Add( " and " );
				}

				SqlString sqlString = ee.Criterion.ToSqlString(
					factory,
					criteria.GetPersistentClass( ee.Alias ),
					ee.Alias,
					criteria.AliasClasses );
				condition.Add( sqlString );

				foundCriterion = true;
			}

			if( !foundCriterion )
			{
				condition.Add( "1=1" ); // TODO: fix this ugliness
			}

			StringBuilder orderBy = new StringBuilder( 30 );
			bool foundOrder = false;

			foreach( Order ord in criteria.IterateOrderings() )
			{
				if( foundOrder )
				{
					orderBy.Append( StringHelper.CommaSpace );
				}
				orderBy.Append( ord.ToSqlString( factory, criteria.CriteriaClass, Alias ) );
				foundOrder = true;
			}

			IList associations = WalkTree( persister, Alias, factory );
			InitClassPersisters( associations );
			InitStatementString( associations, condition.ToSqlString(), orderBy.ToString(), factory );

			PostInstantiate();
		}

		public IList List( ISessionImplementor session )
		{
			ArrayList values = new ArrayList();
			ArrayList types = new ArrayList();

			foreach( CriteriaImpl.CriterionEntry ce in criteria.IterateExpressionEntries() )
			{
				TypedValue[ ] tv = ce.Criterion.GetTypedValues(
					session.Factory,
					criteria.GetCriteriaClass( ce.Alias ),
					criteria.AliasClasses );

				for( int i = 0; i < tv.Length; i++ )
				{
					values.Add( tv[ i ].Value );
					types.Add( tv[ i ].Type );
				}
			}
			object[ ] valueArray = values.ToArray();
			IType[ ] typeArray = ( IType[ ] ) types.ToArray( typeof( IType ) );

			RowSelection selection = new RowSelection();
			selection.FirstRow = criteria.FirstResult;
			selection.MaxRows = criteria.MaxResults;
			selection.Timeout = criteria.Timeout;
			selection.FetchSize = criteria.FetchSize;

			QueryParameters qp = new QueryParameters( typeArray, valueArray, criteria.LockModes, selection );
			qp.Cacheable = criteria.Cacheable;
			qp.CacheRegion = criteria.CacheRegion;

			return List( session, qp, querySpaces, resultTypes );
		}

		protected override object GetResultColumnOrRow( object[ ] row, IDataReader rs, ISessionImplementor session )
		{
			return criteria.ResultTransformer.TransformTuple( row, EntityAliases );
		}

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

		/// <remarks>Uses the discriminator, to narrow the select to instances of the queried subclass.</remarks>
		protected override SqlString GetWhereFragment( )
		{
			return ( (IQueryable) Persister).QueryWhereFragment( Alias, true, true );
		}

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

		protected override string GenerateRootAlias( string tableName )
		{
			return CriteriaUtil.RootAlias;
		}

		public ISet QuerySpaces
		{
			get { return querySpaces; }
		}

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

		protected override IList GetResultList( IList results )
		{
			return criteria.ResultTransformer.TransformList( results );
		}
	}
}