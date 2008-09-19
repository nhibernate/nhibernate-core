using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Util;

namespace NHibernate.Linq.Visitors
{
	public class OrderByTransformer : ExpressionVisitor
	{
		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			OrderByType order = IsOrderCall(m);
			if (order != OrderByType.None)
			{
				var type = (OrderType) order;
				Expression expr = Visit(m.Arguments[0]);
				Expression lambda = Visit(LinqUtil.StripQuotes(m.Arguments[1]));
				return new OrderExpression(expr, lambda as LambdaExpression, type, m.Type);
			}
			else
				return base.Visit(m);
		}

		protected static OrderByType IsOrderCall(Expression ex)
		{
			if (ex is MethodCallExpression)
			{
				var m = ex as MethodCallExpression;
				System.Type t = m.Method.DeclaringType;
				if (t == typeof (Enumerable) || t == typeof (Queryable))
				{
					switch (m.Method.Name)
					{
						case "OrderBy":
						case "ThenBy":
							return OrderByType.Ascending;
						case "OrderByDescending":
						case "ThenByDescending":
							return OrderByType.Descending;
						default:
							return OrderByType.None;
					}
				}
				return OrderByType.None;
			}
			return OrderByType.None;
		}

		#region Nested type: OrderByType

		protected enum OrderByType
		{
			None = 0,
			Ascending,
			Descending
		}

		#endregion
	}
}