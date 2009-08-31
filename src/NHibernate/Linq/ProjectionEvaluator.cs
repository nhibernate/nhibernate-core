using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    public class ProjectionEvaluator : ExpressionTreeVisitor
    {
        protected readonly HqlTreeBuilder _hqlTreeBuilder;
        protected readonly HqlNodeStack _stack;
        private readonly ParameterAggregator _parameterAggregator;
        private HashSet<Expression> _hqlNodes;
        private readonly ParameterExpression _objectArray;
        private Expression _projectionExpression;
        private int _iColumn;

        public ProjectionEvaluator(ParameterAggregator parameterAggregator, ParameterExpression objectArray)
        {
            _parameterAggregator = parameterAggregator;
            _objectArray = objectArray;
            _hqlTreeBuilder = new HqlTreeBuilder();
            _stack = new HqlNodeStack(_hqlTreeBuilder);
        }

        public Expression ProjectionExpression
        {
            get { return _projectionExpression; }
        }

        public IEnumerable<HqlTreeNode> GetAstBuilderNode()
        {
            return _stack.Finish();
        }

        public void Visit(Expression expression)
        {
            // First, find the sub trees that can be expressed purely in HQL
            _hqlNodes = new Nominator(CanBeEvaluatedInHql).Nominate(expression);

            // Now visit the tree
            Expression projection = VisitExpression(expression);

            if ((projection != expression) && !_hqlNodes.Contains(expression))
            {
                _projectionExpression = projection;
            }
        }

        protected override Expression VisitExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (_hqlNodes.Contains(expression))
            {
                // Pure HQL evaluation
                var hqlVisitor = new NhExpressionTreeVisitor(_parameterAggregator);
                hqlVisitor.Visit(expression);
                hqlVisitor.GetAstBuilderNode().ForEach(n =>_stack.PushAndPop(n) );

                return Expression.Convert(Expression.ArrayIndex(_objectArray, Expression.Constant(_iColumn++)),
                                          expression.Type);
            }

            // Can't handle this node with HQL.  Just recurse down, and emit the expression
            return base.VisitExpression(expression);
        }

        private static bool CanBeEvaluatedInHql(Expression expression)
        {
            return (expression.NodeType != ExpressionType.MemberInit) && (expression.NodeType != ExpressionType.New);
        }
    }
}