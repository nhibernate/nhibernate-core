using System;
using System.Collections.Generic;
namespace NHibernate.SqlCommand.Parser
{
	/// <summary>
	/// Represents SQL Server SELECT query parser, primarily intended to support generation of 
	/// limit queries by SQL Server dialects.
	/// </summary>
	internal class MsSqlSelectParser
	{
		private readonly List<ColumnDefinition> _columns = new List<ColumnDefinition>();
		private readonly List<OrderDefinition> _orders = new List<OrderDefinition>();

		public MsSqlSelectParser(SqlString sql)
		{
			if (sql == null) throw new ArgumentNullException("sql");
			this.Sql = sql;
			this.SelectIndex = this.FromIndex = this.OrderByIndex = -1;

			var tokenEnum = new SqlTokenizer(sql).GetEnumerator();
			tokenEnum.MoveNext();

			// Custom SQL may contain multiple SELECT statements, for example to assign parameters. 
			// Therefore we loop over SELECT statements until a SELECT is found that returns data.
			SqlToken selectToken;
			if (tokenEnum.TryParseUntilFirstMsSqlSelectColumn(out selectToken))
			{
				this.SelectIndex = selectToken.SqlIndex;
				_columns.AddRange(ParseColumnDefinitions(tokenEnum));
				if (tokenEnum.TryParseUntil("from"))
				{
					this.FromIndex = tokenEnum.Current.SqlIndex;

					SqlToken orderToken;
					if (tokenEnum.TryParseUntilFirstOrderColumn(out orderToken))
					{
						this.OrderByIndex = orderToken.SqlIndex;
						_orders.AddRange(ParseOrderDefinitions(tokenEnum));
					}
				}
				return;
			}
		}

		public SqlString Sql { get; private set; }
		public int SelectIndex { get; private set; }
		public int FromIndex { get; private set; }
		public int OrderByIndex { get; private set; }

		public IEnumerable<ColumnDefinition> ColumnDefinitions
		{
			get { return _columns; }
		}

		public IEnumerable<OrderDefinition> OrderDefinitions
		{
			get { return _orders; }
		}

		private IEnumerable<ColumnDefinition> ParseColumnDefinitions(IEnumerator<SqlToken> tokenEnum)
		{
			int blockLevel = 0;
			SqlToken columnBeginToken = null;
			SqlToken columnEndToken = null;
			SqlToken columnAliasToken = null;

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

						if (token.Equals(",", StringComparison.InvariantCultureIgnoreCase))
						{
							if (columnAliasToken != null)
							{
								yield return ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
							}
						}

						if (token.Equals("from", StringComparison.InvariantCultureIgnoreCase))
						{
							if (columnAliasToken != null)
							{
								yield return ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
							}
							yield break;
						}

						if (token.Equals("as", StringComparison.InvariantCultureIgnoreCase))
						{
							columnEndToken = token;
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
							yield return ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
						}
						columnBeginToken = columnEndToken = columnAliasToken = null;
						break;
				}
			} while (tokenEnum.MoveNext());

			if (columnAliasToken != null)
			{
				yield return ParseColumnDefinition(columnBeginToken, columnEndToken ?? columnAliasToken, columnAliasToken);
			}
		}

		private ColumnDefinition ParseColumnDefinition(SqlToken beginToken, SqlToken endToken, SqlToken aliasToken)
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
			var sqlLength = (endToken != null ? endToken.SqlIndex : this.Sql.Length) - beginToken.SqlIndex;

			return new ColumnDefinition(sqlIndex, sqlLength, name, alias);
		}

		private ColumnDefinition ParseColumnDefinition(SqlToken beginToken, SqlToken endToken, string alias)
		{
			var sqlIndex = beginToken.SqlIndex;
			var sqlLength = (endToken != null ? endToken.SqlIndex : this.Sql.Length) - beginToken.SqlIndex;

			return new ColumnDefinition(sqlIndex, sqlLength, null, alias);
		}

		private IEnumerable<OrderDefinition> ParseOrderDefinitions(IEnumerator<SqlToken> tokenEnum)
		{
			int blockLevel = 0;
			int orderExprTokenCount = 0;
			SqlToken orderBeginToken = null;
			SqlToken columnNameToken = null;
			SqlToken directionToken = null;

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

					case SqlTokenType.DelimitedText:
						if (blockLevel != 0) break;

						if (orderExprTokenCount++ == 0) columnNameToken = token;
						break;

					case SqlTokenType.Comma:
						if (blockLevel != 0) break;

						yield return ParseOrderDefinition(orderBeginToken, token,
							orderExprTokenCount == 1 ? columnNameToken : null, directionToken);
						orderBeginToken = columnNameToken = directionToken = null;
						orderExprTokenCount = 0;
						break;
				}
			} while (tokenEnum.MoveNext());

			if (orderBeginToken != null)
			{
				yield return ParseOrderDefinition(orderBeginToken, null,
					orderExprTokenCount == 1 ? columnNameToken : null, directionToken);
			}
		}

		private OrderDefinition ParseOrderDefinition(SqlToken beginToken, SqlToken endToken, SqlToken columnNameToken, SqlToken directionToken)
		{
			ColumnDefinition column;
			bool? isDescending = directionToken != null
				? directionToken.Equals("desc", StringComparison.InvariantCultureIgnoreCase)
				: default(bool?);

			if (columnNameToken != null)
			{
				string columnNameOrIndex = columnNameToken.Value;
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

		public class ColumnDefinition
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

		public class OrderDefinition
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
