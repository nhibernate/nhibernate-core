using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR.Tree
{
	[CLSCompliant(false)]
	public class ASTErrorNode : ASTNode
	{
		public ASTErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e) : base(start)
		{
			Input = input;
			Stop = stop;
			RecognitionException = e;
		}

		public ITokenStream Input { get; private set; }
		public IToken Stop { get; private set; }
		public RecognitionException RecognitionException { get; private set; }
	}
}
