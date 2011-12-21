namespace NHibernate.SqlCommand
{
    using System;

    /// <summary>
    /// Represents part of a <see cref="SqlString"/>. This class is immutable.
    /// </summary>
    public struct SqlPart
    {
        private readonly object _content;
        private readonly int _offset;
        private readonly int _sqlIndex;

        internal SqlPart(object content, int index)
            : this(content, index, 0)
        {}

        private SqlPart(object content, int sqlIndex, int offset)
        {
            _content = content;
            _sqlIndex = sqlIndex;
            _offset = offset;
        }

        /// <summary>
        /// The content of this <see cref="SqlString"/> part. This is either a <see cref="string"/>
        /// or a <see cref="Parameter"/>.
        /// </summary>
        public object Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Index at which this <see cref="SqlPart"/> occurs within the <see cref="SqlString"/>.
        /// </summary>
        public int SqlIndex
        {
            get { return _sqlIndex; }
        }

        /// <summary>
        /// Number of characters in this <see cref="SqlPart"/>.
        /// </summary>
        public int SqlLength
        {
            get { return SqlString.LengthOfPart(_content) - _offset; }
        }

        public char this[int offset]
        {
            get
            {
                var stringPart = _content as string;
                return stringPart != null
                    ? stringPart[_offset + offset]
                    : default(char);
            }
        }

        internal int OffsetOf(string value, int startOffset, int length, StringComparison stringComparison)
        {
            var stringPart = _content as string;
            return stringPart != null
                ? stringPart.IndexOf(value, _offset + startOffset, length, stringComparison)
                : -1;
        }

        /// <summary>
        /// Returns a substring from this part.
        /// </summary>
        /// <param name="startOffset">Offset (in characters) from begin of this 
        /// part at which substring starts from which terms are to be enumerated.</param>
        /// <param name="length">Length of substring from which terms are to be 
        /// enumerated.</param>
        /// <returns>The substring of this part that starts at <paramref name="startOffset"/> 
        /// and has the given <paramref name="length"/>.
        /// </returns>
        internal string Substring(int startOffset, int length)
        {
            var stringPart = _content as string;
            return stringPart != null
                ? stringPart.Substring(_offset + startOffset, length)
                : null;
        }

        internal SqlPart Subpart(int startOffset)
        {
            return new SqlPart(_content, _sqlIndex + startOffset, _offset + startOffset);
        }

        public override string ToString()
        {
            return _content != null 
                ? _content.ToString().Substring(_offset) 
                : string.Empty;
        }
    }
}
