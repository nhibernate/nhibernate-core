using System.Data;
using System.Text;
using NHibernate.Driver;
using NHibernate.Mapping;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	using System;
	using System.Collections.Generic;

	public class MsSql2005Dialect : MsSql2000Dialect
	{
		public MsSql2005Dialect()
		{
			RegisterColumnType(DbType.Xml, "XML");
		}

		protected override void RegisterCharacterTypeMappings()
		{
			base.RegisterCharacterTypeMappings();
			RegisterColumnType(DbType.String, SqlClientDriver.MaxSizeForClob, "NVARCHAR(MAX)");
			RegisterColumnType(DbType.AnsiString, SqlClientDriver.MaxSizeForAnsiClob, "VARCHAR(MAX)");
		}

		protected override void RegisterLargeObjectTypeMappings()
		{
			base.RegisterLargeObjectTypeMappings();
			RegisterColumnType(DbType.Binary, "VARBINARY(MAX)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForLengthLimitedBinary, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, SqlClientDriver.MaxSizeForBlob, "VARBINARY(MAX)");
		}

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
			RegisterKeyword("xml");
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			if (offset == null)
			{
				int insertPoint;
				return TryFindLimitInsertPoint(queryString, out insertPoint)
					? queryString.Insert(insertPoint, new SqlString(" TOP (", limit, ")"))
					: null;
			}

			var queryParser = new SqlSelectParser(queryString);
			var result = new SqlStringBuilder();
			BuildSelectClauseForPagingQuery(queryParser, limit, result);
			BuildFromClauseForPagingQuery(queryParser, result);
			BuildWhereAndOrderClausesForPagingQuery(offset, result);
			return result.ToSqlString();
		}

		private static void BuildSelectClauseForPagingQuery(SqlSelectParser sqlQuery, SqlString limit, SqlStringBuilder result)
		{
			result.Add(sqlQuery.Sql.Substring(0, sqlQuery.SelectIndex));
			result.Add("SELECT");

			if (limit != null)
			{
				result.Add(" TOP (").Add(limit).Add(") ");
			}
			else
			{
				// ORDER BY can only be used in subqueries if TOP is also specified.
				result.Add(" TOP (" + int.MaxValue + ") ");
			}

			var sb = new StringBuilder();
			foreach (var column in sqlQuery.ColumnDefinitions)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(column.Alias);
			}

			result.Add(sb.ToString());
		}

		private static void BuildFromClauseForPagingQuery(SqlSelectParser sqlQuery, SqlStringBuilder result)
		{
			var selectClause = sqlQuery.Sql.Substring(sqlQuery.SelectIndex, sqlQuery.FromIndex - sqlQuery.SelectIndex);
			var subselectClause = sqlQuery.OrderByIndex >= 0
				? sqlQuery.Sql.Substring(sqlQuery.FromIndex, sqlQuery.OrderByIndex - sqlQuery.FromIndex)
				: sqlQuery.Sql.Substring(sqlQuery.FromIndex);

			result.Add(" FROM (")
				.Add(selectClause.Trim())
				.Add(", ROW_NUMBER() OVER(ORDER BY ");

			int orderIndex = 0;
			foreach (var order in sqlQuery.OrderDefinitions)
			{
				if (orderIndex++ > 0) result.Add(", ");
				if (order.Column.Name != null)
				{
					result.Add(order.Column.Name);
				}
				else
				{
					result.Add(sqlQuery.Sql.Substring(order.Column.SqlIndex, order.Column.SqlLength).Trim());
				}
				if (order.IsDescending) result.Add(" DESC");
			}
			if (orderIndex == 0)
			{
				result.Add("CURRENT_TIMESTAMP");
			}

			result.Add(") as __hibernate_sort_row ")
				.Add(subselectClause.Trim())
				.Add(") as query");
		}

		private static void BuildWhereAndOrderClausesForPagingQuery(SqlString offset, SqlStringBuilder result)
		{
			result.Add(" WHERE query.__hibernate_sort_row > ")
				.Add(offset)
				.Add(" ORDER BY query.__hibernate_sort_row");
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return true; }
		}

		protected override string GetSelectExistingObject(string name, Table table)
		{
			string schema = table.GetQuotedSchemaName(this);
			if (schema != null)
			{
				schema += ".";
			}
			string objName = string.Format("{0}{1}", schema, Quote(name));
			string parentName = string.Format("{0}{1}", schema, table.GetQuotedName(this));
			return
				string.Format(
					"select 1 from sys.objects where object_id = OBJECT_ID(N'{0}') AND parent_object_id = OBJECT_ID('{1}')", objName,
					parentName);
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Represents SELECT query parser, primarily intended to support generation of SQL Server limit SQL queries.
		/// </summary>
		private class SqlSelectParser
		{
			private readonly List<ColumnDefinition> _columns = new List<ColumnDefinition>();
			private readonly List<OrderDefinition> _orders = new List<OrderDefinition>();

			public SqlSelectParser(SqlString sql)
			{
				if (sql == null) throw new ArgumentNullException("sql");
				this.Sql = sql;
				this.SelectIndex = this.FromIndex = this.OrderByIndex = -1;

				var tokenEnum = sql.Tokenize(SqlTokenType.AllExceptWhitespaceOrComment).GetEnumerator();
				tokenEnum.MoveNext();
				
				// Custom SQL may contain multiple SELECT statements, for example to assign parameters. 
				// Therefore we loop over SELECT statements until a SELECT is found that returns data.
				while (TryParseUntil(tokenEnum, "select"))
				{
					this.SelectIndex = tokenEnum.Current.SqlIndex;
					if (tokenEnum.MoveNext() && TryParseUntilBeginOfColumnDefinitions(tokenEnum))
					{
						_columns.AddRange(ParseColumnDefinitions(tokenEnum));
						if (TryParseUntil(tokenEnum, "from"))
						{
							this.FromIndex = tokenEnum.Current.SqlIndex;
							if (tokenEnum.MoveNext() && TryParseUntil(tokenEnum, "order"))
							{
								this.OrderByIndex = tokenEnum.Current.SqlIndex;
								if (tokenEnum.MoveNext() && TryParseUntilBeginOfOrderDefinitions(tokenEnum))
								{
									_orders.AddRange(ParseOrderDefinitions(tokenEnum));
								}
							}
						}
						return;
					}
					
					this.SelectIndex = -1;
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

			private static bool TryParseUntilBeginOfColumnDefinitions(IEnumerator<SqlToken> tokenEnum)
			{
				if (tokenEnum.Current.Equals("distinct", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!tokenEnum.MoveNext()) return false;
				}

				// Ignore parameter assignment statements with syntax SELECT @p = ...
				return !tokenEnum.Current.UnquotedValue.StartsWith("@");
			}

			private static bool TryParseUntilBeginOfOrderDefinitions(IEnumerator<SqlToken> tokenEnum)
			{
				return tokenEnum.Current.Equals("by", StringComparison.InvariantCultureIgnoreCase)
					? tokenEnum.MoveNext()
					: false;
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
						case SqlTokenType.BlockBegin:
							blockLevel++;
							break;

						case SqlTokenType.BlockEnd:
							blockLevel--;
							break;

						case SqlTokenType.UnquotedText:
							if (blockLevel != 0) break;

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

						case SqlTokenType.QuotedText:
							if (blockLevel != 0) break;

							columnAliasToken = token;
							break;

						case SqlTokenType.ListSeparator:
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
						case SqlTokenType.BlockBegin:
							blockLevel++;
							break;

						case SqlTokenType.BlockEnd:
							blockLevel--;
							if (blockLevel == 0) orderExprTokenCount++;
							break;

						case SqlTokenType.UnquotedText:
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

			private static bool TryParseUntil(IEnumerator<SqlToken> tokenEnum, string term)
			{
				int nestLevel = 0;
				do
				{
					var token = tokenEnum.Current;
					if (token == null) return false;

					switch (token.TokenType)
					{
						case SqlTokenType.BlockBegin:
							nestLevel++;
							break;
						case SqlTokenType.BlockEnd:
							nestLevel--;
							break;
						case SqlTokenType.UnquotedText:
							if (nestLevel == 0 && token.Equals(term, StringComparison.InvariantCultureIgnoreCase)) return true;
							break;
					}
				} while (tokenEnum.MoveNext());

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
}