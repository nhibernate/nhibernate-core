using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Walker for collections of values and many-to-many associations
	/// </summary>
	public class BasicCollectionJoinWalker : CollectionJoinWalker
	{
		private readonly IQueryableCollection collectionPersister;

		public BasicCollectionJoinWalker(IQueryableCollection collectionPersister, int batchSize,
			SqlString subquery, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.collectionPersister = collectionPersister;
			string alias = GenerateRootAlias(collectionPersister.Role);

			WalkCollectionTree(collectionPersister, alias);

			IList<OuterJoinableAssociation> allAssociations = new List<OuterJoinableAssociation>(associations);

			// NH Different behavior : passing enabledFilters instead empty-filter
			allAssociations.Add(
				new OuterJoinableAssociation(collectionPersister.CollectionType, null, null, alias, JoinType.LeftOuterJoin, null, Factory,
				                             enabledFilters));

			InitPersisters(allAssociations, LockMode.None);
			InitStatementString(alias, batchSize, subquery);
		}

		private void InitStatementString(string alias, int batchSize, SqlString subquery)
		{
			int joins = CountEntityPersisters(associations);
			int collectionJoins = CountCollectionPersisters(associations) + 1;
			Suffixes = BasicLoader.GenerateSuffixes(joins);
			CollectionSuffixes = BasicLoader.GenerateSuffixes(joins, collectionJoins);

			SqlStringBuilder whereString = WhereString(alias, collectionPersister.KeyColumnNames, subquery, batchSize);

			string manyToManyOrderBy = string.Empty;
			string filter = collectionPersister.FilterFragment(alias, EnabledFilters);

			if (collectionPersister.IsManyToMany)
			{
				// from the collection of associations, locate OJA for the
				// ManyToOne corresponding to this persister to fully
				// define the many-to-many; we need that OJA so that we can
				// use its alias here
				// TODO : is there a better way here?
				IAssociationType associationType = (IAssociationType)collectionPersister.ElementType;
				foreach (OuterJoinableAssociation oja in associations)
				{
					if (oja.JoinableType == associationType)
					{
						// we found it
						filter += collectionPersister.GetManyToManyFilterFragment(oja.RHSAlias, EnabledFilters);
						manyToManyOrderBy += collectionPersister.GetManyToManyOrderByString(oja.RHSAlias);
					}
				}
			}

			whereString.Insert(0, StringHelper.MoveAndToBeginning(filter));
			JoinFragment ojf = MergeOuterJoins(associations);

			SqlSelectBuilder select =
				new SqlSelectBuilder(Factory)
				.SetSelectClause(collectionPersister.SelectFragment(alias, CollectionSuffixes[0])
				                                              + SelectString(associations))
				.SetFromClause(collectionPersister.TableName, alias)
				.SetWhereClause(whereString.ToSqlString())
				.SetOuterJoins(ojf.ToFromFragmentString, ojf.ToWhereFragmentString);

			select.SetOrderByClause(OrderBy(associations, MergeOrderings(collectionPersister.GetSQLOrderByString(alias), manyToManyOrderBy)));

			if (Factory.Settings.IsCommentsEnabled)
				select.SetComment("load collection " + collectionPersister.Role);

			SqlString = select.ToSqlString();
		}


		public override string ToString()
		{
			return GetType().FullName + '(' + collectionPersister.Role + ')';
		}
	}
}