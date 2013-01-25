using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Type;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	class NestedSelectDetector : ExpressionTreeVisitor
	{
		public bool HasSubquery { get; set; }
		public Expression Expression { get; private set; }

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			HasSubquery |= expression.QueryModel.ResultOperators.Count == 0;
			Expression = expression;
			return base.VisitSubQueryExpression(expression);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			var querySourceReferenceExpression = expression.Expression as QuerySourceReferenceExpression;
			if (querySourceReferenceExpression != null)
			{
				System.Type memberType = null;

				var propertyInfo = expression.Member as PropertyInfo;
				if (propertyInfo != null)
				{
					memberType = propertyInfo.PropertyType;
				}

				var fieldInfo = expression.Member as FieldInfo;
				if (fieldInfo != null)
				{
					memberType = fieldInfo.FieldType;
				}

				if (memberType != null && IsCollectionType(memberType))
				{
					HasSubquery = true;
					Expression = expression;
				}
			}
			return base.VisitMemberExpression(expression);
		}

		private static bool IsCollectionType(System.Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
		}
	}
}