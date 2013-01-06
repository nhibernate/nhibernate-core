using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;

namespace NHibernate.Linq.Visitors
{
	/// <summary>
	/// Applications of the string.Compare(a,b) and a.CompareTo(b) (for various types)
	/// that are then immediately compared to 0 can be simplified by removing the 
	/// Compare/CompareTo method call. The comparison operator is then applied
	/// directly to the arguments for the Compare/CompareTo call.
	/// </summary>
	internal class SimplifyCompareRewriter : NhExpressionTreeVisitor
	{
		// Examples:
		// string.Compare(a, b) = 0    =>    a = b
		// string.Compare(a, b) > 0    =>    a > b
		// string.Compare(a, b) < 0    =>    a < b
		// a.CompareTo(b) op 0         =>    a op b


		internal static void ReWrite(QueryModel queryModel)
		{
			var v = new SimplifyCompareRewriter();
			queryModel.TransformExpressions(v.VisitExpression);
		}


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

		private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
			{
				ReflectionHelper.GetMethodDefinition(() => string.Compare(null, null)),
				ReflectionHelper.GetMethodDefinition<string>(s => s.CompareTo(s)),
				ReflectionHelper.GetMethodDefinition<char>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<byte>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<short>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<int>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<long>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<float>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<double>(x => x.CompareTo(x)),
				ReflectionHelper.GetMethodDefinition<decimal>(x => x.CompareTo(x)),
			};


		protected override Expression VisitBinaryExpression(BinaryExpression expression)
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

			return base.VisitBinaryExpression(expression);
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

			if (!ActingMethods.Contains(methodCall.Method))
				return false;

			return true;
		}


		private Expression Build(ExpressionType et, Expression expression)
		{
			var methodCall = expression as MethodCallExpression;

			var lhs = methodCall.Arguments.Count == 1 ? methodCall.Object : methodCall.Arguments[0];
			var rhs = methodCall.Arguments.Count == 1 ? methodCall.Arguments[0] : methodCall.Arguments[1];

			// There is no built in lt, lt, gt or gte for strings - must pass along a placeholder
			// method for this case.
			if (methodCall.Arguments[0].Type == typeof (string))
				return Expression.MakeBinary(et, VisitExpression(lhs), VisitExpression(rhs), false, DummyStringCompareMethod);

			return Expression.MakeBinary(et, VisitExpression(lhs), VisitExpression(rhs));
		}


		private static readonly MethodInfo DummyStringCompareMethod = ReflectionHelper.GetMethodDefinition(() => DummyStringCompare(null, null));
		private static bool DummyStringCompare(string lhs, string rhs)
		{
			throw new NotImplementedException();
		}
	}
}