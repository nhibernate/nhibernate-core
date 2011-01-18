using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses;

namespace NHibernate.Linq.ReWriters
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
			selectClause.Selector = JoinReplacer(queryModel, selectClause.Selector);
        }

		public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
		{
			foreach (Ordering ordering in orderByClause.Orderings)
			{
				ordering.Expression = JoinReplacer(queryModel, ordering.Expression);
			}
		}

		private Expression JoinReplacer(QueryModel queryModel, Expression expression)
		{
			var joins = LeftJoinDetector.Detect(expression, new NameGenerator(queryModel), _sessionFactory);

			Expression result = expression;

			if (joins.Joins.Count > 0)
			{
				result = joins.Selector;

				queryModel.TransformExpressions(e => ExpressionSwapper.Swap(e, joins.ExpressionMap));

				foreach (var join in joins.Joins)
				{
					queryModel.BodyClauses.Add(join);
				}
			}

			return result;
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