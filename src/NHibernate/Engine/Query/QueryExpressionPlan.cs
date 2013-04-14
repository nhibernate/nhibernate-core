using System;
using System.Collections.Generic;
using NHibernate.Hql;
using NHibernate.Linq;

namespace NHibernate.Engine.Query
{
	[Serializable]
	public class QueryExpressionPlan : HQLQueryPlan, IQueryExpressionPlan
	{
		public IQueryExpression QueryExpression { get; private set; }
	    
	    public QueryExpressionPlan(IQueryExpression queryExpression, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
			: this(queryExpression.Key, CreateTranslators(queryExpression, null, shallow, enabledFilters, factory))
		{
			QueryExpression = queryExpression;
		}

        protected QueryExpressionPlan(QueryExpressionPlan source, IQueryExpression newQueryExpression)
            : base(source)
        {
            QueryExpression = newQueryExpression;
        }

		protected QueryExpressionPlan(string key, IQueryTranslator[] translators)
			: base(key, translators)
		{
		}

		protected static IQueryTranslator[] CreateTranslators(IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
		{
			return factory.Settings.QueryTranslatorFactory.CreateQueryTranslators(queryExpression, collectionRole, shallow, enabledFilters, factory);
		}

        public virtual IQueryExpressionPlan Copy(IQueryExpression newExpression)
        {
            if (newExpression == null) throw new ArgumentNullException("newExpression");

            return new QueryExpressionPlan(this, newExpression);
        }
	}
}