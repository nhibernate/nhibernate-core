using System.Linq.Expressions;
using NHibernate.Linq.Expressions;
using NHibernate.SqlCommand;

namespace NHibernate.Linq.Visitors
{
	public class NHExpressionToSqlQueryTranslator : NHibernateExpressionVisitor
	{
		protected SqlStringBuilder sqlStringBuilder;

		public SqlString Translate(Expression expression)
		{
			sqlStringBuilder = new SqlStringBuilder();
			Visit(expression);
			return sqlStringBuilder.ToSqlString();
		}

		protected override Expression VisitSelect(SelectExpression select)
		{
			return select;
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			return base.VisitMethodCall(m);
		}
	}
}