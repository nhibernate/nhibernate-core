using System;
using System.Collections.Generic;

namespace NHibernate.SqlCommand
{
    public abstract class SelectQuery
    {
        private readonly SqlString _sql;
        private readonly IEnumerator<SqlToken> _tokenEnum;
        private SqlToken _selectClause;
        private SqlToken _columnsBegin;
        private SqlToken _fromClause;
        private SqlToken _orderByClause;
        private SqlToken _ordersBegin;

        private IList<ColumnDefinition> _columns;
        private IList<OrderDefinition> _orders;

        protected SelectQuery(SqlString sql)
        {
            if (sql == null) throw new ArgumentNullException("sql");
            _sql = sql;
            _tokenEnum = new SqlTokenizer(sql).GetEnumerator();
            _tokenEnum.MoveNext();
        }

        public abstract SqlString GetLimitString(SqlString offset, SqlString limit);

        public SqlString Sql
        {
            get { return _sql; }
        }

        public int SelectIndex
        {
            get
            {
                return TryParseUntilBeginOfColumnDefinitions()
                    ? _selectClause.SqlIndex
                    : -1;
            }
        }

        public int ColumnsBeginIndex
        {
            get
            {
                return TryParseUntilBeginOfColumnDefinitions() && _columnsBegin != null
                    ? _columnsBegin.SqlIndex
                    : -1;

            }
        }

        public int FromIndex
        {
            get
            {
                return TryParseUntilFromClause() && _fromClause != null
                    ? _fromClause.SqlIndex
                    : -1;
            }
        }

        public int OrderByIndex
        {
            get
            {
                return TryParseUntilBeginOfOrderDefinitions() && _orderByClause != null
                    ? _orderByClause.SqlIndex
                    : -1;
            }
        }

        protected IList<ColumnDefinition> ColumnDefinitions
        {
            get
            {
                TryParseUntilFromClause();
                return _columns;
            }
        }

        protected IList<OrderDefinition> OrderDefinitions
        {
            get
            {
                ParseUntilEnd();
                return _orders;
            }
        }

        private bool TryParseUntilBeginOfColumnDefinitions()
        {
            if (_columnsBegin != null) return true;

            if (TryParseUntil("select"))
            {
                _selectClause = _tokenEnum.Current;
                if (_tokenEnum.MoveNext())
                {
                    _columnsBegin = _tokenEnum.Current;
                    if (TryParseWhitespaceAndComments() && _tokenEnum.Current.Equals("distinct", StringComparison.InvariantCultureIgnoreCase))
                    {
                        _tokenEnum.MoveNext();
                        _columnsBegin = _tokenEnum.Current;
                    }
                }
                return true;
            }

            return false;
        }

        private bool TryParseUntilFromClause()
        {
            if (_columns == null)
            {
                _columns = TryParseUntilBeginOfColumnDefinitions()
                    ? ParseColumnDefinitions()
                    : new ColumnDefinition[0];
                _fromClause = _tokenEnum.Current;
            }
            return _columns.Count > 0;
        }

        private bool TryParseUntilBeginOfOrderDefinitions()
        {
            if (_ordersBegin != null) return true;
            if (!this.TryParseUntilFromClause()) return false;

            while (TryParseUntil("order"))
            {
                _orderByClause = _tokenEnum.Current;
                if (_tokenEnum.MoveNext() 
                    && TryParseWhitespaceAndComments()
                    && _tokenEnum.Current.Equals("by", StringComparison.InvariantCultureIgnoreCase))
                {
                    _tokenEnum.MoveNext();
                    _ordersBegin = _tokenEnum.Current;
                    return true;
                }
            }

            return false;
        }

        private void ParseUntilEnd()
        {
            if (_orders == null)
            {
                _orders = TryParseUntilBeginOfOrderDefinitions()
                    ? ParseOrderDefinitions()
                    : new OrderDefinition[0];
            }
        }

        private IList<ColumnDefinition> ParseColumnDefinitions()
        {
            var columns = new List<ColumnDefinition>();

            int blockLevel = 0;
            SqlToken columnBeginToken = null;
            SqlToken columnEndToken = null;
            SqlToken columnAliasToken = null;

            do
            {
                var token = _tokenEnum.Current;
                if (token == null) break;
                if (token.TokenType == SqlTokenType.Whitespace) continue;

                columnBeginToken = columnBeginToken ?? token;

                switch (token.TokenType)
                {
                    case SqlTokenType.BlockBegin:
                        blockLevel++;
                        break;

                    case SqlTokenType.BlockEnd:
                        blockLevel--;
                        break;

                    case SqlTokenType.Text:
                        if (blockLevel != 0) break;

                        if (token.Equals("from", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (columnAliasToken != null)
                            {
                                columns.Add(ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken));
                            }
                            return columns;
                        }

                        if (token.Equals("as", StringComparison.InvariantCultureIgnoreCase))
                        {
                            columnEndToken = token;
                        }

                        columnAliasToken = token;
                        break;
                    
                    case SqlTokenType.QuotedText:
                        if (blockLevel != 0) break;

                        columnAliasToken = token;
                        break;
                    
                    case SqlTokenType.ListSeparator:
                        if (blockLevel != 0) break;

                        if (columnAliasToken != null)
                        {
                            columns.Add(ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken));
                        }
                        columnBeginToken = columnEndToken = columnAliasToken = null;
                        break;
                }
            } while (_tokenEnum.MoveNext());

            if (columnAliasToken != null)
            {
                columns.Add(ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken)); 
            }
            
            return columns;
        }

        private ColumnDefinition ParseColumnDefinition(SqlToken beginToken, SqlToken endToken, SqlToken aliasToken)
        {
            var name = beginToken == endToken 
                ? beginToken.ToString() 
                : null;
            
            var alias = aliasToken.ToString();
            var dotIndex = alias.LastIndexOf('.');
            alias = dotIndex >= 0
                ? alias.Substring(dotIndex + 1)
                : alias;

            var sqlIndex = beginToken.SqlIndex;
            var sqlLength = (endToken != null ? endToken.SqlIndex : this.Sql.Length) - beginToken.SqlIndex;

            return new ColumnDefinition(sqlIndex, sqlLength, name, alias);
        }

        private ColumnDefinition ParseColumnDefinition(SqlToken beginToken, SqlToken endToken, string alias)
        {
            var sqlIndex = beginToken.SqlIndex;
            var sqlLength = (endToken != null ? endToken.SqlIndex : this.Sql.Length) - beginToken.SqlIndex;

            return new ColumnDefinition(sqlIndex, sqlLength, null, alias);
        }

        private IList<OrderDefinition> ParseOrderDefinitions()
        {
            var orders = new List<OrderDefinition>();

            int blockLevel = 0;
            int orderExprTokenCount = 0;
            SqlToken orderBeginToken = null;
            SqlToken columnNameToken = null;
            SqlToken directionToken = null;

            do
            {
                var token = _tokenEnum.Current;
                if (token == null) break;
                if (token.TokenType == SqlTokenType.Whitespace) continue;

                orderBeginToken = orderBeginToken ?? token;

                switch (token.TokenType)
                {
                    case SqlTokenType.BlockBegin:
                        blockLevel++;
                        break;

                    case SqlTokenType.BlockEnd:
                        blockLevel--;
                        if (blockLevel == 0) orderExprTokenCount++;
                        break;

                    case SqlTokenType.Text:
                        if (blockLevel != 0) break;

                        if (orderExprTokenCount++ == 0)
                        {
                            columnNameToken = token;
                            break;
                        }

                        if (token.Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                            || token.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (directionToken == null) orderExprTokenCount--;
                            directionToken = token;
                        }
                        break;

                    case SqlTokenType.QuotedText:
                        if (blockLevel != 0) break;

                        if (orderExprTokenCount++ == 0) columnNameToken = token;
                        break;

                    case SqlTokenType.ListSeparator:
                        if (blockLevel != 0) break;

                        orders.Add(ParseOrderDefinition(orderBeginToken, token, 
                            orderExprTokenCount == 1 ? columnNameToken : null, directionToken));
                        orderBeginToken = columnNameToken = directionToken = null;
                        orderExprTokenCount = 0;
                        break;
                }
            } while (_tokenEnum.MoveNext());

            if (orderBeginToken != null)
            {
                orders.Add(ParseOrderDefinition(orderBeginToken, null, 
                    orderExprTokenCount == 1 ? columnNameToken : null, directionToken));
            }
            
            return orders;
        }

        private OrderDefinition ParseOrderDefinition(SqlToken beginToken, SqlToken endToken, SqlToken columnNameToken, SqlToken directionToken)
        {
            ColumnDefinition column;
            bool? isDescending = directionToken != null
                ? directionToken.Equals("desc", StringComparison.InvariantCultureIgnoreCase)
                : default(bool?);
            
            if (columnNameToken != null)
            {
                string columnNameOrIndex = columnNameToken.ToString();
                if (!TryGetColumnDefinition(columnNameOrIndex, out column))
                {
                    // Column appears in order by clause, but not in select clause
                    column = ParseColumnDefinition(columnNameToken, isDescending.HasValue ? directionToken : endToken, columnNameToken);
                }
            }
            else
            {
                // Calculated sort order
                column = ParseColumnDefinition(beginToken, isDescending.HasValue ? directionToken : endToken, "__order" + _columns.Count);
            }

            return new OrderDefinition(column, isDescending ?? false);
        }

        private bool TryGetColumnDefinition(string columnNameOrIndex, out ColumnDefinition result)
        {
            if (!string.IsNullOrEmpty(columnNameOrIndex))
            {
                int columnIndex;
                if (int.TryParse(columnNameOrIndex, out columnIndex) && columnIndex <= _columns.Count)
                {
                    result = _columns[columnIndex - 1];
                    return true;
                }

                foreach (var column in _columns)
                {
                    if (columnNameOrIndex.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)
                        || columnNameOrIndex.Equals(column.Alias, StringComparison.InvariantCultureIgnoreCase))
                    {
                        result = column;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private bool TryParseUntil(string term)
        {
            int nestLevel = 0;
            do
            {
                var token = _tokenEnum.Current;
                if (token == null) return false;

                switch (token.TokenType)
                {
                    case SqlTokenType.BlockBegin:
                        nestLevel++;
                        break;
                    case SqlTokenType.BlockEnd:
                        nestLevel--;
                        break;
                    case SqlTokenType.Text:
                        if (nestLevel == 0 && token.Equals(term, StringComparison.InvariantCultureIgnoreCase)) return true;
                        break;
                }
            } while (_tokenEnum.MoveNext());

            return false;
        }

        private bool TryParseWhitespaceAndComments()
        {
            do
            {
                var token = _tokenEnum.Current;
                if (token == null) return false;

                switch (token.TokenType)
                {
                    case SqlTokenType.Comment:
                    case SqlTokenType.Whitespace:
                        break;
                    default:
                        return true;
                }
            } while (_tokenEnum.MoveNext());

            return false;
        }


        protected class ColumnDefinition
        {
            public string Name { get; private set; }
            public string Alias { get; private set; }
            public int SqlIndex { get; private set; }
            public int SqlLength { get; private set; }

            internal ColumnDefinition(int sqlIndex, int sqlLength, string name, string alias)
            {
                this.SqlIndex = sqlIndex;
                this.SqlLength = sqlLength;
                this.Name = name;
                this.Alias = alias;
            }
        }

        protected class OrderDefinition
        {
            public ColumnDefinition Column { get; private set; }
            public bool IsDescending { get; private set; }

            internal OrderDefinition(ColumnDefinition column, bool isDescending)
            {
                this.Column = column;
                this.IsDescending = isDescending;
            }
        }
    }
}
