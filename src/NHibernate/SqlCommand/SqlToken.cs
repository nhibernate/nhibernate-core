namespace NHibernate.SqlCommand
{
    using System;

    /// <summary>
    /// A SQL query token as returned by <see cref="SqlTokenizer"/>
    /// </summary>
    public class SqlToken
    {
        private readonly SqlTokenType _tokenType;
        private readonly SqlPart _part;
        private readonly int _offset;

        protected SqlToken(SqlTokenType tokenType, SqlPart part, int offset)
        {
            _tokenType = tokenType;
            _part = part;
            _offset = offset;
        }

        #region Static factory methods

        public static SqlToken Whitespace(SqlPart part, int offset, int length)
        {
            return new SqlContentToken(SqlTokenType.Whitespace, part, offset, length);
        }

        public static SqlToken Comment(SqlPart part, int offset, int length)
        {
            return new SqlContentToken(SqlTokenType.Comment, part, offset, length);
        }

        public static SqlToken QuotedText(SqlPart part, int offset, int length)
        {
            return new SqlContentToken(SqlTokenType.QuotedText, part, offset, length);
        }

        public static SqlToken Parameter(SqlPart part)
        {
            return new SqlToken(SqlTokenType.Parameter, part, 0);
        }

        public static SqlToken Text(SqlPart part, int offset, int length)
        {
            return new SqlContentToken(SqlTokenType.Text, part, offset, length);
        }

        public static SqlToken BlockBegin(SqlPart part, int offset)
        {
            return new SqlToken(SqlTokenType.BlockBegin, part, offset);
        }

        public static SqlToken BlockEnd(SqlPart part, int offset)
        {
            return new SqlToken(SqlTokenType.BlockEnd, part, offset);
        }

        public static SqlToken ListSeparator(SqlPart part, int offset)
        {
            return new SqlToken(SqlTokenType.ListSeparator, part, offset);
        }

        #endregion

        #region Properties

        public SqlTokenType TokenType
        {
            get { return _tokenType; }
        }

        /// <summary>
        /// Position at which this token occurs in a <see cref="SqlString"/>.
        /// </summary>
        public int SqlIndex
        {
            get { return this.Part.SqlIndex + _offset; }
        }

        /// <summary>
        /// Number of characters in this token.
        /// </summary>
        public virtual int SqlLength
        {
            get { return 1; }
        }

        /// <summary>
        /// The <see cref="SqlPart"/> that contains this token.
        /// </summary>
        protected SqlPart Part
        {
            get { return _part; }
        }

        /// <summary>
        /// The position within <see cref="Part"/> at which this token occurs.
        /// </summary>
        protected int Offset
        {
            get { return _offset; }
        }

        #endregion

        #region Instance methods

        public int IndexOf(string value, StringComparison stringComparison)
        {
            var offset = _part.OffsetOf(value, _offset, this.SqlLength, stringComparison);
            return offset >= 0
                ? _part.SqlIndex + offset
                : -1;
        }

        public virtual bool Equals(char value)
        {
            return _part[_offset] == value;
        }

        public virtual bool Equals(string value, StringComparison stringComparison)
        {
            return value != null 
                && value.Length == 1 
                && value.Equals(_part.Substring(_offset, 1), stringComparison);
        }

        public virtual bool StartsWith(string value, StringComparison stringComparison)
        {
            return Equals(value, stringComparison);
        }

        #endregion

        #region Inner classes

        private class SqlContentToken : SqlToken
        {
            private readonly int _length;

            public SqlContentToken(SqlTokenType tokenType, SqlPart part, int offset, int length)
                : base(tokenType, part, offset)
            {
                _length = length;
            }

            public override int SqlLength
            {
                get { return _length; }
            }

            public override string ToString()
            {
                return this.Part.Substring(_offset, _length);
            }

            public override bool Equals(char value)
            {
                return _length == 1 && _part[_offset] == value;
            }

            public override bool Equals(string value, StringComparison stringComparison)
            {
                return value != null 
                    && value.Length == _length 
                    && _part.OffsetOf(value, _offset, _length, stringComparison) == _offset;
            }

            public override bool StartsWith(string value, StringComparison stringComparison)
            {
                return value != null
                    && value.Length <= _length
                    && _part.OffsetOf(value, _offset, value.Length, stringComparison) == _offset;
            }
        }

        #endregion
    }
}
