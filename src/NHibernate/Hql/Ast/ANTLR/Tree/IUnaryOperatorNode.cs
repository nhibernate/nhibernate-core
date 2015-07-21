using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Contract for nodes representing unary operators.
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface IUnaryOperatorNode : IOperatorNode
	{
		/// <summary>
		/// Retrieves the node representing the operator's single operand.
		/// </summary>
		IASTNode Operand { get; }
	}
}