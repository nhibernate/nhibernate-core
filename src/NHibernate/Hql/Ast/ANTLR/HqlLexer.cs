using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	[CLSCompliant(false)]
	public partial class HqlLexer
	{
		public override IToken Emit()
		{
			var t = new HqlToken(input,
																state.type,
																state.channel,
																state.tokenStartCharIndex,
																CharIndex - 1) {Line = state.tokenStartLine, Text = state.text};

			Emit(t);
			return t;
		}
	}
}
