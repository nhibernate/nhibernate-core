using System;
using System.Text;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {
	/// <summary>
	/// Loads one-to-many associations
	/// </summary>
	public class OneToManyLoader : OuterJoinLoader, ICollectionInitializer	{
		
		private CollectionPersister collectionPersister;
		private IType idType;
		
		public OneToManyLoader(CollectionPersister collPersister, ISessionFactoryImplementor factory) : base ( factory.Dialect ) {
			collectionPersister = collPersister;
			idType = collectionPersister.KeyType;

			ILoadable persister = (ILoadable) factory.GetPersister(
				((EntityType) collPersister.ElementType).PersistentClass);

			string alias = Alias(collectionPersister.QualifiedTableName, 0);
			string collAlias = persister.GetConcreteClassAlias(alias);

			SqlString whereSqlString = null;

			if (collectionPersister.HasWhere) 
				whereSqlString = new SqlString(collectionPersister.GetSQLWhereString(collAlias));
				
			IList associations = WalkTree(persister, alias, factory);

			int joins=associations.Count;
			suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) suffixes[i] = (joins==0) ? String.Empty : i.ToString() + StringHelper.Underscore;


			JoinFragment ojf = OuterJoins(associations);

			SqlSelectBuilder selectBuilder = new SqlSelectBuilder(factory);
			
			selectBuilder.SetSelectClause(collectionPersister.SelectClauseFragment(collAlias) +
				(joins==0 ? String.Empty : "," + SelectString(associations) ) +
				", " +
				SelectString( persister, alias, suffixes[joins] )
				);


			selectBuilder.SetFromClause(
				persister.FromTableFragment(alias) +
				persister.FromJoinFragment(alias, true, true)
				);

			selectBuilder.SetWhereClause(collAlias, collectionPersister.KeyColumnNames, collectionPersister.KeyType);
			if(collectionPersister.HasWhere) selectBuilder.AddWhereClause(whereSqlString);

			selectBuilder.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString +
				persister.WhereJoinFragment(alias, true, true)
				);

			if(collectionPersister.HasOrdering) selectBuilder.SetOrderByClause( collectionPersister.GetSQLOrderByString(collAlias) );

			this.sqlString = selectBuilder.ToSqlString();


			classPersisters = new ILoadable[joins+1];
			for (int i=0; i<joins; i++) classPersisters[i] = ((OuterJoinableAssociation) associations[i]).Subpersister;
			classPersisters[joins] = persister;
			
			lockModeArray = createLockModeArray(joins+1, LockMode.None);
			PostInstantiate();
		}

		protected override CollectionPersister CollectionPersister {
			get { return collectionPersister; }
		}

		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session) {
			LoadCollection(session, id, idType, owner, collection);
		}
	}
}
