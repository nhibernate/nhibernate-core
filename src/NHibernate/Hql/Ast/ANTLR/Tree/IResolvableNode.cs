using System;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// The contract for expression sub-trees that can resolve themselves.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public interface IResolvableNode
	{
		/// <summary>
		/// Does the work of resolving an identifier or a dot
		/// </summary>
		void Resolve(bool generateJoin, bool implicitJoin, string classAlias, IASTNode parent);

		/// <summary>
		/// Does the work of resolving an identifier or a dot, but without a parent node
		/// </summary>
		void Resolve(bool generateJoin, bool implicitJoin, string classAlias);

		/// <summary>
		/// Does the work of resolving an identifier or a dot, but without a parent node or alias
		/// </summary>
		void Resolve(bool generateJoin, bool implicitJoin);

		/// <summary>
		/// Does the work of resolving inside of the scope of a function call
		/// </summary>
		void ResolveInFunctionCall(bool generateJoin, bool implicitJoin);

		/// <summary>
		/// Does the work of resolving an an index [].
		/// </summary>
		void ResolveIndex(IASTNode parent);
	}
}
