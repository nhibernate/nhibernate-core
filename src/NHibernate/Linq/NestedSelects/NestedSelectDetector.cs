using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Linq.Visitors;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq.NestedSelects
{
	internal class NestedSelectDetector : NhExpressionTreeVisitor
	{
		private readonly ISessionFactory sessionFactory;
		private readonly ICollection<Expression> _expressions = new List<Expression>();

		public NestedSelectDetector(ISessionFactory sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

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

				if (memberType != null && memberType.IsCollectionType() && IsMappedCollection(expression.Member))
				{
					Expressions.Add(expression);
				}
			}

			return base.VisitMemberExpression(expression);
		}

		private bool IsMappedCollection(MemberInfo memberInfo)
		{
			var collectionRole = memberInfo.DeclaringType.FullName + "." + memberInfo.Name;

			return sessionFactory.GetCollectionMetadata(collectionRole) != null;
		}
	}
}
