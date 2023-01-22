using System;
using System.Linq;
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
	class CountNode : AggregateNode
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

		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i);
		}

		/// <inheritdoc />
		public override string[] SetScalarColumnText(int i, Func<int, int, string> aliasCreator)
		{
			return new[] {ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i, aliasCreator)};
		}
	}
}
