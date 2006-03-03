using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Loader.Collection;
using NHibernate.Persister;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
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

		public OneToManyLoader( IQueryableCollection collPersister, ISessionFactoryImplementor factory )
			: this( collPersister, 1, factory )
		{
		}

		public OneToManyLoader( IQueryableCollection collPersister, int batchSize, ISessionFactoryImplementor factory )
			: base( factory.Dialect )
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

		private void InitClassPersisters( IOuterJoinLoadable persister, IList associations )
		{
			int joins = associations.Count;
			lockModeArray = CreateLockModeArray( joins + 1, LockMode.None );

			classPersisters = new ILoadable[joins + 1];
			Owners = new int[ joins + 1 ];
			for( int i = 0; i < joins; i++ )
			{
				OuterJoinableAssociation oj = ( OuterJoinableAssociation ) associations[ i ];
				Persisters[ i ] = (ILoadable) oj.Joinable;
				Owners[ i ] = ToOwner( oj, joins, oj.IsOneToOne );
			}
			classPersisters[ joins ] = persister;
			Owners[ joins ] = -1;

			if ( ArrayHelper.IsAllNegative( Owners ) )
			{
				Owners = null;
			}
		}

		protected override ICollectionPersister CollectionPersister
		{
			get { return collectionPersister; }
		}

		public void Initialize( object id, ISessionImplementor session )
		{
			LoadCollection( session, id, idType );
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

			SqlStringBuilder whereSqlString = WhereString( factory, alias, collPersister.KeyColumnNames, collPersister.KeyType, batchSize );

			if( collectionPersister.HasWhere )
			{
				whereSqlString
					.Add( " and " )
					.Add( collectionPersister.GetSQLWhereString( alias ) );
			}

			JoinFragment ojf = MergeOuterJoins( associations );

			SqlSelectBuilder select = new SqlSelectBuilder( factory )
				.SetSelectClause(
					collectionPersister.SelectFragment( alias, Suffixes[ joins ], true ).ToString() +
					SelectString( associations, factory )
				)
				.SetFromClause(
					persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true )
					)
				)
				.SetWhereClause( whereSqlString.ToSqlString() )
				.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString.Append(
					persister.WhereJoinFragment( alias, true, true )
					)
				);

			if( collectionPersister.HasOrdering )
			{
				select.SetOrderByClause( collectionPersister.GetSQLOrderByString( alias ) );
			}

			SqlString = select.ToSqlString();
		}
	}
}