using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a COUNT expression in a select.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	class CountNode : AbstractSelectExpression, ISelectExpression
	{
		public CountNode(IToken token) : base(token)
		{
		}


		public override IType DataType
		{
			get
			{
				return SessionFactoryHelper.FindFunctionReturnType(Text, null);
			}
			set
			{
				base.DataType = value;
			}
		}
		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}
	}
}
