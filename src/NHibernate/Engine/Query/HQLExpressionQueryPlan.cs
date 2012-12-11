using System;
using System.Collections.Generic;
using NHibernate.Hql;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class HQLExpressionQueryPlan : HQLQueryPlan, IQueryExpressionPlan
	{
		public IQueryExpression QueryExpression { get; private set; }

		public HQLExpressionQueryPlan(IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(queryExpression, null, shallow, enabledFilters, factory)
		{
		}

		private HQLExpressionQueryPlan(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: base(queryExpression.Key, CreateTranslators(queryExpression, collectionRole, shallow, enabledFilters, factory))
		{
			QueryExpression = queryExpression;
		}

		private static IQueryTranslator[] CreateTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			return factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(queryExpression, collectionRole, shallow, enabledFilters, factory);
		}
	}
}