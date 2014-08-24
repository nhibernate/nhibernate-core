using System;
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
	public class CaseNode : AbstractSelectExpression, ISelectExpression 
	{
		public CaseNode(IToken token) : base(token)
		{
		}

		public override IType  DataType
		{
			get
			{
				for (int i = 0; i < ChildCount; i++)
				{
					IASTNode whenOrElseClause = GetChild(i);
					if (whenOrElseClause.Type == HqlParser.WHEN)
					{
						// WHEN Child(0) THEN Child(1)
						IASTNode thenClause = whenOrElseClause.GetChild(1);
						if (thenClause is ISelectExpression)
						{
							if (!(thenClause is ParameterNode))
							{
								return (thenClause as ISelectExpression).DataType;
							}
						}
					}
					else if (whenOrElseClause.Type == HqlParser.ELSE)
					{
						// ELSE Child(0)
						IASTNode elseClause = whenOrElseClause.GetChild(0);
						if (elseClause is ISelectExpression)
						{
							if (!(elseClause is ParameterNode))
							{
								return (elseClause as ISelectExpression).DataType;
							}
						}
					}
					else
					{
						throw new HibernateException("Was expecting a WHEN or ELSE, but found a: " + whenOrElseClause.Text);
					}
				}
				throw new HibernateException("Unable to determine data type of CASE statement.");
			}
			set { base.DataType = value; }
		}

		public override void SetScalarColumnText(int i)
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i );
		}
	}
}
