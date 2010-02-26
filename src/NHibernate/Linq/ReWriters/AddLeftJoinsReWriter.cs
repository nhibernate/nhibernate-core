using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;

namespace NHibernate.Linq.Visitors
{
    public class AddLeftJoinsReWriter : QueryModelVisitorBase
    {
        private readonly ISessionFactory _sessionFactory;

        private AddLeftJoinsReWriter(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public static void ReWrite(QueryModel queryModel, ISessionFactory sessionFactory)
        {
            var rewriter = new AddLeftJoinsReWriter(sessionFactory);

            rewriter.VisitQueryModel(queryModel);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            var joins = LeftJoinDetector.Detect(selectClause.Selector, new NameGenerator(queryModel), _sessionFactory);

            if (joins.Joins.Count > 0)
            {
                selectClause.Selector = joins.Selector;

                queryModel.TransformExpressions(e => ExpressionSwapper.Swap(e, joins.ExpressionMap));

                foreach (var join in joins.Joins)
                {
                    queryModel.BodyClauses.Add(join);
                }
            }
        }
    }

    public class ExpressionSwapper : NhExpressionTreeVisitor
    {
        private readonly Dictionary<Expression, Expression> _expressionMap;

        private ExpressionSwapper(Dictionary<Expression, Expression> expressionMap)
        {
            _expressionMap = expressionMap;
        }

        public static Expression Swap(Expression expression, Dictionary<Expression, Expression> expressionMap)
        {
            var swapper = new ExpressionSwapper(expressionMap);

            return swapper.VisitExpression(expression);
        }

        public override Expression VisitExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            Expression replacement;

            if (_expressionMap.TryGetValue(expression, out replacement))
            {
                return replacement;
            }
            else
            {
                return base.VisitExpression(expression);
            }
        }
    }
}