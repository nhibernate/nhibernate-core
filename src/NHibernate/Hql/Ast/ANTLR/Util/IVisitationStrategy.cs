using Antlr.Runtime.Tree;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	public interface IVisitationStrategy
	{
		void Visit(IASTNode node);
	}
}
