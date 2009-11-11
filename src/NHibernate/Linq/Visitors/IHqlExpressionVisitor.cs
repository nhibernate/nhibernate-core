using System.Linq.Expressions;
using NHibernate.Hql.Ast;

namespace NHibernate.Linq.Visitors
{
    public interface IHqlExpressionVisitor
    {
        HqlTreeNode Visit(Expression expression);
    }
}