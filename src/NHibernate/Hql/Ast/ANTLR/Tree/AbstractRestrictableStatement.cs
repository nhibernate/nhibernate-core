using System;
using Antlr.Runtime;

using NHibernate.Hql.Ast.ANTLR.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public abstract class AbstractRestrictableStatement : AbstractStatement, IRestrictableStatement
	{
		private FromClause _fromClause;
		private IASTNode _whereClause;

		protected AbstractRestrictableStatement(IToken token) : base(token)
		{
		}

		protected abstract IInternalLogger GetLog();
		protected abstract int GetWhereClauseParentTokenType();

		public FromClause FromClause
		{
			get
			{
				if (_fromClause == null)
				{
					_fromClause = (FromClause)ASTUtil.FindTypeInChildren(this, HqlSqlWalker.FROM);
				}
				return _fromClause;
			}
		}

		public bool HasWhereClause
		{
			get
			{
				IASTNode whereClause = ASTUtil.FindTypeInChildren(this, HqlSqlWalker.WHERE);
				return whereClause != null && whereClause.ChildCount > 0;
			}
		}

		public IASTNode WhereClause
		{
			get
			{
				if (_whereClause == null)
				{
					_whereClause = ASTUtil.FindTypeInChildren(this, HqlSqlWalker.WHERE);

					// If there is no WHERE node, make one.
					if (_whereClause == null)
					{
						GetLog().Debug("getWhereClause() : Creating a new WHERE clause...");

						_whereClause = Walker.ASTFactory.CreateNode(HqlSqlWalker.WHERE, "WHERE");

						// inject the WHERE after the parent
						IASTNode parent = ASTUtil.FindTypeInChildren(this, GetWhereClauseParentTokenType());
						parent.AddSibling(_whereClause);
					}
				}

				return _whereClause;
			}
		}
	}
}
