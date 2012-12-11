using System;
using System.Collections.Generic;
using NHibernate.Hql;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class HQLExpressionQueryPlan : HQLQueryPlan, IQueryExpressionPlan
	{
		public IQueryExpression QueryExpression { get; private set; }

		public HQLExpressionQueryPlan(string expressionStr, IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(expressionStr, queryExpression, null, shallow, enabledFilters, factory)
		{
		}

		private HQLExpressionQueryPlan(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: base(expressionStr, CreateTranslators(expressionStr, queryExpression, collectionRole, shallow, enabledFilters, factory))
		{
			QueryExpression = queryExpression;
		}

		private static IQueryTranslator[] CreateTranslators(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			return factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(expressionStr, queryExpression, collectionRole, shallow, enabledFilters, factory);
		}
	}
}