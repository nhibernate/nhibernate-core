using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
    /// <summary>
    /// A custom token class for the HQL grammar.
    /// </summary>
	[CLSCompliant(false)]
	public class HqlToken : CommonToken
    {
        /// <summary>
        /// The previous token type.
        /// </summary>
        private int _previousTokenType;
 
        /// <summary>
        /// Public constructor
        /// </summary>
        public HqlToken(ICharStream input, int type, int channel, int start, int stop) : base(input, type, channel, start, stop)
        {
           CharPositionInLine = input.CharPositionInLine - (stop - start + 1);
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public HqlToken(IToken other)
            : base(other)
        {
            var hqlToken = other as HqlToken;

            if (hqlToken != null)
            {
                _previousTokenType = hqlToken._previousTokenType;
            }
        }

        /// <summary>
        /// Indicates if the token could be an identifier.
        /// </summary>
        public bool PossibleId
        {
            get { return HqlParser.possibleIds[Type]; }
        }

        /// <summary>
        /// Gets or Sets the type of the token, remembering the previous type on Sets.
        /// </summary>
        //public override int Type
        //{
        //    get
        //    {
        //        return base.Type;
        //    }
        //    set
        //    {
        //        _previousTokenType = Type;
        //        base.Type = value;
        //    }
        //}

        /// <summary>
        /// Returns the previous token type.
        /// </summary>
        private int PreviousType
        {
            get { return _previousTokenType; }
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>The debug string</returns>
        public override string ToString()
        {
            return "[\""
                    + Text
                    + "\",<" + Type + "> previously: <" + PreviousType + ">,line="
                    + Line + ",col="
                    + CharPositionInLine + ",possibleID=" + PossibleId + "]";
        }
    }
}