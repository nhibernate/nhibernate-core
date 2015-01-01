using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;

namespace NHibernate.Linq
{
	public class NhLinqUpdateExpression<T> : NhLinqExpression
	{
		private readonly bool _versioned;

		public NhLinqUpdateExpression(Expression expression, Assignments<T,T> assignments, ISessionFactoryImplementor sessionFactory, bool versioned)
			: base(RewriteForUpdate(expression, assignments), sessionFactory)
		{
			_versioned = versioned;
			Key = Key + "UPDATE" + versioned;
		}

		protected override ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, VisitorParameters visitorParameters)
		{
			visitorParameters.EntityType = typeof(T);
			return QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true, null, _versioned ? QueryMode.UpdateVersioned : QueryMode.Update);
		}

		internal static Expression RewriteForUpdate(Expression expression, Assignments<T,T> assignments)
		{
			var lambda = assignments.ConvertToDictionaryExpression();

			return
				Expression.Call(
					typeof(Queryable), "Select",
					new System.Type[] { typeof(T), lambda.Body.Type },
					expression, Expression.Quote(lambda));
		}
	}

	
}