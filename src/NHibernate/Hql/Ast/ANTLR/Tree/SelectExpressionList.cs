using System;
using System.Collections.Generic;
using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR.Util;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	/// <summary>
	/// Common behavior - a node that contains a list of select expressions.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public abstract class SelectExpressionList : HqlSqlWalkerNode 
	{
		protected SelectExpressionList(IToken token) : base(token)
		{
		}

		/// <summary>
		/// Returns an array of SelectExpressions gathered from the children of the given parent AST node.
		/// </summary>
		// Since v5.4
		[Obsolete("Use GetSelectExpressions method instead.")]
		public ISelectExpression[] CollectSelectExpressions()
		{
			return CollectSelectExpressions(false);
		}

		/// <summary>
		/// Returns an array of SelectExpressions gathered from the children of the given parent AST node.
		/// </summary>
		// Since v5.4
		[Obsolete("Use GetSelectExpressions method instead.")]
		public ISelectExpression[] CollectSelectExpressions(bool recurse)
		{
			return GetSelectExpressions(recurse, null).ToArray();
		}

		/// <summary>
		/// Gets a list of <see cref="ISelectExpression"/> gathered from the children of the given parent AST node.
		/// </summary>
		public List<ISelectExpression> GetSelectExpressions()
		{
			return GetSelectExpressions(false, null);
		}

		/// <summary>
		/// Gets a list of <see cref="ISelectExpression"/> gathered from the children of the given parent AST node.
		/// </summary>
		public List<ISelectExpression> GetSelectExpressions(
			bool recurse,
			Predicate<ISelectExpression> predicate)
		{
			// Get the first child to be considered.  Sub-classes may do this differently in order to skip nodes that
			// are not select expressions (e.g. DISTINCT).
			IASTNode firstChild = GetFirstSelectExpression();
			IASTNode parent = this;
			var list = new List<ISelectExpression>(parent.ChildCount);
			for (IASTNode n = firstChild; n != null; n = n.NextSibling)
			{
				if (recurse && n is ConstructorNode ctor)
				{
					for (IASTNode cn = ctor.GetChild(1); cn != null; cn = cn.NextSibling)
					{
						AddExpression(cn);
					}
				}

				AddExpression(n);
			}

			return list;

			void AddExpression(IASTNode n)
			{
				if (!(n is ISelectExpression se))
				{
					throw new InvalidOperationException(
						"Unexpected AST: " + n.GetType().FullName + " " + new ASTPrinter().ShowAsString(n, ""));
				}

				if (predicate?.Invoke(se) != false)
				{
					list.Add(se);
				}
			}
		}

		/// <summary>
		/// Returns the first select expression node that should be considered when building the array of select
		/// expressions.
		/// </summary>
		protected internal abstract IASTNode GetFirstSelectExpression();
	}
}
