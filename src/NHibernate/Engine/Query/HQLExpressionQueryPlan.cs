using System;
using System.Collections.Generic;
using NHibernate.Hql;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Engine.Query
{
    [Serializable]
    public class HQLExpressionQueryPlan : HQLQueryPlan, IQueryExpressionPlan
    {
        public IQueryExpression QueryExpression
        {
            get;
            protected set;
        }

		private HQLExpressionQueryPlan(HQLQueryPlan source, IQueryExpression newQueryExpression)
			: base(source)
		{
			QueryExpression = newQueryExpression;
		}

		internal HQLExpressionQueryPlan Copy(IQueryExpression newExpression)
		{
			return new HQLExpressionQueryPlan(this, newExpression);
		}

        public HQLExpressionQueryPlan(string expressionStr, IQueryExpression queryExpression, bool shallow,
                                      IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : this(expressionStr, queryExpression, null, shallow, enabledFilters, factory)
        {
        }

        protected internal HQLExpressionQueryPlan(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow,
                                                  IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
            : base(expressionStr, CreateTranslators(expressionStr, queryExpression, collectionRole, shallow, enabledFilters, factory))
        {
            QueryExpression = queryExpression;
        }

        private static IQueryTranslator[] CreateTranslators(string expressionStr, IQueryExpression queryExpression, string collectionRole, bool shallow, IDictionary<string, IFilter> enabledFilters, ISessionFactoryImplementor factory)
        {
            IQueryTranslatorFactory2 qtFactory = new ASTQueryTranslatorFactory();

            return qtFactory.CreateQueryTranslators(expressionStr, queryExpression, collectionRole, shallow, enabledFilters, factory);
        }
    }
}