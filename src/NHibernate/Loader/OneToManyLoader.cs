using System;
using System.Text;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader {
	/// <summary>
	/// Loads one-to-many associations
	/// </summary>
	public class OneToManyLoader : OuterJoinLoader, ICollectionInitializer	{
		
		private CollectionPersister collectionPersister;
		private IType idType;
		
		public OneToManyLoader(CollectionPersister collPersister, ISessionFactoryImplementor session) : base ( session.Dialect ) {
			collectionPersister = collPersister;
			idType = collectionPersister.KeyType;

			ILoadable persister = (ILoadable) session.GetPersister(
				((EntityType) collPersister.ElementType).PersistentClass);

			string alias = Alias(collectionPersister.QualifiedTableName, 0);
			string collAlias = persister.GetConcreteClassAlias(alias);

			string whereString="";
			if (collectionPersister.HasWhere) whereString = " and " + collectionPersister.GetSQLWhereString(collAlias);

			IList associations = WalkTree(persister, alias, session);

			int joins=associations.Count;
			suffixes = new string[joins+1];
			for (int i=0; i<=joins; i++) suffixes[i] = (joins==0) ? StringHelper.EmptyString : i.ToString() + StringHelper.Underscore;


			JoinFragment ojf = OuterJoins(associations);
			Select select = new Select()
				.SetSelectClause(
				collectionPersister.SelectClauseFragment(collAlias) +
				(joins==0 ? StringHelper.EmptyString : "," + SelectString(associations) ) +
				", " +
				SelectString( persister, alias, suffixes[joins] )
				)
				.SetFromClause(
				persister.FromTableFragment(alias) +
				persister.FromJoinFragment(alias, true, true)
				)
				.SetWhereClause(
				new ConditionalFragment().SetTableAlias(collAlias)
				.SetCondition( collectionPersister.KeyColumnNames, "?" )
				.ToFragmentString()
				+ whereString
				)
				.SetOuterJoins(
				ojf.ToFromFragmentString,
				ojf.ToWhereFragmentString +
				persister.WhereJoinFragment(alias, true, true)
				);
			if (collectionPersister.HasOrdering) select.SetOrderByClause( collectionPersister.GetSQLOrderByString(collAlias) );
			sql = select.ToStatementString();

			classPersisters = new ILoadable[joins+1];
			for (int i=0; i<joins; i++) classPersisters[i] = ((OuterJoinableAssociation) associations[i]).Subpersister;
			classPersisters[joins] = persister;
		}

		protected override CollectionPersister CollectionPersister {
			get { return collectionPersister; }
		}

		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session) {
			LoadCollection(session, id, idType, owner, collection);
		}
	}
}
