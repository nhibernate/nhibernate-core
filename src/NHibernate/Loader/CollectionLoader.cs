using System;
using System.Text;
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
		private CollectionPersister collectionPersister;
		private IType idType;
		
		public CollectionLoader(CollectionPersister persister, ISessionFactoryImplementor factory) : base(factory.Dialect) 
		{
			idType = persister.KeyType;

			string alias = Alias( persister.QualifiedTableName, 0);

			//TODO: H2.0.3 the whereString is appended with the " and " - I don't think
			// that is needed because we are building SqlStrings differently and the Builder
			// probably already takes this into account.
			SqlString whereSqlString = null;
			if (persister.HasWhere) 
				whereSqlString = new SqlString(persister.GetSQLWhereString(alias));
				
			IList associations = WalkCollectionTree(persister, alias, factory);

			int joins = associations.Count;
			suffixes = new string[joins];
			for (int i=0; i<joins; i++) suffixes[i] = i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);
			
			SqlSelectBuilder selectBuilder = new SqlSelectBuilder(factory);
			selectBuilder.SetSelectClause(
				persister.SelectClauseFragment(alias) + 
				(joins==0 ? String.Empty: ", " + SelectString(associations))
				)
				.SetFromClause(persister.QualifiedTableName, alias)
				.SetWhereClause(alias, persister.KeyColumnNames, persister.KeyType)
				.SetOuterJoins(ojf.ToFromFragmentString, ojf.ToWhereFragmentString);
			
			if(persister.HasWhere) selectBuilder.AddWhereClause(whereSqlString);
				
			if(persister.HasOrdering) selectBuilder.SetOrderByClause(persister.GetSQLOrderByString(alias));

			this.sqlString = selectBuilder.ToSqlString();

			classPersisters = new ILoadable[joins];
			lockModeArray = CreateLockModeArray(joins, LockMode.None);
			for (int i=0; i<joins; i++) classPersisters[i] = (ILoadable) ((OuterJoinableAssociation) associations[i]).Subpersister;
			this.collectionPersister = persister;

			PostInstantiate();
			
		}

		protected override CollectionPersister CollectionPersister 
		{
			get { return collectionPersister; }
		}

		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session) 
		{
			LoadCollection(session, id, idType, owner, collection);
		}
	}
}
