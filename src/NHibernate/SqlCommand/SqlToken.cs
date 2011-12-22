using System;

namespace NHibernate.SqlCommand
{
    /// <summary>
    /// A SQL query token as returned by <see cref="SqlTokenizer"/>
    /// </summary>
    public class SqlToken
    {
        private readonly SqlTokenType _tokenType;
        private readonly SqlString _sql;
        private readonly int _sqlIndex;
        private readonly int _length;
        private string _value;

        public SqlToken(SqlTokenType tokenType, SqlString sql, int sqlIndex, int length)
        {
            _tokenType = tokenType;
            _sql = sql;
            _sqlIndex = sqlIndex;
            _length = length;
        }

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
            get { return _sqlIndex; }
        }

        /// <summary>
        /// Number of characters in this token.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        public string Value
        {
            get { return _value ?? (_value = _sql.ToString(_sqlIndex, _length)); }
        }

        #endregion

        #region Instance methods

        public virtual bool Equals(string value, StringComparison stringComparison)
        {
            return value != null
                && value.Equals(this.Value, stringComparison);
        }

        public override string ToString()
        {
            return this.Value;
        }

        #endregion
    }
}
