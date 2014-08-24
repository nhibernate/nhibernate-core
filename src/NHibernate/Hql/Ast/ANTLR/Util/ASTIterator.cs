using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Hql.Ast.ANTLR.Tree;

namespace NHibernate.Hql.Ast.ANTLR.Util
{
	/// <summary>
	/// Depth first iteration of an ANTLR AST.
	/// Author: josh
	/// Ported by: Steve Strong
	/// </summary>
	[CLSCompliant(false)]
	public class ASTIterator : IEnumerable<IASTNode>
	{
		private readonly Stack<IASTNode> _stack = new Stack<IASTNode>();
		private IASTNode _current;

		public ASTIterator(IASTNode tree)
		{
			_current = tree;
		}

		#region IEnumerable<IASTNode> Members

		public IEnumerator<IASTNode> GetEnumerator()
		{
			Down();

			while (_current != null)
			{
				yield return _current;

				_current = _current.NextSibling;

				if (_current == null)
				{
					if (_stack.Count > 0)
					{
						_current = _stack.Pop();
					}
				}
				else
				{
					Down();
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		private void Down()
		{
			while (_current != null && _current.ChildCount > 0)
			{
				_stack.Push(_current);
				_current = _current.GetChild(0);
			}
		}
	}
}