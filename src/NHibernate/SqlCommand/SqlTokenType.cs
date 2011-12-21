namespace NHibernate.SqlCommand
{
    /// <summary>
    /// <see cref="SqlToken"/> token types.
    /// </summary>
    public enum SqlTokenType
    {
        /// <summary>
        /// Whitespace
        /// </summary>
        Whitespace,

        /// <summary>
        /// Single line comment (preceeded by --) or multi-line comment (terminated by /* and */)
        /// </summary>
        Comment,

        /// <summary>
        /// Quoted text, either single quoted, double quoted or bracketed.
        /// </summary>
        QuotedText,

        /// <summary>
        /// List separator, the ',' character.
        /// </summary>
        ListSeparator,

        /// <summary>
        /// Begin of an expression block, consisting of a '(' character.
        /// </summary>
        BlockBegin,

        /// <summary>
        /// End of an expression block, consisting of a ')' character.
        /// </summary>
        BlockEnd,

        /// <summary>
        /// A query parameter.
        /// </summary>
        Parameter,

        /// <summary>
        /// Non-quoted text fragment
        /// </summary>
        Text
    }
}
