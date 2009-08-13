using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public class NhQueryProvider : QueryProvider
	{
		private readonly ISession _session;

        public NhQueryProvider(ISession session)
		{
			_session = session;
		}

		public override object Execute(Expression expression)
		{
			// walk the expression tree and build an HQL AST to mirror it
            return _session.CreateQuery(new LinqExpression(expression)).List();
		}
	}
}