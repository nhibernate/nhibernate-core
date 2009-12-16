using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq.Visitors
{
    public class SelectClauseVisitor : ExpressionTreeVisitor
    {
        private HashSet<Expression> _hqlNodes;
        private readonly ParameterExpression _inputParameter;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;
    	private int _iColumn;
        private List<HqlExpression> _hqlTreeNodes = new List<HqlExpression>();

        public SelectClauseVisitor(System.Type inputType, IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
        {
            _inputParameter = Expression.Parameter(inputType, "input");
        	_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
        }

        public LambdaExpression ProjectionExpression { get; private set; }

        public IEnumerable<HqlExpression> GetHqlNodes()
        {
            return _hqlTreeNodes;
        }

        public void Visit(Expression expression)
        {
            // First, find the sub trees that can be expressed purely in HQL
            _hqlNodes = new Nominator(CanBeEvaluatedInHqlSelectStatement, CanBeEvaluatedInHqlStatementShortcut).Nominate(expression);

            // Now visit the tree
            Expression projection = VisitExpression(expression);

            if ((projection != expression) && !_hqlNodes.Contains(expression))
            {
                ProjectionExpression = Expression.Lambda(projection, _inputParameter);
            }

            // Finally, handle any boolean results in the output nodes
            _hqlTreeNodes = BooleanToCaseConvertor.Convert(_hqlTreeNodes).ToList();
        }

        protected override Expression VisitExpression(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (_hqlNodes.Contains(expression))
            {
                // Pure HQL evaluation - TODO - cache the Visitor?
				var hqlVisitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
                
                _hqlTreeNodes.Add(hqlVisitor.Visit(expression).AsExpression());

                return Expression.Convert(Expression.ArrayIndex(_inputParameter, Expression.Constant(_iColumn++)),
                                          expression.Type);
            }

            // Can't handle this node with HQL.  Just recurse down, and emit the expression
            return base.VisitExpression(expression);
        }

        private static bool CanBeEvaluatedInHqlSelectStatement(Expression expression)
        {
            return (expression.NodeType != ExpressionType.MemberInit) && (expression.NodeType != ExpressionType.New);
        }

        private static bool CanBeEvaluatedInHqlStatementShortcut(Expression expression)
        {
            return ((NhExpressionType) expression.NodeType) == NhExpressionType.Count;
        }
    }

    public static class BooleanToCaseConvertor
    {
        public static IEnumerable<HqlExpression> Convert(IEnumerable<HqlExpression> hqlTreeNodes)
        {
            return hqlTreeNodes.Select(node => ConvertBooleanToCase(node));
        }

        private static HqlExpression ConvertBooleanToCase(HqlExpression node)
        {
            if (node is HqlBooleanExpression)
            {
                var builder = new HqlTreeBuilder();

                return builder.Case(
                    new HqlWhen[] {builder.When(node, builder.True())},
                    builder.False());
            }

            return node;
        }
    }
}