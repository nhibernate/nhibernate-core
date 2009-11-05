using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.ReWriters;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;

namespace NHibernate.Linq.Visitors
{
    public class HqlGeneratorExpressionTreeVisitor : NhThrowingExpressionTreeVisitor
    {
        protected readonly HqlTreeBuilder _hqlTreeBuilder;
        protected readonly HqlNodeStack _stack;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;

    	public HqlGeneratorExpressionTreeVisitor(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
        {
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
            _stack = new HqlNodeStack(_hqlTreeBuilder);
        }

        public IEnumerable<HqlTreeNode> GetHqlTreeNodes()
        {
            return _stack.Finish();
        }

        public virtual void Visit(Expression expression)
        {
            VisitExpression(expression);
        }

        protected override Expression VisitNhAverage(NhAverageExpression expression)
        {
            var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhCount(NhCountExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhMin(NhMinExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Min(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhMax(NhMaxExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Max(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhSum(NhSumExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(visitor.GetHqlTreeNodes().Single()), expression.Type));

            return expression;
        }

        protected override Expression VisitNhDistinct(NhDistinctExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);
            visitor.Visit(expression.Expression);

            _stack.PushLeaf(_hqlTreeBuilder.Distinct());

            foreach (var node in visitor.GetHqlTreeNodes())
            {
                _stack.PushLeaf(node);
            }

            return expression;
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.ReferencedQuerySource.ItemName));

            return expression;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            HqlTreeNode operatorNode = GetHqlOperatorNodeForBinaryOperator(expression);

            using (_stack.PushNode(operatorNode))
            {
                VisitExpression(expression.Left);

                VisitExpression(expression.Right);
            }

            return expression;
        }

        private HqlTreeNode GetHqlOperatorNodeForBinaryOperator(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    return _hqlTreeBuilder.Equality();

                case ExpressionType.NotEqual:
                    return _hqlTreeBuilder.Inequality();

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return _hqlTreeBuilder.BooleanAnd();

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return _hqlTreeBuilder.BooleanOr();

                case ExpressionType.Add:
                    return _hqlTreeBuilder.Add();

                case ExpressionType.Subtract:
                    return _hqlTreeBuilder.Subtract();

                case ExpressionType.Multiply:
                    return _hqlTreeBuilder.Multiply();

                case ExpressionType.Divide:
                    return _hqlTreeBuilder.Divide();

                case ExpressionType.LessThan:
                    return _hqlTreeBuilder.LessThan();

                case ExpressionType.LessThanOrEqual:
                    return _hqlTreeBuilder.LessThanOrEqual();

                case ExpressionType.GreaterThan:
                    return _hqlTreeBuilder.GreaterThan();

                case ExpressionType.GreaterThanOrEqual:
                    return _hqlTreeBuilder.GreaterThanOrEqual();
            }

            throw new InvalidOperationException();
        }

        protected override Expression VisitUnaryExpression(UnaryExpression expression)
        {
            HqlTreeNode operatorNode = GetHqlOperatorNodeforUnaryOperator(expression);

            using (_stack.PushNode(operatorNode))
            {
                VisitExpression(expression.Operand);
            }

            return expression;
        }

        private HqlTreeNode GetHqlOperatorNodeforUnaryOperator(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    return _hqlTreeBuilder.Not();
            }
            
            throw new InvalidOperationException();
        }

        protected override Expression VisitMemberExpression(MemberExpression expression)
        {
            using (_stack.PushNode(_hqlTreeBuilder.Dot()))
            {
                Expression newExpression = VisitExpression(expression.Expression);

                _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.Member.Name));

                if (newExpression != expression.Expression)
                {
                    return Expression.MakeMemberAccess(newExpression, expression.Member);
                }
            }

            return expression;
        }

        protected override Expression VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Value != null)
            {
                System.Type t = expression.Value.GetType();

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (NhQueryable<>))
                {
                    _stack.PushLeaf(_hqlTreeBuilder.Ident(t.GetGenericArguments()[0].Name));
                    return expression;
                }
            }

        	NamedParameter namedParameter;

			if (_parameters.TryGetValue(expression, out namedParameter))
			{
				_stack.PushLeaf(_hqlTreeBuilder.Cast(_hqlTreeBuilder.Parameter(namedParameter.Name), namedParameter.Value.GetType()));
				_requiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, new []{ _requiredHqlParameters.Count + 1}, false));
			}
			else
			{
				_stack.PushLeaf(_hqlTreeBuilder.Constant(expression.Value));
			}

            return expression;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType == typeof(Enumerable) ||
                expression.Method.DeclaringType == typeof(Queryable))
            {
                switch (expression.Method.Name)
                {
                    case "Any":
                        // Any has one or two arguments.  Arg 1 is the source and arg 2 is the optional predicate
                        using (_stack.PushNode(_hqlTreeBuilder.Exists()))
                        {
							using (_stack.PushNode(_hqlTreeBuilder.Query()))
							{
								using (_stack.PushNode(_hqlTreeBuilder.SelectFrom()))
								{
									using (_stack.PushNode(_hqlTreeBuilder.From()))
									{
										using (_stack.PushNode(_hqlTreeBuilder.Range()))
										{
											VisitExpression(expression.Arguments[0]);

											if (expression.Arguments.Count > 1)
											{
												var expr = (LambdaExpression) expression.Arguments[1];
												_stack.PushLeaf(_hqlTreeBuilder.Alias(expr.Parameters[0].Name));
											}
										}
									}
								}
								if (expression.Arguments.Count > 1)
								{
									using (_stack.PushNode(_hqlTreeBuilder.Where()))
									{
										VisitExpression(expression.Arguments[1]);
									}
								}
							}
                        }
                        break;

					case "All":
						// All has one or two arguments.  Arg 1 is the source and arg 2 is the optional predicate
						using (_stack.PushNode(_hqlTreeBuilder.Not()))
						{
							using (_stack.PushNode(_hqlTreeBuilder.Exists()))
							{
								using (_stack.PushNode(_hqlTreeBuilder.Query()))
								{
									using (_stack.PushNode(_hqlTreeBuilder.SelectFrom()))
									{
										using (_stack.PushNode(_hqlTreeBuilder.From()))
										{
											using (_stack.PushNode(_hqlTreeBuilder.Range()))
											{
												VisitExpression(expression.Arguments[0]);

												if (expression.Arguments.Count > 1)
												{
													var expr = (LambdaExpression) expression.Arguments[1];

													_stack.PushLeaf(_hqlTreeBuilder.Alias(expr.Parameters[0].Name));
												}
											}
										}
									}
									if (expression.Arguments.Count > 1)
									{
										using (_stack.PushNode(_hqlTreeBuilder.Where()))
										{
											using (_stack.PushNode(_hqlTreeBuilder.Not()))
											{
												VisitExpression(expression.Arguments[1]);
											}
										}
									}
								}
							}
						}
                		break;

					case "Min": 
                        using (_stack.PushNode(_hqlTreeBuilder.Min()))
                        {
                            VisitExpression(expression.Arguments[1]);
                        }
                        break;
                    case "Max":
                        using (_stack.PushNode(_hqlTreeBuilder.Max()))
                        {
                            VisitExpression(expression.Arguments[1]);
                        }
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The Enumerable method {0} is not supported", expression.Method.Name));
                }

                return expression;
            }
            else
            {
                return base.VisitMethodCallExpression(expression); // throws
            }
        }

        protected override Expression VisitLambdaExpression(LambdaExpression expression)
        {
            VisitExpression(expression.Body);

            return expression;
        }

        protected override Expression VisitParameterExpression(ParameterExpression expression)
        {
            _stack.PushLeaf(_hqlTreeBuilder.Ident(expression.Name));

            return expression;
        }

        protected override Expression VisitConditionalExpression(ConditionalExpression expression)
        {
            using (_stack.PushNode(_hqlTreeBuilder.Case()))
            {
                using (_stack.PushNode(_hqlTreeBuilder.When()))
                {
                    VisitExpression(expression.Test);

                    VisitExpression(expression.IfTrue);
                }

                if (expression.IfFalse != null)
                {
                    using (_stack.PushNode(_hqlTreeBuilder.Else()))
                    {
                        VisitExpression(expression.IfFalse);
                    }
                }
            }

            return expression;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            CommandData query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, _requiredHqlParameters);

            _stack.PushLeaf(query.Statement);
            
            return expression;
        }


        // Called when a LINQ expression type is not handled above.
        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            string itemText = FormatUnhandledItem(unhandledItem);
            var message = string.Format("The expression '{0}' (type: {1}) is not supported by this LINQ provider.", itemText, typeof(T));
            return new NotSupportedException(message);
        }

        private string FormatUnhandledItem<T>(T unhandledItem)
        {
            var itemAsExpression = unhandledItem as Expression;
            return itemAsExpression != null ? FormattingExpressionTreeVisitor.Format(itemAsExpression) : unhandledItem.ToString();
        }
    }
}