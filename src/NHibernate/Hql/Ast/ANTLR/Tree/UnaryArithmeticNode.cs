using System;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class UnaryArithmeticNode : AbstractSelectExpression, IUnaryOperatorNode 
	{
		public UnaryArithmeticNode(IToken token) : base(token)
		{
		}

		public override IType  DataType
		{
			get 
			{ 
				return ( ( SqlNode ) Operand ).DataType;
			}
			set 
			{ 
				base.DataType = value;
			}
		}

		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i );
		}

		public void Initialize() 
		{
			// nothing to do; even if the operand is a parameter, no way we could
			// infer an appropriate expected type here
		}

		public IASTNode Operand
		{
			get { return GetChild(0); }
		}
	}
}
