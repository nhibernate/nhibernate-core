using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Implementation of OrderByClause.
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class OrderByClause : HqlSqlWalkerNode 
	{
		public OrderByClause(IToken token) : base(token)
		{
		}

		public void AddOrderFragment(string orderByFragment) 
		{
			AddChild(ASTFactory.CreateNode(HqlSqlWalker.SQL_TOKEN, orderByFragment));
		}
	}
}
