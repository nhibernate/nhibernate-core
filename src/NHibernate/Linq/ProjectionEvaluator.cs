using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    public class ProjectionEvaluator : ExpressionTreeVisitor
    {
        protected readonly HqlTreeBuilder _hqlTreeBuilder;
        protected readonly HqlNodeStack _stack;
        private HashSet<Expression> _hqlNodes;
        private readonly ParameterExpression _inputParameter;
        private readonly Func<Expression, bool> _predicate;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;
    	private int _iColumn;

		public ProjectionEvaluator(System.Type inputType, Func<Expression, bool> predicate, IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
        {
            _inputParameter = Expression.Parameter(inputType, "input");
            _predicate = predicate;
        	_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
            _stack = new HqlNodeStack(_hqlTreeBuilder);
        }

        public LambdaExpression ProjectionExpression { get; private set; }

        public IEnumerable<HqlTreeNode> GetAstBuilderNode()
        {
            return _stack.Finish();
        }

        public void Visit(Expression expression)
        {
            // First, find the sub trees that can be expressed purely in HQL
            _hqlNodes = new Nominator(_predicate).Nominate(expression);

            // Now visit the tree
            Expression projection = VisitExpression(expression);

            if ((projection != expression) && !_hqlNodes.Contains(expression))
            {
                ProjectionExpression = Expression.Lambda(projection, _inputParameter);
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
				var hqlVisitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
                hqlVisitor.Visit(expression);
                hqlVisitor.GetHqlTreeNodes().ForEach(n =>_stack.PushLeaf(n) );

                return Expression.Convert(Expression.ArrayIndex(_inputParameter, Expression.Constant(_iColumn++)),
                                          expression.Type);
            }

            // Can't handle this node with HQL.  Just recurse down, and emit the expression
            return base.VisitExpression(expression);
        }
    }
}