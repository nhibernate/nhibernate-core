using System;
using System.Collections;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Loads a collection of values or a many-to-many association
	/// </summary>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer
	{
		private readonly IQueryableCollection collectionPersister;
		private readonly IType keyType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		public CollectionLoader( IQueryableCollection persister, ISessionFactoryImplementor factory ) : this( persister, 1, factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="batchSize"></param>
		/// <param name="factory"></param>
		public CollectionLoader( IQueryableCollection persister, int batchSize, ISessionFactoryImplementor factory ) : base( factory.Dialect )
		{
			this.collectionPersister = persister;
			this.keyType = persister.KeyType;

			string alias = GenerateRootAlias( persister.TableName );
			IList associations = WalkCollectionTree( persister, alias, factory );

			InitStatementString( persister, alias, associations, batchSize, factory );
			InitClassPersisters( associations );
			
			PostInstantiate();
		}

		private void InitClassPersisters( IList associations )
		{
			int joins = associations.Count;
			LockModeArray = CreateLockModeArray( joins, LockMode.None );

			classPersisters = new ILoadable[ joins ];
			SetOwners( new int[ joins ] );

			int i = 0;
			foreach( OuterJoinableAssociation oj in associations )
			{
				classPersisters[ i ] = (ILoadable) oj.Joinable;
				Owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
				i++;
			}

			if ( ArrayHelper.IsAllNegative( Owners ) )
			{
				SetOwners( null );
			}
		}

		/// <summary></summary>
		protected override ICollectionPersister CollectionPersister
		{
			get { return collectionPersister; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="session"></param>
		public void Initialize( object id, ISessionImplementor session )
		{
			LoadCollection( session, id, keyType );
		}

		private void InitStatementString( IQueryableCollection persister, string alias, IList associations, int batchSize, ISessionFactoryImplementor factory )
		{
			Suffixes = GenerateSuffixes( associations.Count );

			SqlString whereString = WhereString( factory, alias, persister.KeyColumnNames, persister.KeyType, batchSize );
			if ( persister.HasWhere )
			{
				whereString = whereString.Append( " and ").Append( persister.GetSQLWhereString( alias ) );
			}

			JoinFragment ojf = MergeOuterJoins( associations );
			SqlSelectBuilder select = new SqlSelectBuilder( factory )
				.SetSelectClause(
					persister.SelectFragment( alias ).Append(
					SelectString( associations, factory ) ).ToString()
				)
				.SetFromClause( persister.TableName, alias )
				.AddWhereClause( whereString )
				.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString 
				);

			if ( persister.HasOrdering )
			{
				select.SetOrderByClause( persister.GetSQLOrderByString( alias ) );
			}
			SqlString = select.ToSqlString();
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
			ISessionFactoryImplementor factory
			)
		{
			JoinType joinType = base.GetJoinType( type, config, path, table, foreignKeyColumns, factory );
			// We can use an inner-join for the many-to-many
			if ( joinType == JoinType.LeftOuterJoin && path.Equals( string.Empty ) )
			{
				joinType = JoinType.InnerJoin;
			}
			return joinType;
		}
	}
}