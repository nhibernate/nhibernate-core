using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Linq.Visitors;
using Remotion.Linq;

namespace NHibernate.Linq
{
	public class NhLinqDeleteExpression : NhLinqExpression
	{
		public NhLinqDeleteExpression(Expression expression, ISessionFactoryImplementor sessionFactory)
			: base(expression, sessionFactory)
		{
			Key = Key + "DELETE";
		}

		protected override ExpressionToHqlTranslationResults GenerateHqlQuery(QueryModel queryModel, VisitorParameters visitorParameters)
		{
			visitorParameters.EntityType = Type;
			return QueryModelVisitor.GenerateHqlQuery(queryModel, visitorParameters, true, null, QueryMode.Delete);
		}
	}
}