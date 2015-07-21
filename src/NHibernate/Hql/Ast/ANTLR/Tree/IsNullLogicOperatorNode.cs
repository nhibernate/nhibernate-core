using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a 'is null' check.
	/// </summary>
	[CLSCompliant(false)]
	public class IsNullLogicOperatorNode : AbstractNullnessCheckNode
	{
		public IsNullLogicOperatorNode(IToken token) : base(token) {}

		protected override int ExpansionConnectorType
		{
			get { return HqlSqlWalker.AND; }
		}

		protected override string ExpansionConnectorText
		{
			get { return "AND"; }
		}
	}
}