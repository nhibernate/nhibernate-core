using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq.Visitors
{
	public interface IHqlExpressionVisitor
	{
		ISessionFactory SessionFactory { get; }

		HqlTreeNode Visit(Expression expression);
	}
}
