using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Loader.Entity
{
	internal partial class DynamicEntityLoader : EntityLoader
	{
		readonly string _alias;

		public DynamicEntityLoader(IOuterJoinLoadable persister, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(persister, lockMode, factory, enabledFilters)
		{
			var walker = new DynamicEntityJoinWalker(persister, persister.IdentifierColumnNames, lockMode, factory, enabledFilters);

			InitFromWalker(walker);
			this._alias = walker.Alias;
			PostInstantiate();
		}

		protected override bool IsSingleRowLoader => false;

		public virtual IList DoEntityBatchFetch(ISessionImplementor session, QueryParameters queryParameters)
		{
			return LoadEntityBatch(session, persister, queryParameters);
		}

		private protected override SqlString TransformSql(SqlString sqlString, QueryParameters queryParameters, HashSet<IParameterSpecification> parameterSpecifications)
		{
			var columns = StringHelper.Qualify(_alias, persister.KeyColumnNames);
			DynamicBatchingHelper.ExpandBatchIdPlaceholder(sqlString, queryParameters, columns, Factory.Dialect, out var parameters, out var result);
			parameterSpecifications.UnionWith(CreateParameterSpecificationsAndAssignBackTrack(parameters));
			return result;
		}

		class DynamicEntityJoinWalker : EntityJoinWalker
		{
			public DynamicEntityJoinWalker(IOuterJoinLoadable persister, string[] identifierColumnNames, LockMode lockMode, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters) : base(persister, identifierColumnNames, -1, lockMode, factory, enabledFilters)
			{
			}

			protected override SqlStringBuilder WhereString(string alias, string[] columnNames, int batchSize)
			{
				return DynamicBatchingHelper.BuildBatchFetchRestrictionFragment();
			}

			new public string Alias => base.Alias;
		}
	}
}
