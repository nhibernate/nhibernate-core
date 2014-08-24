using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// IsNotNullLogicOperatorNode implementation
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class IsNotNullLogicOperatorNode : AbstractNullnessCheckNode
	{
		public IsNotNullLogicOperatorNode(IToken token) : base(token)
		{
		}

		protected override int ExpansionConnectorType
		{
			get { return HqlSqlWalker.OR; }
		}

		protected override string ExpansionConnectorText
		{
			get { return "OR"; }
		}
	}
}
