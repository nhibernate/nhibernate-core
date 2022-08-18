using System;
using System.Collections.Generic;
using System.Linq;
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
	[CLSCompliant(false)]
	public class CaseNode : AbstractSelectExpression, IExpectedTypeAwareNode
	{
		private IType _expectedType;

		public CaseNode(IToken token) : base(token)
		{
		}

		public override IType  DataType
		{
			get
			{
				if (ExpectedType != null)
					return ExpectedType;

				foreach (var node in GetResultNodes())
				{
					if (node is ISelectExpression select && !(node is ParameterNode))
						return select.DataType;
				}

				throw new HibernateException("Unable to determine data type of CASE statement.");
			}
			set { base.DataType = value; }
		}

		public IEnumerable<IASTNode> GetResultNodes()
		{
			for (int i = 0; i < ChildCount; i++)
			{
				IASTNode whenOrElseClause = GetChild(i);
				if (whenOrElseClause.Type == HqlParser.WHEN)
				{
					// WHEN Child(0) THEN Child(1)
					yield return whenOrElseClause.GetChild(1);
				}
				else if (whenOrElseClause.Type == HqlParser.ELSE)
				{
					// ELSE Child(0)
					yield return whenOrElseClause.GetChild(0);
				}
				else
				{
					throw new HibernateException("Was expecting a WHEN or ELSE, but found a: " + whenOrElseClause.Text);
				}
			}
		}

		// Since v5.4
		[Obsolete("Use overload with aliasCreator parameter instead.")]
		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i );
		}

		/// <inheritdoc />
		public override string[] SetScalarColumnText(int i, Func<int, int, string> aliasCreator)
		{
			return new[] {ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i, aliasCreator)};
		}

		public IType ExpectedType
		{
			get => _expectedType;
			set
			{
				_expectedType = value;
				foreach (var node in GetResultNodes().OfType<IExpectedTypeAwareNode>())
				{
					node.ExpectedType = ExpectedType;
				}
			}
		}
	}
}
