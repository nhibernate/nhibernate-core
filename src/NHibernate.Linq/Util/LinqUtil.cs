using System.Linq.Expressions;

namespace NHibernate.Linq.Util
{
	public static class LinqUtil
	{
		public static Expression StripQuotes(Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression = ((UnaryExpression) expression).Operand;
			}
			return expression;
		}

		public static bool IsAnonymousType(System.Type type)
		{
			return type != null && type.Name.StartsWith("<");
		}
	}
}