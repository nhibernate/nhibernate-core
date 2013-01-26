using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Type;
using Remotion.Linq;
using NHibernate.Util;
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
				var memberType = expression.Member.GetPropertyOrFieldType();

				if (memberType != null && memberType.IsCollectionType())
				{
					HasSubquery = true;
					Expression = expression;
				}
			}

			return base.VisitMemberExpression(expression);
		}
	}
}
