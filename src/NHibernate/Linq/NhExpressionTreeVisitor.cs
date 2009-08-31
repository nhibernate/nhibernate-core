using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Hql.Ast;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.Parsing;

namespace NHibernate.Linq
{
    public class NhExpressionTreeVisitor : ThrowingExpressionTreeVisitor
    {
        protected readonly HqlTreeBuilder _hqlTreeBuilder;
        protected readonly HqlNodeStack _stack;
        private readonly ParameterAggregator _parameterAggregator;

        public NhExpressionTreeVisitor(ParameterAggregator parameterAggregator)
        {
            _parameterAggregator = parameterAggregator;
            _hqlTreeBuilder = new HqlTreeBuilder();
            _stack = new HqlNodeStack(_hqlTreeBuilder);
        }

        public IEnumerable<HqlTreeNode> GetAstBuilderNode()
        {
            return _stack.Finish();
        }

        public virtual void Visit(Expression expression)
        {
            VisitExpression(expression);
        }

        protected override Expression VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            _stack.PushAndPop(_hqlTreeBuilder.Ident(expression.ReferencedQuerySource.ItemName));

            return expression;
        }

        protected override Expression VisitBinaryExpression(BinaryExpression expression)
        {
            HqlTreeNode operatorNode = GetHqlOperatorNodeForBinaryOperator(expression);

            using (_stack.Push(operatorNode))
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

            using (_stack.Push(operatorNode))
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
            using (_stack.Push(_hqlTreeBuilder.Dot()))
            {
                Expression newExpression = VisitExpression(expression.Expression);

                _stack.PushAndPop(_hqlTreeBuilder.Ident(expression.Member.Name));

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
                    _stack.PushAndPop(_hqlTreeBuilder.Ident(t.GetGenericArguments()[0].Name));
                    return expression;
                }
            }

            /*
            var namedParameter = _parameterAggregator.AddParameter(expression.Value);

            _expression = _hqlTreeBuilder.Parameter(namedParameter.Name);

            return expression;
             */
            // TODO - get parameter support in place in the HQLQueryPlan
            _stack.PushAndPop(_hqlTreeBuilder.Constant(expression.Value));

            return expression;
        }

        protected override Expression VisitMethodCallExpression(MethodCallExpression expression)
        {
            if (expression.Method.DeclaringType == typeof(Enumerable))
            {
                switch (expression.Method.Name)
                {
                    case "Any":
                        // Any has one or two arguments.  Arg 1 is the source and arg 2 is the optional predicate
                        using (_stack.Push(_hqlTreeBuilder.Exists()))
                        {
                            using (_stack.Push(_hqlTreeBuilder.Query()))
                            {
                                using (_stack.Push(_hqlTreeBuilder.SelectFrom()))
                                {
                                    using (_stack.Push(_hqlTreeBuilder.From()))
                                    {
                                        using (_stack.Push(_hqlTreeBuilder.Range()))
                                        {
                                            VisitExpression(expression.Arguments[0]);

                                            if (expression.Arguments.Count > 1)
                                            {
                                                var expr = (LambdaExpression) expression.Arguments[1];
                                                _stack.PushAndPop(_hqlTreeBuilder.Alias(expr.Parameters[0].Name));
                                            }
                                        }
                                    }
                                }
                                if (expression.Arguments.Count > 1)
                                {
                                    using (_stack.Push(_hqlTreeBuilder.Where()))
                                    {
                                        VisitExpression(expression.Arguments[1]);
                                    }
                                }
                            }
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
            _stack.PushAndPop(_hqlTreeBuilder.Ident(expression.Name));

            return expression;
        }

        protected override Expression VisitConditionalExpression(ConditionalExpression expression)
        {
            using (_stack.Push(_hqlTreeBuilder.Case()))
            {
                using (_stack.Push(_hqlTreeBuilder.When()))
                {
                    VisitExpression(expression.Test);

                    VisitExpression(expression.IfTrue);
                }

                if (expression.IfFalse != null)
                {
                    using (_stack.Push(_hqlTreeBuilder.Else()))
                    {
                        VisitExpression(expression.IfFalse);
                    }
                }
            }

            return expression;
        }

        protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
        {
            CommandData query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameterAggregator);

            if (query.ProjectionExpression != null)
            {
                throw new InvalidOperationException();
            }

            // TODO - what if there was a projection expression?

            _stack.PushAndPop(query.Statement);
            
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