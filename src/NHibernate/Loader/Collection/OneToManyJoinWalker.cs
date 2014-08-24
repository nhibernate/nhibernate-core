using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Walker for one-to-many associations
	/// </summary>
	/// <seealso cref="OneToManyLoader" />
	public class OneToManyJoinWalker : CollectionJoinWalker
	{
		private readonly IOuterJoinLoadable elementPersister;
		private readonly IQueryableCollection oneToManyPersister;

		protected override bool IsDuplicateAssociation(string foreignKeyTable, string[] foreignKeyColumns)
		{
			//disable a join back to this same association
			bool isSameJoin = oneToManyPersister.TableName.Equals(foreignKeyTable)
			                  && CollectionHelper.CollectionEquals<string>(foreignKeyColumns, oneToManyPersister.KeyColumnNames);
			return isSameJoin || base.IsDuplicateAssociation(foreignKeyTable, foreignKeyColumns);
		}

		public OneToManyJoinWalker(IQueryableCollection oneToManyPersister, int batchSize, SqlString subquery,
		                           ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(factory, enabledFilters)
		{
			this.oneToManyPersister = oneToManyPersister;
			elementPersister = (IOuterJoinLoadable)oneToManyPersister.ElementPersister;
			string alias = GenerateRootAlias(oneToManyPersister.Role);

			WalkEntityTree(elementPersister, alias);

			IList<OuterJoinableAssociation> allAssociations = new List<OuterJoinableAssociation>(associations);
			allAssociations.Add(
				new OuterJoinableAssociation(oneToManyPersister.CollectionType, null, null, alias, JoinType.LeftOuterJoin, null, Factory,
				                             new CollectionHelper.EmptyMapClass<string, IFilter>()));

			InitPersisters(allAssociations, LockMode.None);
			InitStatementString(elementPersister, alias, batchSize, subquery);
		}

		protected override string GenerateAliasForColumn(string rootAlias, string column)
		{
			// NH-1747 FIX (see OneToManyPersister)
			return elementPersister.GenerateTableAliasForColumn(rootAlias, column);
		}

		private void InitStatementString(IOuterJoinLoadable elementPersister, string alias, int batchSize, SqlString subquery)
		{
			int joins = CountEntityPersisters(associations);
			Suffixes = BasicLoader.GenerateSuffixes(joins + 1);

			int collectionJoins = CountCollectionPersisters(associations) + 1;
			CollectionSuffixes = BasicLoader.GenerateSuffixes(joins + 1, collectionJoins);

			SqlStringBuilder whereString = WhereString(alias, oneToManyPersister.KeyColumnNames, subquery, batchSize);
			string filter = oneToManyPersister.FilterFragment(alias, EnabledFilters);
			whereString.Insert(0, StringHelper.MoveAndToBeginning(filter));

			JoinFragment ojf = MergeOuterJoins(associations);
			SqlSelectBuilder select =
				new SqlSelectBuilder(Factory).SetSelectClause(
					oneToManyPersister.SelectFragment(null, null, alias, Suffixes[joins], CollectionSuffixes[0], true)
					+ SelectString(associations)).SetFromClause(elementPersister.FromTableFragment(alias)
					                                            + elementPersister.FromJoinFragment(alias, true, true)).SetWhereClause(
					whereString.ToSqlString()).SetOuterJoins(ojf.ToFromFragmentString,
					                                         ojf.ToWhereFragmentString
					                                         + elementPersister.WhereJoinFragment(alias, true, true));

			select.SetOrderByClause(OrderBy(associations, oneToManyPersister.GetSQLOrderByString(alias)));

			if (Factory.Settings.IsCommentsEnabled)
			{
				select.SetComment("load one-to-many " + oneToManyPersister.Role);
			}

			SqlString = select.ToSqlString();
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + oneToManyPersister.Role + ')';
		}
	}
}