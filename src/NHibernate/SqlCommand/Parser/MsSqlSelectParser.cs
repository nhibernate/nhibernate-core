using System;
using System.Collections.Generic;

namespace NHibernate.SqlCommand.Parser
{
	using System.Linq;

	/// <summary>
	/// Represents SQL Server SELECT query parser, primarily intended to support generation of 
	/// limit queries by SQL Server dialects.
	/// </summary>
	internal class MsSqlSelectParser
	{
		private readonly List<ColumnDefinition> _columns = new List<ColumnDefinition>();
		private readonly List<OrderDefinition> _orders = new List<OrderDefinition>();
		private int _nextOrderAliasIndex;

		public MsSqlSelectParser(SqlString sql)
		{
			Sql = sql ?? throw new ArgumentNullException(nameof(sql));
			SelectIndex = FromIndex = OrderByIndex = -1;

			using (var tokenEnum = new SqlTokenizer(sql).GetEnumerator())
			{
				tokenEnum.MoveNext();

				// Custom SQL may contain multiple SELECT statements, for example to assign parameters. 
				// Therefore we loop over SELECT statements until a SELECT is found that returns data.
				if (tokenEnum.TryParseUntilFirstMsSqlSelectColumn(out var selectToken, out var isDistinct))
				{
					SelectIndex = selectToken.SqlIndex;
					IsDistinct = isDistinct;
					_columns.AddRange(ParseColumnDefinitions(tokenEnum));
					if (tokenEnum.TryParseUntil("from"))
					{
						FromIndex = tokenEnum.Current.SqlIndex;

						if (tokenEnum.TryParseUntilFirstOrderColumn(out var orderToken))
						{
							OrderByIndex = orderToken.SqlIndex;
							foreach (var order in ParseOrderDefinitions(tokenEnum))
							{
								_orders.Add(order);
								if (!order.Column.InSelectClause)
								{
									_columns.Add(order.Column);
								}
							}
						}
					}
				}
			}
		}

		public SqlString Sql { get; }
		public int SelectIndex { get; }
		public int FromIndex { get; }
		public int OrderByIndex { get;}
		public bool IsDistinct { get; }

		/// <summary>
		/// Column definitions in SELECT clause
		/// </summary>
		public IEnumerable<ColumnDefinition> SelectColumns
		{
			get { return _columns.Where(c => c.InSelectClause); }
		}

		/// <summary>
		/// Column definitions for columns that appear in ORDER BY clause 
		/// but do not appear in SELECT clause.
		/// </summary>
		public IEnumerable<ColumnDefinition> NonSelectColumns
		{
			get { return _columns.Where(c => !c.InSelectClause); }
		}

		/// <summary>
		/// Sort orders as defined in ORDER BY clause
		/// </summary>
		public IEnumerable<OrderDefinition> Orders
		{
			get { return _orders; }
		}

		public SqlString SelectClause
		{
			get { return this.Sql.Substring(this.SelectIndex, this.FromIndex - this.SelectIndex).Trim(); }
		}

		public SqlString FromAndWhereClause
		{
			get
			{
				return this.OrderByIndex >= 0
					? this.Sql.Substring(this.FromIndex, this.OrderByIndex - this.FromIndex).Trim()
					: this.Sql.Substring(this.FromIndex).Trim();
			}
		}

		public SqlString ColumnExpression(ColumnDefinition column)
		{
			return this.Sql.Substring(column.SqlIndex, column.SqlLength);
		}

		private IEnumerable<ColumnDefinition> ParseColumnDefinitions(IEnumerator<SqlToken> tokenEnum)
		{
			int blockLevel = 0;
			SqlToken columnBeginToken = null;
			SqlToken columnEndToken = null;
			SqlToken columnAliasToken = null;

			SqlToken prevToken = null;
			do
			{
				var token = tokenEnum.Current;
				if (token == null) break;

				columnBeginToken = columnBeginToken ?? token;

				switch (token.TokenType)
				{
					case SqlTokenType.BracketOpen:
						blockLevel++;
						break;

					case SqlTokenType.BracketClose:
						blockLevel--;
						break;

					case SqlTokenType.Text:
						if (blockLevel != 0) break;

						if (token.Equals(",", StringComparison.Ordinal))
						{
							if (columnAliasToken != null)
							{
								yield return ParseSelectColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
							}
						}

						if (token.Equals("from", StringComparison.OrdinalIgnoreCase))
						{
							if (columnAliasToken != null)
							{
								yield return ParseSelectColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
							}
							yield break;
						}

						if (token.Equals("as", StringComparison.OrdinalIgnoreCase))
						{
							columnEndToken = prevToken;
						}

						columnAliasToken = token;
						break;

					case SqlTokenType.DelimitedText:
						if (blockLevel != 0) break;

						columnAliasToken = token;
						break;

					case SqlTokenType.Comma:
						if (blockLevel != 0) break;

						if (columnAliasToken != null)
						{
							yield return ParseSelectColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
						}
						columnBeginToken = columnEndToken = columnAliasToken = null;
						break;
				}

				prevToken = token;
			} while (tokenEnum.MoveNext());

			if (columnAliasToken != null)
			{
				yield return ParseSelectColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
			}
		}

		private ColumnDefinition ParseSelectColumnDefinition(SqlToken beginToken, SqlToken endToken, SqlToken aliasToken)
		{
			var name = beginToken == endToken
				? beginToken.Value
				: null;

			var alias = aliasToken.Value;
			var dotIndex = alias.LastIndexOf('.');
			alias = dotIndex >= 0
				? alias.Substring(dotIndex + 1)
				: alias;

			var sqlIndex = beginToken.SqlIndex;
			var sqlLength = (endToken != null ? endToken.SqlIndex + endToken.Length : Sql.Length) - beginToken.SqlIndex;

			return new ColumnDefinition(sqlIndex, sqlLength, name, alias, true);
		}

		private ColumnDefinition ParseOrderColumnDefinition(SqlToken beginToken, SqlToken endToken, string alias)
		{
			var sqlIndex = beginToken.SqlIndex;
			var sqlLength = (endToken != null ? endToken.SqlIndex + endToken.Length : Sql.Length) - beginToken.SqlIndex;

			return new ColumnDefinition(sqlIndex, sqlLength, null, alias, false);
		}

		private IEnumerable<OrderDefinition> ParseOrderDefinitions(IEnumerator<SqlToken> tokenEnum)
		{
			int blockLevel = 0;
			SqlToken orderBeginToken = null;
			SqlToken orderEndToken = null;
			SqlToken directionToken = null;

			SqlToken prevToken = null;
			do
			{
				var token = tokenEnum.Current;
				if (token == null) break;
				if (token.TokenType == SqlTokenType.Whitespace) continue;

				orderBeginToken = orderBeginToken ?? token;

				switch (token.TokenType)
				{
					case SqlTokenType.BracketOpen:
						blockLevel++;
						break;

					case SqlTokenType.BracketClose:
						blockLevel--;
						break;

					case SqlTokenType.Text:
						if (blockLevel != 0) break;

						if (token.Equals("asc", StringComparison.OrdinalIgnoreCase)
							|| token.Equals("desc", StringComparison.OrdinalIgnoreCase))
						{
							orderEndToken = prevToken;
							directionToken = token;
						}
						break;

					case SqlTokenType.DelimitedText:
						if (blockLevel != 0) break;
						break;

					case SqlTokenType.Comma:
						if (blockLevel != 0) break;

						yield return ParseOrderDefinition(orderBeginToken, orderEndToken ?? prevToken, directionToken);
						orderBeginToken = orderEndToken = directionToken = null;
						break;
				}

				prevToken = token;
			} while (tokenEnum.MoveNext());

			if (orderBeginToken != null)
			{
				yield return ParseOrderDefinition(orderBeginToken, orderEndToken ?? prevToken, directionToken);
			}
		}

		private OrderDefinition ParseOrderDefinition(SqlToken beginToken, SqlToken endToken, SqlToken directionToken)
		{
			var isDescending = directionToken != null &&
							   directionToken.Equals("desc", StringComparison.OrdinalIgnoreCase);

			var columnNameOrIndex = beginToken == endToken
				? beginToken.Value
				: null;

			ColumnDefinition column;
			if (!TryGetColumnDefinition(columnNameOrIndex, out column, beginToken, endToken))
			{   
				// Column appears in order by clause, but not in select clause
				column = ParseOrderColumnDefinition(beginToken, endToken, "__c" + _nextOrderAliasIndex++);
			}

			return new OrderDefinition(column, isDescending);
		}

		private bool TryGetColumnDefinition(string columnNameOrIndex, out ColumnDefinition result, SqlToken beginToken, SqlToken endToken)
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
			else
			{
				var sqlIndex = beginToken.SqlIndex;
				var sqlLength = (endToken != null ? endToken.SqlIndex + endToken.Length : Sql.Length) - beginToken.SqlIndex;
				var text = Sql.ToString(sqlIndex, sqlLength);
				foreach (var column in _columns)
				{
					if (text.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase) ||
						text.Equals(column.Alias, StringComparison.InvariantCultureIgnoreCase) ||
						text.Equals(Sql.ToString(column.SqlIndex, column.SqlLength), StringComparison.InvariantCultureIgnoreCase))
					{
						result = column;
						return true;
					}
				}
			}

			result = null;
			return false;
		}

		public class ColumnDefinition
		{
			public string Name { get; private set; }
			public string Alias { get; private set; }
			public int SqlIndex { get; private set; }
			public int SqlLength { get; private set; }
			public bool InSelectClause { get; private set; }

			internal ColumnDefinition(int sqlIndex, int sqlLength, string name, string alias, bool inSelectClause)
			{
				this.SqlIndex = sqlIndex;
				this.SqlLength = sqlLength;
				this.Name = name;
				this.Alias = alias;
				this.InSelectClause = inSelectClause;
			}

			public override string ToString()
			{
				if (this.Name == null) return this.Alias;
				return this.Name != this.Alias
					? this.Name + " AS " + this.Alias
					: this.Name;
			}
		}

		public class OrderDefinition
		{
			public ColumnDefinition Column { get; private set; }
			public bool IsDescending { get; private set; }

			internal OrderDefinition(ColumnDefinition column, bool isDescending)
			{
				this.Column = column;
				this.IsDescending = isDescending;
			}

			public override string ToString()
			{
				return this.IsDescending
					? this.Column + " DESC"
					: this.Column.ToString();
			}
		}
	}
}
