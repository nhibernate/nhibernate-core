using System;
using System.Linq.Expressions;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Remove redundant casts to the same type or to superclass (upcast) in <see cref="ExpressionType.Convert"/>, <see cref=" ExpressionType.ConvertChecked"/>
	/// and <see cref="ExpressionType.TypeAs"/> <see cref="UnaryExpression"/>s
	/// </summary>
	public class RemoveRedundantCast : IExpressionTransformer<UnaryExpression>
	{
		private static readonly ExpressionType[] _supportedExpressionTypes = new[]
			{
				ExpressionType.TypeAs,
				ExpressionType.Convert,
				ExpressionType.ConvertChecked,
			};

		public Expression Transform(UnaryExpression expression)
		{
			if (expression.Type != typeof(object) &&
				expression.Type != typeof(Enum) &&
				expression.Type.IsAssignableFrom(expression.Operand.Type) &&
				expression.Method == null &&
				!expression.IsLiftedToNull)
			{
				return expression.Operand;
			}

			// Reduce double casting (e.g. (long?)(long)3 => (long?)3)
			if (expression.Operand.NodeType == ExpressionType.Convert &&
				expression.Type.UnwrapIfNullable() == expression.Operand.Type)
			{
				return Expression.Convert(((UnaryExpression) expression.Operand).Operand, expression.Type);
			}

			return expression;
		}

		public ExpressionType[] SupportedExpressionTypes
		{
			get { return _supportedExpressionTypes; }
		}
	}
}
