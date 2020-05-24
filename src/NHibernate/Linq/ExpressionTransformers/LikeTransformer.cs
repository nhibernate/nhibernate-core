using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Replace <see cref="string.StartsWith(string)"/>, <see cref="string.EndsWith(string)"/> and <see cref="string.Contains(string)"/>
	/// with <see cref="SqlMethods.Like(string, string)"/>
	/// </summary>
	internal class LikeTransformer : IExpressionTransformer<MethodCallExpression>
	{
		private static readonly MethodInfo Like = ReflectHelper.FastGetMethod(SqlMethods.Like, default(string), default(string));

		public ExpressionType[] SupportedExpressionTypes { get; } = {ExpressionType.Call};

		public Expression Transform(MethodCallExpression expression)
		{
			if (expression.Method.Name == nameof(string.StartsWith) && expression.Method == ReflectionCache.StringMethods.StartsWith)
			{
				return Expression.Call(
					Like,
					expression.Object,
					Concat(expression.Arguments[0], Expression.Constant("%"))
				);
			}

			if (expression.Method.Name == nameof(string.EndsWith) && expression.Method == ReflectionCache.StringMethods.EndsWith)
			{
				return Expression.Call(
					Like,
					expression.Object,
					Concat(Expression.Constant("%"), expression.Arguments[0])
				);
			}

			if (expression.Method.Name == nameof(string.Contains) && expression.Method == ReflectionCache.StringMethods.Contains)
			{
				return Expression.Call(
					Like,
					expression.Object,
					Concat(Concat(Expression.Constant("%"), expression.Arguments[0]), Expression.Constant("%"))
				);
			}

			return expression;
		}

		private static Expression Concat(Expression arg1, Expression arg2)
		{
			if (arg1 is ConstantExpression const1 && arg2 is ConstantExpression const2)
			{
				return Expression.Constant(string.Concat(const1.Value, const2.Value));
			}

			return Expression.Add(arg1, arg2, ReflectionCache.StringMethods.Concat);
		}
	}
}
