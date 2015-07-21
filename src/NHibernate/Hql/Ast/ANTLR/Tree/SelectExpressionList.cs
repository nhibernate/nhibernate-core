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
		public ISelectExpression[] CollectSelectExpressions()
		{
			return CollectSelectExpressions(false);
		}

		/// <summary>
		/// Returns an array of SelectExpressions gathered from the children of the given parent AST node.
		/// </summary>
		public ISelectExpression[] CollectSelectExpressions(bool recurse) 
		{
			// Get the first child to be considered.  Sub-classes may do this differently in order to skip nodes that
			// are not select expressions (e.g. DISTINCT).
			IASTNode firstChild = GetFirstSelectExpression();
			IASTNode parent = this;
			var list = new List<ISelectExpression>(parent.ChildCount);

			for (IASTNode n = firstChild; n != null; n = n.NextSibling)
			{
				if (recurse)
				{
					var ctor = n as ConstructorNode;

					if (ctor != null)
					{
						for (IASTNode cn = ctor.GetChild(1); cn != null; cn = cn.NextSibling)
						{
							var se = cn as ISelectExpression;
							if (se != null)
							{
								list.Add(se);
							}
						}
					}
					else
					{
						var se = n as ISelectExpression;
						if (se != null)
						{
							list.Add(se);
						}
						else
						{
							throw new InvalidOperationException("Unexpected AST: " + n.GetType().FullName + " " +
																new ASTPrinter().ShowAsString(n, ""));
						}
					}
				}
				else
				{
					var se = n as ISelectExpression;
					if (se != null)
					{
						list.Add(se);
					}
					else
					{
						throw new InvalidOperationException("Unexpected AST: " + n.GetType().FullName + " " +
															new ASTPrinter().ShowAsString(n, ""));
					}					
				}
			}

			return list.ToArray();
		}

		/// <summary>
		/// Returns the first select expression node that should be considered when building the array of select
		/// expressions.
		/// </summary>
		protected internal abstract IASTNode GetFirstSelectExpression();
	}
}
