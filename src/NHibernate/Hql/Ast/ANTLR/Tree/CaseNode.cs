using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a case ... when .. then ... else ... end expression in a select.
	/// 
	/// Author: Gavin King
	/// Ported by: Steve Strong
	/// </summary>
	public class CaseNode : AbstractSelectExpression, ISelectExpression 
	{
		public CaseNode(IToken token) : base(token)
		{
		}

		public override IType  DataType
		{
			  get { return GetFirstThenNode().DataType; }
			  set { base.DataType = value; }
		}

		private ISelectExpression GetFirstThenNode() 
		{
			return (ISelectExpression) GetChild(0).GetChild(0).NextSibling;
		}

		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i );
		}
	}
}
