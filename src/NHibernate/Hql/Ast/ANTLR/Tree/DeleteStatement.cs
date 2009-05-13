using System;
using Antlr.Runtime;
using log4net;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Defines a top-level AST node representing an HQL delete statement. 
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class DeleteStatement : AbstractRestrictableStatement
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(DeleteStatement));

		public DeleteStatement(IToken token) : base(token) {}

		public override bool NeedsExecutor
		{
			get { return true; }
		}

		public override int StatementType
		{
			get { return HqlSqlWalker.DELETE; }
		}

		protected override ILog GetLog()
		{
			return Log;
		}

		protected override int GetWhereClauseParentTokenType()
		{
			return HqlSqlWalker.FROM;
		}
	}
}