using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public interface IASTFactory
	{
		IASTNode CreateNode(int type, string text, params IASTNode[] children);
	}
}
