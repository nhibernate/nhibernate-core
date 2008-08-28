using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate.SqlCommand;

namespace NHibernate.Linq.Visitors
{
	public class NHExpressionToSqlQueryTranslator:NHibernateExpressionVisitor
	{
		protected SqlStringBuilder sqlStringBuilder;
		public SqlString Translate(Expression expression)
		{
			this.sqlStringBuilder = new SqlStringBuilder();
			this.Visit(expression);
			return sqlStringBuilder.ToSqlString();
		}
		protected override Expression VisitSelect(NHibernate.Linq.Expressions.SelectExpression select)
		{
			return select;
		}
		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			return base.VisitMethodCall(m);
		}
	}
}
