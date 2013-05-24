using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Param;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors
{
	public class HqlGeneratorExpressionTreeVisitor : IHqlExpressionVisitor
	{
		private readonly HqlTreeBuilder _hqlTreeBuilder = new HqlTreeBuilder();
		private readonly VisitorParameters _parameters;
		private readonly ILinqToHqlGeneratorsRegistry _functionRegistry;

		public static HqlTreeNode Visit(Expression expression, VisitorParameters parameters)
		{
			return new HqlGeneratorExpressionTreeVisitor(parameters).VisitExpression(expression);
		}

		public HqlGeneratorExpressionTreeVisitor(VisitorParameters parameters)
		{
			_functionRegistry = parameters.SessionFactory.Settings.LinqToHqlGeneratorsRegistry;
			_parameters = parameters;
		}


		public ISessionFactory SessionFactory { get { return _parameters.SessionFactory; } }


		public HqlTreeNode Visit(Expression expression)
		{
			return VisitExpression(expression);
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
					return VisitUnaryExpression((UnaryExpression) expression);
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
					return VisitBinaryExpression((BinaryExpression) expression);
				case ExpressionType.Conditional:
					return VisitConditionalExpression((ConditionalExpression) expression);
				case ExpressionType.Constant:
					return VisitConstantExpression((ConstantExpression) expression);
				case ExpressionType.Invoke:
					return VisitInvocationExpression((InvocationExpression) expression);
				case ExpressionType.Lambda:
					return VisitLambdaExpression((LambdaExpression) expression);
				case ExpressionType.MemberAccess:
					return VisitMemberExpression((MemberExpression) expression);
				case ExpressionType.Call:
					return VisitMethodCallExpression((MethodCallExpression) expression);
					//case ExpressionType.New:
					//    return VisitNewExpression((NewExpression)expression);
					//case ExpressionType.NewArrayBounds:
				case ExpressionType.NewArrayInit:
					return VisitNewArrayExpression((NewArrayExpression) expression);
					//case ExpressionType.MemberInit:
					//    return VisitMemberInitExpression((MemberInitExpression)expression);
					//case ExpressionType.ListInit:
					//    return VisitListInitExpression((ListInitExpression)expression);
				case ExpressionType.Parameter:
					return VisitParameterExpression((ParameterExpression) expression);
				case ExpressionType.TypeIs:
					return VisitTypeBinaryExpression((TypeBinaryExpression) expression);

				default:
					var subQueryExpression = expression as SubQueryExpression;
					if (subQueryExpression != null)
						return VisitSubQueryExpression(subQueryExpression);

					var querySourceReferenceExpression = expression as QuerySourceReferenceExpression;
					if (querySourceReferenceExpression != null)
						return VisitQuerySourceReferenceExpression(querySourceReferenceExpression);

					var vbStringComparisonExpression = expression as VBStringComparisonExpression;
					if (vbStringComparisonExpression != null)
						return VisitVBStringComparisonExpression(vbStringComparisonExpression);

					switch ((NhExpressionType) expression.NodeType)
					{
						case NhExpressionType.Average:
							return VisitNhAverage((NhAverageExpression) expression);
						case NhExpressionType.Min:
							return VisitNhMin((NhMinExpression) expression);
						case NhExpressionType.Max:
							return VisitNhMax((NhMaxExpression) expression);
						case NhExpressionType.Sum:
							return VisitNhSum((NhSumExpression) expression);
						case NhExpressionType.Count:
							return VisitNhCount((NhCountExpression) expression);
						case NhExpressionType.Distinct:
							return VisitNhDistinct((NhDistinctExpression) expression);
						case NhExpressionType.Star:
							return VisitNhStar((NhStarExpression) expression);
							//case NhExpressionType.New:
							//    return VisitNhNew((NhNewExpression)expression);
					}

					throw new NotSupportedException(expression.ToString());
			}
		}

		private HqlTreeNode VisitTypeBinaryExpression(TypeBinaryExpression expression)
		{
			return _hqlTreeBuilder.Equality(
				_hqlTreeBuilder.Dot(Visit(expression.Expression).AsExpression(), _hqlTreeBuilder.Class()),
				_hqlTreeBuilder.Ident(expression.TypeOperand.FullName));
		}

		protected HqlTreeNode VisitNhStar(NhStarExpression expression)
		{
			return _hqlTreeBuilder.Star();
		}

		private HqlTreeNode VisitInvocationExpression(InvocationExpression expression)
		{
			return VisitExpression(expression.Expression);
		}

		protected HqlTreeNode VisitNhAverage(NhAverageExpression expression)
		{
			var hqlExpression = VisitExpression(expression.Expression).AsExpression();
			if (expression.Type != expression.Expression.Type)
				hqlExpression = _hqlTreeBuilder.Cast(hqlExpression, expression.Type);

			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Average(hqlExpression), expression.Type);
		}

		protected HqlTreeNode VisitNhCount(NhCountExpression expression)
		{
			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Count(VisitExpression(expression.Expression).AsExpression()), expression.Type);
		}

		protected HqlTreeNode VisitNhMin(NhMinExpression expression)
		{
			return _hqlTreeBuilder.Min(VisitExpression(expression.Expression).AsExpression());
		}

		protected HqlTreeNode VisitNhMax(NhMaxExpression expression)
		{
			return _hqlTreeBuilder.Max(VisitExpression(expression.Expression).AsExpression());
		}

		protected HqlTreeNode VisitNhSum(NhSumExpression expression)
		{
			return _hqlTreeBuilder.Cast(_hqlTreeBuilder.Sum(VisitExpression(expression.Expression).AsExpression()), expression.Type);
		}

		protected HqlTreeNode VisitNhDistinct(NhDistinctExpression expression)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters);
			return _hqlTreeBuilder.ExpressionSubTreeHolder(_hqlTreeBuilder.Distinct(), visitor.VisitExpression(expression.Expression));
		}

		protected HqlTreeNode VisitQuerySourceReferenceExpression(QuerySourceReferenceExpression expression)
		{
			return _hqlTreeBuilder.Ident(_parameters.QuerySourceNamer.GetName(expression.ReferencedQuerySource));
		}

		private HqlTreeNode VisitVBStringComparisonExpression(VBStringComparisonExpression expression)
		{
			// We ignore the case sensitivity flag in the same way that == does.
			return VisitExpression(expression.Comparison);
		}

		protected HqlTreeNode VisitBinaryExpression(BinaryExpression expression)
		{
			var lhs = VisitExpression(expression.Left).AsExpression();
			var rhs = VisitExpression(expression.Right).AsExpression();

			switch (expression.NodeType)
			{
				case ExpressionType.Equal:
					return TranslateEqualityComparison(expression, lhs, rhs,
													   expr => _hqlTreeBuilder.IsNull(expr),
													   (l, r) => _hqlTreeBuilder.Equality(l, r));

				case ExpressionType.NotEqual:
					return TranslateEqualityComparison(expression, lhs, rhs,
													   expr => _hqlTreeBuilder.IsNotNull(expr),
													   (l, r) => _hqlTreeBuilder.Inequality(l, r));

				case ExpressionType.And:
					return _hqlTreeBuilder.BitwiseAnd(lhs, rhs);

				case ExpressionType.AndAlso:
					return _hqlTreeBuilder.BooleanAnd(lhs.AsBooleanExpression(), rhs.AsBooleanExpression());

				case ExpressionType.Or:
					return _hqlTreeBuilder.BitwiseOr(lhs, rhs);

				case ExpressionType.OrElse:
					return _hqlTreeBuilder.BooleanOr(lhs.AsBooleanExpression(), rhs.AsBooleanExpression());

				case ExpressionType.Add:
					if (expression.Left.Type == typeof (string) && expression.Right.Type == typeof (string))
					{
						return _hqlTreeBuilder.MethodCall("concat", lhs, rhs);
					}
					return _hqlTreeBuilder.Add(lhs, rhs);

				case ExpressionType.Subtract:
					return _hqlTreeBuilder.Subtract(lhs, rhs);

				case ExpressionType.Multiply:
					return _hqlTreeBuilder.Multiply(lhs, rhs);

				case ExpressionType.Divide:
					return _hqlTreeBuilder.Divide(lhs, rhs);

				case ExpressionType.Modulo:
					return _hqlTreeBuilder.MethodCall("mod", lhs, rhs);

				case ExpressionType.LessThan:
					return _hqlTreeBuilder.LessThan(lhs, rhs);

				case ExpressionType.LessThanOrEqual:
					return _hqlTreeBuilder.LessThanOrEqual(lhs, rhs);

				case ExpressionType.GreaterThan:
					return _hqlTreeBuilder.GreaterThan(lhs, rhs);

				case ExpressionType.GreaterThanOrEqual:
					return _hqlTreeBuilder.GreaterThanOrEqual(lhs, rhs);

				case ExpressionType.Coalesce:
					return _hqlTreeBuilder.Coalesce(lhs, rhs);
			}

			throw new InvalidOperationException();
		}

		private HqlTreeNode TranslateEqualityComparison(BinaryExpression expression, HqlExpression lhs, HqlExpression rhs, Func<HqlExpression, HqlTreeNode> applyNullComparison, Func<HqlExpression, HqlExpression, HqlTreeNode> applyRegularComparison)
		{
			// Check for nulls on left or right.
			if (expression.Right is ConstantExpression && expression.Right.Type.IsNullableOrReference() &&
				((ConstantExpression) expression.Right).Value == null)
			{
				rhs = null;
			}

			if (expression.Left is ConstantExpression && expression.Left.Type.IsNullableOrReference() &&
				((ConstantExpression) expression.Left).Value == null)
			{
				lhs = null;
			}

			// Need to check for boolean equality
			if (lhs is HqlBooleanExpression || rhs is HqlBooleanExpression)
			{
				if (lhs != null)
					lhs = GetExpressionForBooleanEquality(expression.Left, lhs);
				if (rhs != null)
					rhs = GetExpressionForBooleanEquality(expression.Right, rhs);
			}

			if (lhs == null)
				return applyNullComparison(rhs);
			if (rhs == null)
				return applyNullComparison(lhs);

			return applyRegularComparison(lhs, rhs);
		}

		private HqlExpression GetExpressionForBooleanEquality(Expression @operator, HqlExpression original)
		{
			//When the expression is a constant then use the constant
			var operandEx = @operator as ConstantExpression;
			if (operandEx != null)
			{
				NamedParameter namedParameter;
				if (_parameters.ConstantToParameterMap.TryGetValue(operandEx, out namedParameter))
				{
					_parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));
					return _hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression();
				}

				return _hqlTreeBuilder.Constant(operandEx.Value);
			}

			//When the expression is a member-access not nullable then use the HbmDot
			var memberAccessExpression = @operator as MemberExpression;
			if (ExpressionType.MemberAccess.Equals(@operator.NodeType) && memberAccessExpression != null && typeof (bool) == memberAccessExpression.Type)
			{
				// this case make the difference when the property "Value" of a nullable type is used (ignore the null since the user is explicity checking the Value)
				return original;
			}

			var subQueryExpression = @operator as SubQueryExpression;
			if (subQueryExpression != null)
			{
				var resultOperators = subQueryExpression.QueryModel.ResultOperators;
				if (resultOperators.Count == 1 &&
					(resultOperators[0] is FirstResultOperator ||
					 resultOperators[0] is SingleResultOperator))
				{
					return original;
				}
			}

			//When the expression is a member-access nullable then use the "case" clause to transform it to boolean (to use always .NET meaning instead leave the DB the behavior for null)
			//When the expression is a complex-expression then use the "case" clause to transform it to boolean
			return _hqlTreeBuilder.Case(new[] {_hqlTreeBuilder.When(original, _hqlTreeBuilder.True())}, _hqlTreeBuilder.False());
		}

		protected HqlTreeNode VisitUnaryExpression(UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Not:
					return _hqlTreeBuilder.BooleanNot(VisitExpression(expression.Operand).AsBooleanExpression());
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.TypeAs:
					if ((expression.Operand.Type.IsPrimitive || expression.Operand.Type == typeof(Decimal)) &&
						(expression.Type.IsPrimitive || expression.Type == typeof(Decimal)))
					{
						return _hqlTreeBuilder.Cast(VisitExpression(expression.Operand).AsExpression(), expression.Type);
					}

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
			IHqlGeneratorForProperty generator;

			if (_functionRegistry.TryGetGenerator(expression.Member, out generator))
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

			if (_parameters.ConstantToParameterMap.TryGetValue(expression, out namedParameter))
			{
				_parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));

				if (namedParameter.Value is bool)
				{
					return _hqlTreeBuilder.Equality(_hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression(), _hqlTreeBuilder.Constant(true));
				}

				return _hqlTreeBuilder.Parameter(namedParameter.Name).AsExpression();
			}

			return _hqlTreeBuilder.Constant(expression.Value);
		}

		protected HqlTreeNode VisitMethodCallExpression(MethodCallExpression expression)
		{
			IHqlGeneratorForMethod generator;

			var method = expression.Method;
			if (!_functionRegistry.TryGetGenerator(method, out generator))
			{
				throw new NotSupportedException(method.ToString());
			}

			return generator.BuildHql(method, expression.Object, expression.Arguments, _hqlTreeBuilder, this);
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
			var test = VisitExpression(expression.Test).AsExpression();
			var ifTrue = BooleanToCaseConvertor.ConvertBooleanToCase(VisitExpression(expression.IfTrue).AsExpression());
			var ifFalse = (expression.IfFalse != null
							   ? BooleanToCaseConvertor.ConvertBooleanToCase(VisitExpression(expression.IfFalse).AsExpression())
							   : null);

			var @case = _hqlTreeBuilder.Case(new[] {_hqlTreeBuilder.When(test, ifTrue)}, ifFalse);

			return (expression.Type == typeof (bool) || expression.Type == (typeof (bool?)))
					   ? (HqlTreeNode) _hqlTreeBuilder.Equality(@case, _hqlTreeBuilder.True())
					   : _hqlTreeBuilder.Cast(@case, expression.Type);
		}

		protected HqlTreeNode VisitSubQueryExpression(SubQueryExpression expression)
		{
			ExpressionToHqlTranslationResults query = QueryModelVisitor.GenerateHqlQuery(expression.QueryModel, _parameters, false);
			return query.Statement;
		}

		protected HqlTreeNode VisitNewArrayExpression(NewArrayExpression expression)
		{
			var visitor = new HqlGeneratorExpressionTreeVisitor(_parameters);
			var expressionSubTree = expression.Expressions.Select(exp => visitor.Visit(exp));
			return _hqlTreeBuilder.ExpressionSubTreeHolder(expressionSubTree);
		}
	}
}