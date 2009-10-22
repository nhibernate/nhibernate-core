using System.Collections.Generic;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Impl;

namespace NHibernate
{
    public interface IQueryExpression
    {
        IASTNode Translate(ISessionFactory sessionFactory);
        string Key { get; }
        System.Type Type { get; }
    	IList<NamedParameterDescriptor> ParameterDescriptors { get; }
    	void SetQueryPropertiesPriorToExecute(IQuery impl);
    }
}