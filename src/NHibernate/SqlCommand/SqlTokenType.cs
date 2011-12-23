namespace NHibernate.SqlCommand
{
    using System;

    /// <summary>
    /// <see cref="SqlToken"/> token types.
    /// </summary>
    [Flags]
    public enum SqlTokenType
    {
        /// <summary>
        /// Whitespace
        /// </summary>
        Whitespace = 0x1,

        /// <summary>
        /// Single line comment (preceeded by --) or multi-line comment (terminated by /* and */)
        /// </summary>
        Comment = 0x2,

		/// <summary>
		/// Keyword, operator, unquoted identifier or unquoted literal
		/// </summary>
		UnquotedText = 0x4,

		/// <summary>
		/// Quoted identifier, surrounded by double quotes or straight brackets.
		/// </summary>
		QuotedIdentifier = 0x8,
		
		/// <summary>
        /// Quoted text, surrounded by single quotes.
        /// </summary>
        QuotedText = 0x10,

        /// <summary>
        /// List separator, the ',' character.
        /// </summary>
        ListSeparator = 0x20,

        /// <summary>
        /// Begin of an expression block, consisting of a '(' character.
        /// </summary>
        BlockBegin = 0x40,

        /// <summary>
        /// End of an expression block, consisting of a ')' character.
        /// </summary>
        BlockEnd = 0x80,

        /// <summary>
        /// A query parameter.
        /// </summary>
        Parameter = 0x100,

		/// <summary>
		/// Tokens for begin or end of expression blocks.
		/// </summary>
		AllBlockBeginOrEnd = BlockBegin | BlockEnd,

		/// <summary>
		/// Includes all token types except whitespace or comments
		/// </summary>
		AllExceptWhitespaceOrComment = AllExceptWhitespace & ~Comment, 

		/// <summary>
		/// Includes all token types except whitespace
		/// </summary>
		AllExceptWhitespace = All & ~Whitespace,
		
		/// <summary>
		/// Includes all token types
		/// </summary>
		All = Whitespace | Comment | QuotedText | ListSeparator | BlockBegin | BlockEnd | Parameter | UnquotedText | QuotedIdentifier,
    }
}
