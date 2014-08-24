using System;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;
using NHibernate.Type;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Represents a literal.
	/// 
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class LiteralNode : AbstractSelectExpression
	{
		public LiteralNode(IToken token) : base(token)
		{
		}

		public override void SetScalarColumnText(int i) 
		{
			ColumnHelper.GenerateSingleScalarColumn(ASTFactory, this, i );
		}

		public override IType DataType
		{
			get
			{
				switch (Type)
				{
					case HqlSqlWalker.NUM_INT:
						return NHibernateUtil.Int32;
					case HqlSqlWalker.NUM_LONG:
						return NHibernateUtil.Int64;
					case HqlSqlWalker.NUM_FLOAT:
                        return NHibernateUtil.Single;
                    case HqlSqlWalker.NUM_DOUBLE:
                        return NHibernateUtil.Double;
                    case HqlSqlWalker.NUM_DECIMAL:
				        return NHibernateUtil.Decimal;
					case HqlSqlWalker.QUOTED_String:
						return NHibernateUtil.String;
					case HqlSqlWalker.TRUE:
					case HqlSqlWalker.FALSE:
						return NHibernateUtil.Boolean;
					default:
						return null;
				}
			}
			set
			{
				base.DataType = value;
			}
		}
	}
}
