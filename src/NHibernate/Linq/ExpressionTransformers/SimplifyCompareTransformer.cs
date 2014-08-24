using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Functions;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Applications of the string.Compare(a,b) and a.CompareTo(b) (for various types)
	/// that are then immediately compared to 0 can be simplified by removing the 
	/// Compare/CompareTo method call. The comparison operator is then applied
	/// directly to the arguments for the Compare/CompareTo call.
	/// </summary>
	internal class SimplifyCompareTransformer : IExpressionTransformer<BinaryExpression>
	{
		// Examples:
		// string.Compare(a, b) = 0    =>    a = b
		// string.Compare(a, b) > 0    =>    a > b
		// string.Compare(a, b) < 0    =>    a < b
		// a.CompareTo(b) op 0         =>    a op b


		private static readonly IDictionary<ExpressionType, ExpressionType> ActingOperators = new Dictionary
			<ExpressionType, ExpressionType>
			{
				{ExpressionType.LessThan, ExpressionType.GreaterThan},
				{ExpressionType.LessThanOrEqual, ExpressionType.GreaterThanOrEqual},
				{ExpressionType.GreaterThan, ExpressionType.LessThan},
				{ExpressionType.GreaterThanOrEqual, ExpressionType.LessThanOrEqual},
				{ExpressionType.Equal, ExpressionType.Equal},
				{ExpressionType.NotEqual, ExpressionType.NotEqual},
			};


		public ExpressionType[] SupportedExpressionTypes
		{
			get { return ActingOperators.Keys.ToArray(); }
		}


		public Expression Transform(BinaryExpression expression)
		{
			ExpressionType inverseExpressionType;
			if (ActingOperators.TryGetValue(expression.NodeType, out inverseExpressionType))
			{
				if (IsCompare(expression.Left) && IsConstantZero(expression.Right))
					return Build(expression.NodeType, expression.Left);

				// When the zero is to the left, we need to use the inverse operator.
				// E.g. 0 >= a.CompareTo(b)   is equivalent to   a.CompareTo(b) <= 0
				if (IsConstantZero(expression.Left) && IsCompare(expression.Right))
					return Build(inverseExpressionType, expression.Right);
			}

			return expression;
		}


		private static bool IsConstantZero(Expression expression)
		{
			var constantExpr = expression as ConstantExpression;
			if (constantExpr != null)
			{
				if (constantExpr.Type == typeof(int) && ((int)constantExpr.Value) == 0)
					return true;

				if (constantExpr.Type == typeof(long) && ((long)constantExpr.Value) == 0)
					return true;
			}

			return false;
		}


		private static bool IsCompare(Expression expression)
		{
			var methodCall = expression as MethodCallExpression;
			if (methodCall == null)
				return false;

			return CompareGenerator.IsCompareMethod(methodCall.Method);
		}


		private Expression Build(ExpressionType et, Expression expression)
		{
			var methodCall = expression as MethodCallExpression;

			var lhs = methodCall.Arguments.Count == 1 ? methodCall.Object : methodCall.Arguments[0];
			var rhs = methodCall.Arguments.Count == 1 ? methodCall.Arguments[0] : methodCall.Arguments[1];

			// There is no built in lt, lte, gt or gte for strings and some other types. Must
			// pass along a placeholder method for these cases.
			MethodInfo dummyCompare;
			if (dummies.TryGetValue(methodCall.Arguments[0].Type, out dummyCompare))
				return Expression.MakeBinary(et, lhs, rhs, false, dummyCompare);

			return Expression.MakeBinary(et, lhs, rhs);
		}


		private static readonly IDictionary<System.Type, MethodInfo> dummies = new Dictionary<System.Type, MethodInfo>
			{
				// Corresponds to string.Compare(a, b).
				{typeof (string), ReflectionHelper.GetMethod(() => DummyComparison<string>(null, null))},

				// System.Data.Services.Providers.DataServiceProviderMethods has Compare methods for these types.
				{typeof (bool), ReflectionHelper.GetMethod(() => DummyComparison<bool>(false, false))},
				{typeof (bool?), ReflectionHelper.GetMethod(() => DummyComparison<bool?>(null, null))},
				{typeof (Guid), ReflectionHelper.GetMethod(() => DummyComparison<Guid>(Guid.Empty, Guid.Empty))},
				{typeof (Guid?), ReflectionHelper.GetMethod(() => DummyComparison<Guid?>(null, null))},
			};


		private static bool DummyComparison<T>(T lhs, T rhs)
		{
			throw new NotSupportedException("This method is not intended to be called.");
		}
	}
}