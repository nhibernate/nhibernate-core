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
	/// Loads a collection of values or a many-to-many association
	/// </summary>
	public class CollectionLoader : OuterJoinLoader, ICollectionInitializer {
		private CollectionPersister collectionPersister;
		private IType idType;
		private bool allowTwoPhaseLoad;

		public CollectionLoader(CollectionPersister persister, ISessionFactoryImplementor session) : base(session.Dialect) {
			allowTwoPhaseLoad = !persister.IsSet;

			idType = persister.KeyType;

			string alias = Alias( persister.QualifiedTableName, 0);

			string whereString="";
			if (persister.HasWhere) whereString = " and " + persister.GetSQLWhereString(alias);

			IList associations = WalkTree(persister, alias, session);

			int joins = associations.Count;
			suffixes = new string[joins];
			for (int i=0; i<joins; i++) suffixes[i] = i.ToString() + StringHelper.Underscore;

			JoinFragment ojf = OuterJoins(associations);
			Select select = new Select()
				.SetSelectClause(
					persister.SelectClauseFragment(alias) +
					(joins==0 ? StringHelper.EmptyString : ", " + SelectString(associations) )
					)
				.SetFromClause ( persister.QualifiedTableName, alias )
				.SetWhereClause(
					new ConditionalFragment().SetTableAlias(alias)
					.SetCondition( persister.KeyColumnNames, "?" )
					.ToFragmentString() +
					whereString
				)
				.SetOuterJoins(
					ojf.ToFromFragmentString,
					ojf.ToWhereFragmentString
				);
			if (persister.HasOrdering) select.SetOrderByClause( persister.GetSQLOrderByString(alias));
			sql = select.ToStatementString();

			classPersisters = new ILoadable[joins];
			for (int i=0; i<joins; i++) classPersisters[i] = (ILoadable) ((OuterJoinableAssociation) associations[i]).Subpersister;
			this.collectionPersister = persister;
		}

		protected override CollectionPersister CollectionPersister {
			get { return collectionPersister; }
		}

		public void Initialize(object id, PersistentCollection collection, object owner, ISessionImplementor session) {
			LoadCollection(session, id, idType, owner, collection);
		}

		protected override bool AllowTwoPhaseLoad {
			get { return allowTwoPhaseLoad; }
		}
	}
}
