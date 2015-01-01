using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Expressions;
using NHibernate.Linq.Visitors;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using System.Reflection;

namespace NHibernate.Linq
{
	public class NhLinqInsertExpression<TInput,TOutput> : NhLinqExpression
	{
		public NhLinqInsertExpression(Expression expression, Assignments<TInput, TOutput> assignments, ISessionFactoryImplementor sessionFactory)
			: base(RewriteForInsert(expression, assignments), sessionFactory)
		{
			Key = Key + "INSERT";
		}

		internal static Expression RewriteForInsert(Expression expression, Assignments<TInput, TOutput> assignments)
		{
			var lambda = assignments.ConvertToDictionaryExpression();

			return
				Expression.Call(
					typeof(Queryable), "Select",
					new System.Type[] { typeof(TInput), lambda.Body.Type },
					expression, Expression.Quote(lambda));
		}

		protected override ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, VisitorParameters visitorParameters)
		{
			visitorParameters.EntityType = typeof (TOutput);
			return QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true, null, QueryMode.Insert);
		}
	}
}