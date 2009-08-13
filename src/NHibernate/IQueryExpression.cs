using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate
{
    public interface IQueryExpression
    {
        IASTNode Translate(ISessionFactory sessionFactory);
        string Key { get; }
        System.Type Type { get; }
    }
}