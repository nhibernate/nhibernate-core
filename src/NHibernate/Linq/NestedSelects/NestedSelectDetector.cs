using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	internal class NestedSelectDetector : ExpressionTreeVisitor
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

				if (memberType != null && memberType.IsCollectionType())
				{
					Expressions.Add(expression);
				}
			}

			return base.VisitMemberExpression(expression);
		}
	}
}