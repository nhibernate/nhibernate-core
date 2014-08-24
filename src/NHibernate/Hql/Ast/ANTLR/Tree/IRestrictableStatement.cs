using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Type definition for Statements which are restrictable via a where-clause (and
	/// thus also having a from-clause).
	/// Author: Steve Ebersole
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface IRestrictableStatement : IStatement
	{
		/// <summary>
		/// Retreives the from-clause in effect for this statement; could be null if the from-clause
		/// has not yet been parsed/generated.
		/// </summary>
		FromClause FromClause { get; }

		/// <summary>
		/// Does this statement tree currently contain a where clause?
		/// Returns True if a where-clause is found in the statement tree and
		/// that where clause actually defines restrictions; false otherwise.
		/// </summary>
		bool HasWhereClause { get; }

		/// <summary>
		/// Retreives the where-clause defining the restriction(s) in effect for
		/// this statement.
		/// Note that this will generate a where-clause if one was not found, so caution
		/// needs to taken prior to calling this that restrictions will actually exist
		/// in the resulting statement tree (otherwise "unexpected end of subtree" errors
		/// might occur during rendering).
		/// </summary>
		IASTNode WhereClause { get; }
	}
}