using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Util;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace NHibernate.Linq.NestedSelects
{
	internal class NestedSelectDetector : RelinqExpressionVisitor
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

		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			if (expression.QueryModel.ResultOperators.Count == 0)
				Expressions.Add(expression);
			return base.VisitSubQuery(expression);
		}

		protected override Expression VisitMember(MemberExpression expression)
		{
			var memberType = ReflectHelper.GetPropertyOrFieldType(expression.Member);

			if (memberType != null && memberType.IsCollectionType()
				&& IsChainedFromQuerySourceReference(expression)
				&& IsMappedCollection(expression.Member))
			{
				Expressions.Add(expression);
			}

			return base.VisitMember(expression);
		}

		private bool IsMappedCollection(MemberInfo memberInfo)
		{
			var collectionRole = memberInfo.DeclaringType.FullName + "." + memberInfo.Name;

			return sessionFactory.GetCollectionMetadata(collectionRole) != null;
		}

		private bool IsChainedFromQuerySourceReference(MemberExpression expression)
		{
			while (expression != null)
			{
				if (expression.Expression is QuerySourceReferenceExpression)
				{
					return true;
				}
				expression = expression.Expression as MemberExpression;
			}

			return false;
		}
	}
}
