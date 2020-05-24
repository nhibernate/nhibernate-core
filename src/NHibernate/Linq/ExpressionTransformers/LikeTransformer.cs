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
			if (IsLike(expression, out var value))
			{
				return Expression.Call(
					Like,
					expression.Object,
					Expression.Constant(value)
				);
			}

			return expression;
		}

		private static bool IsLike(MethodCallExpression expression, out string value)
		{
			if (expression.Method == ReflectionCache.StringMethods.StartsWith)
			{
				if (expression.Arguments[0] is ConstantExpression constantExpression)
				{
					value = string.Concat(constantExpression.Value, "%");
					return true;
				}
			}
			else if (expression.Method == ReflectionCache.StringMethods.EndsWith)
			{
				if (expression.Arguments[0] is ConstantExpression constantExpression)
				{
					value = string.Concat("%", constantExpression.Value);
					return true;
				}
			}
			else if (expression.Method == ReflectionCache.StringMethods.Contains)
			{
				if (expression.Arguments[0] is ConstantExpression constantExpression)
				{
					value = string.Concat("%", constantExpression.Value, "%");
					return true;
				}
			}

			value = null;
			return false;
		}
	}
}
