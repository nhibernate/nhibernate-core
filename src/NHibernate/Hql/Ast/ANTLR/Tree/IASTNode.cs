using System;
using System.Collections.Generic;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public interface IASTNode : IEnumerable<IASTNode>
	{
		bool IsNil { get; }
		int Type { get; set; }
		string Text { get; set; }
		int Line { get; }
		int CharPositionInLine { get; }
		int ChildCount { get; }
		int ChildIndex { get; }
		IASTNode Parent { get; set; }
		IASTNode NextSibling { get; }
		IASTNode GetChild(int index);
	    void SetChild(int index, IASTNode newChild);
		IASTNode AddChild(IASTNode childNode);
		IASTNode InsertChild(int index, IASTNode child);
		IASTNode AddSibling(IASTNode newSibling);
		void ClearChildren();
		void RemoveChild(IASTNode child);
		void AddChildren(IEnumerable<IASTNode> children);
		void AddChildren(params IASTNode[] children);

		IASTNode DupNode();

		// TODO - not sure we need this...
		IToken Token { get; }
		
		string ToStringTree();
	}
}
