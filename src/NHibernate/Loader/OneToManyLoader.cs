using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader
{
	/// <summary>
	/// Loads one-to-many associations
	/// </summary>
	/// <remarks>
	/// The collection persister must implement IQueryableCollection. For
	/// other collections, create a customized subclass of Loader.
	/// </remarks>
	public class OneToManyLoader : OuterJoinLoader, ICollectionInitializer
	{
		private IQueryableCollection collectionPersister;
		private IType idType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collPersister"></param>
		/// <param name="factory"></param>
		public OneToManyLoader( IQueryableCollection collPersister, ISessionFactoryImplementor factory ) : this( collPersister, 1, factory )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collPersister"></param>
		/// <param name="batchSize"></param>
		/// <param name="factory"></param>
		public OneToManyLoader( IQueryableCollection collPersister, int batchSize, ISessionFactoryImplementor factory ) : base( factory.Dialect )
		{
			collectionPersister = collPersister;
			idType = collectionPersister.KeyType;

			IOuterJoinLoadable persister = ( IOuterJoinLoadable ) collPersister.ElementPersister;

			string alias = GenerateRootAlias( collPersister.Role );
			IList associations = WalkTree( persister, alias, factory );

			InitStatementString( collPersister, persister, alias, associations, batchSize, factory );
			InitClassPersisters( persister, associations );

			PostInstantiate();
		}

		/// <summary>
		/// Disable a join back to this same association
		/// </summary>
		/// <param name="type"></param>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected override bool IsJoinedFetchEnabled( IType type, bool mappingDefault, string path, string table, string[] foreignKeyColumns )
		{
			return base.IsJoinedFetchEnabled( type, mappingDefault, path, table, foreignKeyColumns) && (
				!table.Equals( collectionPersister.TableName ) || 
				!ArrayHelper.Equals( foreignKeyColumns, collectionPersister.KeyColumnNames ) 
				);
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
			LoadCollection( session, id, idType );
		}

		private void InitClassPersisters( IOuterJoinLoadable persister, IList associations )
		{
			int joins = associations.Count;
			LockModeArray = CreateLockModeArray( joins + 1, LockMode.None );

			Persisters = new ILoadable[joins + 1];
			SetOwners( new int[ joins + 1 ] );
			for( int i = 0; i < joins; i++ )
			{
				OuterJoinableAssociation oj = ( OuterJoinableAssociation ) associations[ i ];
				Persisters[ i ] = (ILoadable) oj.Joinable;
				Owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
			}
			Persisters[ joins ] = persister;
			Owners[ joins ] = -1;

			if ( ArrayHelper.IsAllNegative( Owners ) )
			{
				SetOwners( null );
			}
		}

		private void InitStatementString(
			IQueryableCollection collPersister,
			IOuterJoinLoadable persister,
			string alias,
			IList associations,
			int batchSize,
			ISessionFactoryImplementor factory
			)
		{
			int joins = associations.Count;

			Suffixes = GenerateSuffixes( joins + 1 );

			SqlString whereSqlString = WhereString( factory, alias, collPersister.KeyColumnNames, collPersister.KeyType, batchSize );

			if( collectionPersister.HasWhere )
			{
				whereSqlString = whereSqlString.Append( " and " ).Append( collectionPersister.GetSQLWhereString( alias ) );
			}

			JoinFragment ojf = MergeOuterJoins( associations );

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder( factory );
			selectBuilder.SetSelectClause(
					collectionPersister.SelectFragment( alias, Suffixes[ joins ], true ).ToString() +
					SelectString( associations, factory )
				);
			selectBuilder.SetFromClause(
					persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true )
					)
				);
			selectBuilder.SetWhereClause( whereSqlString );
			selectBuilder.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString.Append(
					persister.WhereJoinFragment( alias, true, true )
					)
				);

			if( collectionPersister.HasOrdering )
			{
				selectBuilder.SetOrderByClause( collectionPersister.GetSQLOrderByString( alias ) );
			}

			this.SqlString = selectBuilder.ToSqlString();
		}
	}
}