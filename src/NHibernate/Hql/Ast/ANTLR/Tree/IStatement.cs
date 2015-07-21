using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Common interface modeling the different HQL statements (i.e., INSERT, UPDATE, DELETE, SELECT).
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface IStatement
	{
		/// <summary>
		/// The "phase 2" walker which generated this statement tree.
		/// </summary>
		HqlSqlWalker Walker { get; }

		/// <summary>
		/// The main token type representing the type of this statement.
		/// </summary>
		int StatementType { get; }

		/// <summary>
		/// Does this statement require the StatementExecutor?
		/// Essentially, at the JDBC level, does this require an executeUpdate()?
		/// </summary>
		bool NeedsExecutor { get; }
	}
}