using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	internal class DynamicBatchingCollectionLoader : CollectionLoader
	{
		readonly string _alias;

		public DynamicBatchingCollectionLoader(IQueryableCollection collectionPersister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(collectionPersister, factory, enabledFilters)
		{
			JoinWalker walker = BuildJoinWalker(collectionPersister, factory, enabledFilters);
			InitFromWalker(walker);
			_alias = StringHelper.GenerateAlias(collectionPersister.Role, 0);
			PostInstantiate();
		}

		private JoinWalker BuildJoinWalker(IQueryableCollection collectionPersister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			if (collectionPersister.IsOneToMany)
				return new DynamicOneToManyJoinWalker(collectionPersister, factory, enabledFilters);

			return new DynamicBasicCollectionJoinWalker(collectionPersister, factory, enabledFilters);
		}

		private protected override SqlString TransformSql(SqlString sqlString, QueryParameters queryParameters, HashSet<IParameterSpecification> parameterSpecifications)
		{
			var columns = StringHelper.Qualify(_alias, CollectionPersister.KeyColumnNames);
			return DynamicBatchingHelper.ExpandBatchIdPlaceholder(sqlString, parameterSpecifications, columns, queryParameters.PositionalParameterTypes, Factory);
		}

		private class DynamicOneToManyJoinWalker : OneToManyJoinWalker
		{
			public DynamicOneToManyJoinWalker(IQueryableCollection collectionPersister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(collectionPersister, 1, null, factory, enabledFilters)
			{
			}

			protected override SqlStringBuilder WhereString(string alias, string[] columnNames, int batchSize)
			{
				return DynamicBatchingHelper.BuildBatchFetchRestrictionFragment();
			}
		}

		private class DynamicBasicCollectionJoinWalker : BasicCollectionJoinWalker
		{
			public DynamicBasicCollectionJoinWalker(IQueryableCollection collectionPersister, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(collectionPersister, 1, null, factory, enabledFilters)
			{
			}

			protected override SqlStringBuilder WhereString(string alias, string[] columnNames, int batchSize)
			{
				return DynamicBatchingHelper.BuildBatchFetchRestrictionFragment();
			}
		}
	}
}
