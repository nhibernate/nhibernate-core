using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using Remotion.Data.Linq.Clauses.Expressions;

namespace NHibernate.Linq.Visitors
{
    public class HqlGeneratorExpressionTreeVisitor : IHqlExpressionVisitor
    {
        private readonly HqlTreeBuilder _hqlTreeBuilder;
    	private readonly IDictionary<ConstantExpression, NamedParameter> _parameters;
    	private readonly IList<NamedParameterDescriptor> _requiredHqlParameters;
        static private readonly FunctionRegistry FunctionRegistry = FunctionRegistry.Initialise();

        public HqlGeneratorExpressionTreeVisitor(IDictionary<ConstantExpression, NamedParameter> parameters, IList<NamedParameterDescriptor> requiredHqlParameters)
        {
			_parameters = parameters;
			_requiredHqlParameters = requiredHqlParameters;
			_hqlTreeBuilder = new HqlTreeBuilder();
        }

        public virtual HqlTreeNode Visit(Expression expression)
        {
            return VisitExpression(expression);
        }

        public HqlTreeBuilder TreeBuilder
        {
            get { return _hqlTreeBuilder; }
        }

        protected HqlTreeNode VisitExpression(Expression expression)
        {
            if (expression == null)
                return null;

            switch (expression.NodeType)
            {
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return VisitUnaryExpression((UnaryExpression)expression);
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Power:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    return VisitBinaryExpression((BinaryExpression)expression);
                case ExpressionType.Conditional:
                    return VisitConditionalExpression((ConditionalExpression)expression);
                case ExpressionType.Constant:
                    return VisitConstantExpression((ConstantExpression)expression);
                case ExpressionType.Invoke:
                    return VisitInvocationExpression((InvocationExpression)expression);
                case ExpressionType.Lambda:
                    return VisitLambdaExpression((LambdaExpression)expression);
                case ExpressionType.MemberAccess:
                    return VisitMemberExpression((MemberExpression)expression);
                case ExpressionType.Call:
                    return VisitMethodCallExpression((MethodCallExpression)expression);
                //case ExpressionType.New:
                //    return VisitNewExpression((NewExpression)expression);
                case ExpressionType.NewArrayBounds:
                //case ExpressionType.NewArrayInit:
                //    return VisitNewArrayExpression((NewArrayExpression)expression);
                //case ExpressionType.MemberInit:
                //    return VisitMemberInitExpression((MemberInitExpression)expression);
                //case ExpressionType.ListInit:
                //    return VisitListInitExpression((ListInitExpression)expression);
                case ExpressionType.Parameter:
                    return VisitParameterExpression((ParameterExpression)expression);
                //case ExpressionType.TypeIs:
                //    return VisitTypeBinaryExpression((TypeBinaryExpression)expression);

                default:
                    if (expression is SubQueryExpression)
                        return VisitSubQueryExpression((SubQueryExpression)expression);

                    if (expression is QuerySourceReferenceExpression)
                        return VisitQuerySourceReferenceExpression((QuerySourceReferenceExpression)expression);

                    switch ((NhExpressionType)expression.NodeType)
                    {
                        case NhExpressionType.Average:
                            return VisitNhAverage((NhAverageExpression)expression);
                        case NhExpressionType.Min:
                            return VisitNhMin((NhMinExpression)expression);
                        case NhExpressionType.Max:
                            return VisitNhMax((NhMaxExpression)expression);
                        case NhExpressionType.Sum:
                            return VisitNhSum((NhSumExpression)expression);
                        case NhExpressionType.Count:
                            return VisitNhCount((NhCountExpression)expression);
                        case NhExpressionType.Distinct:
                            return VisitNhDistinct((NhDistinctExpression)expression);
                        //case NhExpressionType.New:
                        //    return VisitNhNew((NhNewExpression)expression);
                    }

                    throw new NotSupportedException(expression.GetType().Name);
            }
        }

        private HqlTreeNode VisitInvocationExpression(InvocationExpression expression)
        {
            return VisitExpression(expression.Expression);
        }


        protected HqlTreeNode VisitNhAverage(NhAverageExpression expression)
        {
            return _hqlTreeBuilder.Cast(
                _hqlTreeBuilder.Average(VisitExpression(expression.Expression).AsExpression()),
                expression.Type);
        }

        protected HqlTreeNode VisitNhCount(NhCountExpression expression)
        {
            return _hqlTreeBuilder.Cast(
                _hqlTreeBuilder.Count(VisitExpression(expression.Expression).AsExpression()),
                expression.Type);
        }

        protected HqlTreeNode VisitNhMin(NhMinExpression expression)
        {
            return _hqlTreeBuilder.Cast(
                _hqlTreeBuilder.Min(VisitExpression(expression.Expression).AsExpression()), 
                expression.Type);
        }

        protected HqlTreeNode VisitNhMax(NhMaxExpression expression)
        {
            return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Max(VisitExpression(expression.Expression).AsExpression()), expression.Type);
        }

        protected HqlTreeNode VisitNhSum(NhSumExpression expression)
        {
            return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(VisitExpression(expression.Expression).AsExpression()), expression.Type);
        }

        protected HqlTreeNode VisitNhDistinct(NhDistinctExpression expression)
        {
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters, _requiredHqlParameters);

            return _hqlTreeBuilder.DistinctHolder(
                _hqlTreeBuilder.Distinct(),
                visitor.VisitExpression(expression.Expression));
        }

        protected HqlTreeNode VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
        {
            return _hqlTreeBuilder.Ident(expression.ReferencedQuerySource.ItemName);
        }

        protected HqlTreeNode VisitBinaryExpression(BinaryExpression expression)
        {
            var lhs = VisitExpression(expression.Left).AsExpression();
            var rhs = VisitExpression(expression.Right).AsExpression();

            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                    // Need to check for boolean equality
                    if (lhs is HqlBooleanExpression || rhs is HqlBooleanExpression)
                    {
                        lhs =
                            _hqlTreeBuilder.Case(
                                new [] { _hqlTreeBuilder.When(lhs, _hqlTreeBuilder.Constant(1)) },
                                _hqlTreeBuilder.Constant(0));

                        rhs =
                            _hqlTreeBuilder.Case(
                                new [] { _hqlTreeBuilder.When(rhs, _hqlTreeBuilder.Constant(1)) },
                                _hqlTreeBuilder.Constant(0));

                        return _hqlTreeBuilder.Equality(lhs, rhs);
                    }

                    // Also check for nullability
                    if (expression.Left.Type.IsNullable() || expression.Right.Type.IsNullable())
                    {
                        // TODO - yuck.  This clone is needed because the AST tree nodes are not immutable,
                        // and sharing nodes between multiple branches will cause issues in the hqlSqlWalker phase -
                        // a node, x, gets visited during the walk and updated to refer to a real property.  Later in
                        // the walk, x get revisited (since we copied it here), but now the type doesn't match what
                        // the parser expects.  So we can't share.  Implementing Clone() on HqlTreeNode would be better
                        // that doing a full visit of the Expression tree.  Allowing shared nodes in the AST would be better
                        // still, but might be more work
                        var lhs2 = VisitExpression(expression.Left).AsExpression();
                        var rhs2 = VisitExpression(expression.Right).AsExpression();

                        return _hqlTreeBuilder.BooleanOr(
                                _hqlTreeBuilder.BooleanAnd(
                                    _hqlTreeBuilder.IsNull(lhs),
                                    _hqlTreeBuilder.IsNull(rhs)),
                                _hqlTreeBuilder.Equality(lhs2, rhs2)
                                );
                    }

                    return _hqlTreeBuilder.Equality(lhs, rhs);

                case ExpressionType.NotEqual:
                    // Need to check for boolean in-equality
                    if (lhs is HqlBooleanExpression || rhs is HqlBooleanExpression)
                    {
                        lhs =
                            _hqlTreeBuilder.Case(
                                new [] { _hqlTreeBuilder.When(lhs, _hqlTreeBuilder.Constant(1)) },
                                _hqlTreeBuilder.Constant(0));

                        rhs =
                            _hqlTreeBuilder.Case(
                                new [] { _hqlTreeBuilder.When(rhs, _hqlTreeBuilder.Constant(1)) },
                                _hqlTreeBuilder.Constant(0));

                        return _hqlTreeBuilder.Inequality(lhs, rhs);

                    }

                    // Also check for nullability
                    if (expression.Left.Type.IsNullable() || expression.Right.Type.IsNullable())
                    {
                        var lhs2 = VisitExpression(expression.Left).AsExpression();
                        var rhs2 = VisitExpression(expression.Right).AsExpression();
                        var lhs3 = VisitExpression(expression.Left).AsExpression();
                        var rhs3 = VisitExpression(expression.Right).AsExpression();

                        return
                            _hqlTreeBuilder.BooleanOr(
                                _hqlTreeBuilder.BooleanOr(
                                    _hqlTreeBuilder.BooleanAnd(
                                        _hqlTreeBuilder.IsNull(lhs),
                                        _hqlTreeBuilder.IsNotNull(rhs)),
                                    _hqlTreeBuilder.BooleanAnd(
                                        _hqlTreeBuilder.IsNotNull(lhs2),
                                        _hqlTreeBuilder.IsNull(rhs2))
                                    ),
                                _hqlTreeBuilder.Inequality(lhs3, rhs3));
                    }

                    return _hqlTreeBuilder.Inequality(lhs, rhs);

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return _hqlTreeBuilder.BooleanAnd(lhs.AsBooleanExpression(), rhs.AsBooleanExpression());

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return _hqlTreeBuilder.BooleanOr(lhs.AsBooleanExpression(), rhs.AsBooleanExpression());

                case ExpressionType.Add:
                    return _hqlTreeBuilder.Add(lhs, rhs);

                case ExpressionType.Subtract:
                    return _hqlTreeBuilder.Subtract(lhs, rhs);

                case ExpressionType.Multiply:
                    return _hqlTreeBuilder.Multiply(lhs, rhs);

                case ExpressionType.Divide:
                    return _hqlTreeBuilder.Divide(lhs, rhs);

                case ExpressionType.LessThan:
                    return _hqlTreeBuilder.LessThan(lhs, rhs);

                case ExpressionType.LessThanOrEqual:
                    return _hqlTreeBuilder.LessThanOrEqual(lhs, rhs);

                case ExpressionType.GreaterThan:
                    return _hqlTreeBuilder.GreaterThan(lhs, rhs);

                case ExpressionType.GreaterThanOrEqual:
                    return _hqlTreeBuilder.GreaterThanOrEqual(lhs, rhs);
            }

            throw new InvalidOperationException();
        }

        protected HqlTreeNode VisitUnaryExpression(UnaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Not:
                    return _hqlTreeBuilder.Not(VisitExpression(expression.Operand).AsBooleanExpression());
                case ExpressionType.Convert:
                    return VisitExpression(expression.Operand);
            }

            throw new NotSupportedException(expression.ToString());
        }

        protected HqlTreeNode VisitMemberExpression(MemberExpression expression)
        {
            // Strip out the .Value property of a nullable type, HQL doesn't need that
            if (expression.Member.Name == "Value" && expression.Expression.Type.IsNullable())
            {
                return VisitExpression(expression.Expression);
            }

            // Look for "special" properties (DateTime.Month etc)
            var generator = FunctionRegistry.GetPropertyGenerator(expression.Member);

            if (generator != null)
            {
                return generator.BuildHql(expression.Member, expression.Expression, _hqlTreeBuilder, this);
            }

            // Else just emit standard HQL for a property reference
            return _hqlTreeBuilder.Dot(VisitExpression(expression.Expression).AsExpression(), _hqlTreeBuilder.Ident(expression.Member.Name));
        }

        protected HqlTreeNode VisitConstantExpression(ConstantExpression expression)
        {
            if (expression.Value != null)
            {
                System.Type t = expression.Value.GetType();

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (NhQueryable<>))
                {
                    return _hqlTreeBuilder.Ident(t.GetGenericArguments()[0].FullName);
                }
            }

        	NamedParameter namedParameter;

			if (_parameters.TryGetValue(expression, out namedParameter))
			{
                _requiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, new[] { _requiredHqlParameters.Count + 1 }, false));
                
                if (namedParameter.Value is bool)
                {
                    return _hqlTreeBuilder.Equality(_hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression(),
                                                    _hqlTreeBuilder.Constant(1));
                }

                return _hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression();
			}
            
            return _hqlTreeBuilder.Constant(expression.Value);
        }

        protected HqlTreeNode VisitMethodCallExpression(MethodCallExpression expression)
        {
            var generator = FunctionRegistry.GetMethodGenerator(expression.Method);

            return generator.BuildHql(expression.Method, expression.Object, expression.Arguments, _hqlTreeBuilder, this);
        }

        protected HqlTreeNode VisitLambdaExpression(LambdaExpression expression)
        {
            return VisitExpression(expression.Body);
        }

        protected HqlTreeNode VisitParameterExpression(ParameterExpression expression)
        {
            return _hqlTreeBuilder.Ident(expression.Name);
        }

        protected HqlTreeNode VisitConditionalExpression(ConditionalExpression expression)
        {
            var when = _hqlTreeBuilder.When(VisitExpression(expression.Test).AsExpression(), VisitExpression(expression.IfTrue).AsExpression());
            HqlExpression ifFalse = null;

            if (expression.IfFalse != null)
            {
                ifFalse = VisitExpression(expression.IfFalse).AsExpression();
            }

            return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Case(new []{when}, ifFalse), expression.Type);
        }

        protected HqlTreeNode VisitSubQueryExpression(SubQueryExpression expression)
        {
            ExpressionToHqlTranslationResults query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, _requiredHqlParameters);

            return query.Statement;
        }
    }
}