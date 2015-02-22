using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq
{
	public static class ExpressionExtensions
	{
		public static bool IsGroupingKey(this MemberExpression expression)
		{
			return expression.Member.Name == "Key" && expression.Member.DeclaringType!=null &&
					 expression.Member.DeclaringType.IsGenericType && expression.Member.DeclaringType.GetGenericTypeDefinition() == typeof(IGrouping<,>);
		}

		public static bool IsGroupingKeyOf(this MemberExpression expression,GroupResultOperator groupBy)
		{
			if (!expression.IsGroupingKey())
			{
				return false;
			}
			
			var querySource = expression.Expression as QuerySourceReferenceExpression;
			if (querySource == null) return false;
			
			var fromClause = querySource.ReferencedQuerySource as MainFromClause;
			if (fromClause == null) return false;
			
			var query = fromClause.FromExpression as SubQueryExpression;
			if (query == null) return false;
	
			return query.QueryModel.ResultOperators.Contains(groupBy);
		}
	}
}
