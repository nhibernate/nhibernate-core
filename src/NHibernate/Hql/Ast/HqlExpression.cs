using System.Collections.Generic;
using System.Reflection;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast
{
    public class HqlExpression : IQueryExpression
    {
        private readonly IASTNode _node;
        private readonly System.Type _type;
        private readonly string _key;

        public HqlExpression(HqlQuery node, System.Type type)
        {
            _node = node.AstNode;
            _type = type;
            _key = _node.ToStringTree();
        }

        public IASTNode Translate(ISessionFactory sessionFactory)
        {
            return _node;
        }

        public string Key
        {
            get { return _key; }
        }

        public System.Type Type
        {
            get { return _type; }
        }
    }
}
