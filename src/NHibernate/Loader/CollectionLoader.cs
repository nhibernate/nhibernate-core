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
	/// Loads a collection of values or a many-to-many association
	/// </summary>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer
	{
		private ICollectionPersister collectionPersister;
		private IType idType;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="factory"></param>
		public CollectionLoader( IQueryableCollection persister, ISessionFactoryImplementor factory ) : base( factory.Dialect )
		{
			idType = persister.KeyType;

			string alias = ToAlias( persister.TableName, 0 );

			//TODO: H2.0.3 the whereString is appended with the " and " - I don't think
			// that is needed because we are building SqlStrings differently and the Builder
			// probably already takes this into account.
			SqlString whereSqlString = null;
			if( persister.HasWhere )
			{
				whereSqlString = new SqlString( persister.GetSQLWhereString( alias ) );
			}

			IList associations = WalkCollectionTree( persister, alias, factory );

			int joins = associations.Count;
			Suffixes = new string[joins];
			for( int i = 0; i < joins; i++ )
			{
				Suffixes[ i ] = i.ToString() + StringHelper.Underscore;
			}

			JoinFragment ojf = OuterJoins( associations );

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder( factory );
			selectBuilder.SetSelectClause(
				persister.SelectFragment( alias ).ToString() +
					( joins == 0 ? String.Empty : ", " + SelectString( associations ) )
				)
				.SetFromClause( persister.TableName, alias )
				.SetWhereClause( alias, persister.KeyColumnNames, persister.KeyType )
				.SetOuterJoins( ojf.ToFromFragmentString, ojf.ToWhereFragmentString );

			if( persister.HasWhere )
			{
				selectBuilder.AddWhereClause( whereSqlString );
			}

			if( persister.HasOrdering )
			{
				selectBuilder.SetOrderByClause( persister.GetSQLOrderByString( alias ) );
			}

			this.SqlString = selectBuilder.ToSqlString();

			Persisters = new ILoadable[joins];
			LockModeArray = CreateLockModeArray( joins, LockMode.None );
			for( int i = 0; i < joins; i++ )
			{
				Persisters[ i ] = ( ILoadable ) ( ( OuterJoinableAssociation ) associations[ i ] ).Subpersister;
			}
			this.collectionPersister = persister;

			PostInstantiate();

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
	}
}