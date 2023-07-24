using System;
using Antlr.Runtime;

namespace NHibernate.Hql.Ast.ANTLR
{
	public static class TokenExtensions
	{
		/// <summary>
		/// Indicates if the token could be an identifier.
		/// </summary>
		/// <param name="token"></param>
		public static bool IsPossibleId(this IToken token)
		{
			var type = token.Type;
			return type >= 0 && type < HqlParser.possibleIds.Length && HqlParser.possibleIds[type];
		}
	}

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
        // Since 5.5
        [Obsolete("Use IsPossibleId extension method instead.")]
        public bool PossibleId => this.IsPossibleId();

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
                    + CharPositionInLine + ",possibleID=" + this.IsPossibleId() + "]";
        }
    }
}
