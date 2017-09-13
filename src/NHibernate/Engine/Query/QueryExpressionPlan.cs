using System;
using System.Collections.Generic;
using NHibernate.Hql;
using NHibernate.Linq;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class QueryExpressionPlan : HQLQueryPlan, IQueryExpressionPlan
	{
		public IQueryExpression QueryExpression { get; }

		public QueryExpressionPlan(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(queryExpression.Key, CreateTranslators(queryExpression, collectionRole, shallow, enabledFilters, factory))
		{
			QueryExpression = queryExpression;
		}

		public QueryExpressionPlan(IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(queryExpression.Key, CreateTranslators(queryExpression, null, shallow, enabledFilters, factory))
		{
			QueryExpression = queryExpression;
		}

		protected QueryExpressionPlan(string key, IQueryTranslator[] translators)
			: base(key, translators)
		{
		}

		protected QueryExpressionPlan(HQLQueryPlan source, IQueryExpression expression) 
			: base(source)
		{
			QueryExpression = expression;
		}

		protected static IQueryTranslator[] CreateTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			return factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(queryExpression, collectionRole, shallow, enabledFilters, factory);
		}

		public virtual QueryExpressionPlan Copy(IQueryExpression expression)
		{
			return new QueryExpressionPlan(this, expression);
		}
	}
}
