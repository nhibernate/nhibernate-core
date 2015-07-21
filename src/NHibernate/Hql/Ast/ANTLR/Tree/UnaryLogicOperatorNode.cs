using System;
using Antlr.Runtime;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a unary operator node.
	/// 
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class UnaryLogicOperatorNode : HqlSqlWalkerNode, IUnaryOperatorNode 
	{
		public UnaryLogicOperatorNode(IToken token) : base(token)
		{
		}

		public IASTNode Operand
		{
			get { return GetChild(0); }
		}

		public virtual void Initialize()
		{
			// nothing to do; even if the operand is a parameter, no way we could
			// infer an appropriate expected type here
		}

		public override IType DataType
		{
			get
			{
				// logic operators by definition resolve to booleans
				return NHibernateUtil.Boolean;
			}
			set
			{
				base.DataType = value;
			}
		}
	}
}
