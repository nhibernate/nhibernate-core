using System;
using Antlr.Runtime;


namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Defines a top-level AST node representing an HQL delete statement. 
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class DeleteStatement : AbstractRestrictableStatement
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(DeleteStatement));

		public DeleteStatement(IToken token) : base(token) {}

		public override bool NeedsExecutor
		{
			get { return true; }
		}

		public override int StatementType
		{
			get { return HqlSqlWalker.DELETE; }
		}

		protected override IInternalLogger GetLog()
		{
			return Log;
		}

		protected override int GetWhereClauseParentTokenType()
		{
			return HqlSqlWalker.FROM;
		}
	}
}