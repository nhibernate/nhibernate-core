using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NHibernate.Linq.Util
{
	public static class LinqUtil
	{
		public static Expression StripQuotes(Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression=((UnaryExpression)expression).Operand;
			}
			return expression;
		}
	}
}
