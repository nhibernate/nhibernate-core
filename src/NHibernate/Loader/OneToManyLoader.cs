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

			//ILoadable persister = ( ILoadable ) factory.GetPersister( ( ( EntityType ) collPersister.ElementType ).AssociatedClass );
			ILoadable persister = ( ILoadable ) collPersister.ElementPersister;

			string alias = ToAlias( collectionPersister.TableName, 0 );

			SqlString whereSqlString = null;

			if( collectionPersister.HasWhere )
			{
				whereSqlString = new SqlString( collectionPersister.GetSQLWhereString( alias ) );
			}

			IList associations = WalkTree( persister, alias, factory );

			int joins = associations.Count;
			Suffixes = new string[joins + 1];
			for( int i = 0; i <= joins; i++ )
			{
				Suffixes[ i ] = ( joins == 0 ) ? String.Empty : i.ToString() + StringHelper.Underscore;
			}

			JoinFragment ojf = OuterJoins( associations );

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder( factory );

			selectBuilder.SetSelectClause(
				collectionPersister.SelectFragment( alias ).ToString() +
					( joins == 0 ? String.Empty : "," + SelectString( associations ) ) +
					", " +
					SelectString( persister, alias, Suffixes[ joins ] )
				);


			selectBuilder.SetFromClause(
				persister.FromTableFragment( alias ).Append(
					persister.FromJoinFragment( alias, true, true )
					)
				);

			selectBuilder.SetWhereClause( alias, collectionPersister.KeyColumnNames, collectionPersister.KeyType );
			if( collectionPersister.HasWhere )
			{
				selectBuilder.AddWhereClause( whereSqlString );
			}

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


			Persisters = new ILoadable[joins + 1];
			LockModeArray = CreateLockModeArray( joins + 1, LockMode.None );
			for( int i = 0; i < joins; i++ )
			{
				Persisters[ i ] = ( ( OuterJoinableAssociation ) associations[ i ] ).Subpersister;
			}
			Persisters[ joins ] = persister;

			PostInstantiate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="mappingDefault"></param>
		/// <param name="path"></param>
		/// <param name="table"></param>
		/// <param name="foreignKeyColumns"></param>
		/// <returns></returns>
		protected override bool EnableJoinedFetch( bool mappingDefault, string path, string table, string[ ] foreignKeyColumns )
		{
			return mappingDefault && (
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
		/// <param name="collection"></param>
		/// <param name="owner"></param>
		/// <param name="session"></param>
		public void Initialize( object id, PersistentCollection collection, object owner, ISessionImplementor session )
		{
			LoadCollection( session, id, idType, owner, collection );
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
	}
}