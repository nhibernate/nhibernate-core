using System;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	class ASTPrinter
	{
		public string ShowAsString(IASTNode ast, string header)
		{
            return ast.ToStringTree();
		}
	}
}
