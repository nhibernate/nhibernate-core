using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	using System.Collections;

	internal class NestedSelectDetector : NhExpressionTreeVisitor
	{
		private readonly ICollection<Expression> _expressions = new List<Expression>();

		public ICollection<Expression> Expressions
		{
			get { return _expressions; }
		}

		public bool HasSubqueries
		{
			get { return Expressions.Count > 0; }
		}

		protected override Expression VisitSubQueryExpression(SubQueryExpression expression)
		{
			if (expression.QueryModel.ResultOperators.Count == 0)
				Expressions.Add(expression);
			return base.VisitSubQueryExpression(expression);
		}

		protected override Expression VisitMemberExpression(MemberExpression expression)
		{
			var querySourceReferenceExpression = expression.Expression as QuerySourceReferenceExpression;
			if (querySourceReferenceExpression != null)
			{
				var memberType = expression.Member.GetPropertyOrFieldType();

				if (memberType != null && IsCollectionType(memberType))
				{
					Expressions.Add(expression);
				}
			}

			return base.VisitMemberExpression(expression);
		}

		private static bool IsCollectionType(System.Type type)
		{
			return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) && !type.IsArray;
		}
	}
}