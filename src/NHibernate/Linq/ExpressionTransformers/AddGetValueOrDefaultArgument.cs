using System;
using System.Linq.Expressions;
using NHibernate.Linq.Functions;
using NHibernate.Util;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;

namespace NHibernate.Linq.ExpressionTransformers
{
	/// <summary>
	/// Supplies the implicit argument of the parameterless <c>Nullable&lt;T&gt;.GetValueOrDefault()</c> overload
	/// whenever <c>default(T)</c> cannot be rendered as an HQL literal.
	/// </summary>
	/// <remarks>
	/// The parameterless overload has no argument in the expression tree, so its default value is built at HQL
	/// generation time and inlined as a literal by <see cref="Hql.Ast.HqlTreeBuilder.Constant"/>. That yields
	/// invalid SQL for most types (e.g. <c>coalesce(col, '01/01/0001 00:00:00')</c> for a <see cref="DateTime"/>)
	/// or is not supported at all. By adding the argument here, before the query is parameterized, the default
	/// value goes through the usual constant to parameter conversion instead.
	/// </remarks>
	internal class AddGetValueOrDefaultArgument : IExpressionTransformer<MethodCallExpression>
	{
		public ExpressionType[] SupportedExpressionTypes { get; } = { ExpressionType.Call };

		public Expression Transform(MethodCallExpression expression)
		{
			if (expression.Arguments.Count > 0 || !GetValueOrDefaultGenerator.IsGetValueOrDefaultMethod(expression.Method))
				return expression;

			var type = expression.Object.Type.NullableOf();
			if (CanBeInlinedAsLiteral(type))
				return expression;

			return Expression.Call(
				expression.Object,
				expression.Method.DeclaringType.GetMethod(expression.Method.Name, new[] {type}),
				Expression.Constant(Activator.CreateInstance(type), type));
		}

		/// <summary>
		/// Whether <c>default(T)</c> is rendered correctly by <see cref="Hql.Ast.HqlTreeBuilder.Constant"/>.
		/// </summary>
		/// <remarks>
		/// Enums are excluded although their type code is integral: they would be rendered by their member name.
		/// Char and DateTime are excluded although handled, they would be rendered as an invalid, respectively
		/// culture dependent, string literal. The remaining cases all default to zero or false, which is rendered
		/// the same way whatever the culture.
		/// </remarks>
		private static bool CanBeInlinedAsLiteral(System.Type type)
		{
			if (type.IsEnum)
				return false;

			switch (System.Type.GetTypeCode(type))
			{
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
				case TypeCode.Boolean:
					return true;
				default:
					return false;
			}
		}
	}
}
