using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
    public partial class HqlLexer
    {
        public override IToken Emit()
        {
            HqlToken t = new HqlToken(input, 
                                      state.type, 
                                      state.channel,
                                      state.tokenStartCharIndex,
                                      CharIndex - 1);

            t.Line = state.tokenStartLine;
            t.Text = state.text;

            Emit(t);
            return t;
        }
    }
}
