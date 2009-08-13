using System;
using System.Linq.Expressions;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Linq
{
    class LinqExpression : IQueryExpression
    {
        private readonly Expression _linqExpression;

        internal LinqExpression(Expression linqExpression)
        {
            _linqExpression = linqExpression;
        }

        public IASTNode Translate(ISessionFactory sessionFactory)
        {
            ASTFactory factory = new ASTFactory(new ASTTreeAdaptor());

            return factory.CreateNode(HqlSqlWalker.QUERY, "query",
                                      factory.CreateNode(HqlSqlWalker.SELECT_FROM, "select from",
                                                         factory.CreateNode(HqlSqlWalker.FROM, "from",
                                                                            factory.CreateNode(
                                                                                HqlSqlWalker.RANGE, "range",
                                                                                factory.CreateNode(
                                                                                    HqlSqlWalker.IDENT,
                                                                                    "Product")))));
        }

        public string Key
        {
            get { return _linqExpression.ToString(); }
        }

        public System.Type Type
        {
            get { return _linqExpression.Type.GetGenericArguments()[0]; }
        }
    }
}
