using System.Linq;
using Antlr.Runtime;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a COUNT expression in a select.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	class CountNode : AggregateNode, ISelectExpression
	{
		public CountNode(IToken token) : base(token)
		{
		}

		public override IType DataType
		{
			get
			{
				return SessionFactoryHelper.FindFunctionReturnType(Text, Enumerable.Empty<IASTNode>());
			}
			set
			{
				base.DataType = value;
			}
		}
	}
}
