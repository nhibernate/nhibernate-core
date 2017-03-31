using System.Linq.Expressions;
using NHibernate.Engine;

namespace NHibernate.Linq
{
	public class NhLinqDeleteExpression : NhLinqExpression
	{
		protected override QueryMode QueryMode => QueryMode.Delete;

		public NhLinqDeleteExpression(Expression expression, ISessionFactoryImplementor sessionFactory)
			: base(expression, sessionFactory)
		{
			Key = "DELETE " + Key;
		}
	}
}