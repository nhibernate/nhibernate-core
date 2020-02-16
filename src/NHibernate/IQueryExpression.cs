using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate
{
	//TODO 6.0: Merge into IQueryExpression
	internal interface ICacheableQueryExpression
	{
		bool CanCachePlan { get; }
	}

	public interface IQueryExpression
	{
		IASTNode Translate(ISessionFactoryImplementor sessionFactory, bool filter);
		string Key { get; }
		System.Type Type { get; }
		IList<NamedParameterDescriptor> ParameterDescriptors { get; }
	}
}
