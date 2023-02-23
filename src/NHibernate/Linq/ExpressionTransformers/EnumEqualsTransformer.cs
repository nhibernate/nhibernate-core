using System;
using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Transforms <see cref="Enum"/>.Equals method to equality operator.
	/// This cannot be done easily in <see cref="Functions.IHqlGeneratorForMethod"/> as Equals operator
	/// is boxed to ((object)Enum).Equals((object)EnumValue) expression.
	/// </summary>
	public class EnumEqualsTransformer : IExpressionTransformer<MethodCallExpression>
	{
		public ExpressionType[] SupportedExpressionTypes => _supportedExpressionTypes;

		private static readonly ExpressionType[] _supportedExpressionTypes = new[]
		{
			ExpressionType.Call
		};

		public Expression Transform(MethodCallExpression expression)
		{
			if (expression.Object?.Type.IsEnum == true &&
				expression.Method.Name == nameof(Enum.Equals) &&
				expression.Arguments.Count == 1)
			{
				return Expression.Equal(expression.Object, Unwrap(expression.Arguments[0], expression.Object.Type));
			}

			return expression;
		}

		private Expression Unwrap(Expression expression, System.Type type)
		{
			// 1) unwrap convert operand as convert is converting from enum type to object
			if (expression is UnaryExpression u && u.NodeType == ExpressionType.Convert)
			{
				return u.Operand;
			}

			// 2) convert constant expression which is of type object
			if (expression is ConstantExpression c)
			{
				return Expression.Convert(c, type);
			}

			return expression;
		}
	}
}
