using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	public class MsSql2005Dialect : MsSql2000Dialect
	{
		public MsSql2005Dialect()
		{
			RegisterColumnType(DbType.String, 1073741823, "NVARCHAR(MAX)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "VARCHAR(MAX)");
			RegisterColumnType(DbType.Binary, 2147483647, "VARBINARY(MAX)");
		}

		/// <summary>
		/// Add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>
		/// </summary>
		/// <param name="querySqlString">The <see cref="SqlString"/> to base the limit query off of.</param>
		/// <param name="offset">Offset of the first row to be returned by the query (zero-based)</param>
		/// <param name="limit">Maximum number of rows to be returned by the query</param>
		/// <param name="offsetParameterIndex">Optionally, the Offset parameter index</param>
		/// <param name="limitParameterIndex">Optionally, the Limit parameter index</param>
		/// <returns>A new <see cref="SqlString"/> with the <c>LIMIT</c> clause applied.</returns>
		/// <remarks>
		/// Note that we need to explicitly specify the columns, because we need to be able to use them in a paged subselect [NH-1155]
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit, int? offsetParameterIndex, int? limitParameterIndex)
		{
			SqlStringBuilder result = new SqlStringBuilder();
						
			if (offset == 0)
			{
				int insertPoint = this.GetAfterSelectInsertPoint(querySqlString);
				
				return result
					.Add(querySqlString.Substring(0, insertPoint))
					.Add(" TOP (")
					.Add(limitParameterIndex == null ? Parameter.Placeholder : Parameter.WithIndex(limitParameterIndex.Value))
					.Add(")")
					.Add(querySqlString.Substring(insertPoint))
					.ToSqlString();
			}

			int fromIndex = GetFromIndex(querySqlString);
			SqlString select = querySqlString.Substring(0, fromIndex);

			List<SqlString> columnsOrAliases;
			Dictionary<SqlString, SqlString> aliasToColumn;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn);
		
			int orderIndex = querySqlString.LastIndexOfCaseInsensitive(" order by ");
			SqlString fromAndWhere;
			SqlString[] sortExpressions;

			//don't use the order index if it is contained within a larger statement(assuming 
			//a statement with non matching parenthesis is part of a larger block)
			if (orderIndex > 0 && HasMatchingParens(querySqlString.Substring(orderIndex).ToString()))
			{
				fromAndWhere = querySqlString.Substring(fromIndex, orderIndex - fromIndex).Trim();
				SqlString orderBy = querySqlString.Substring(orderIndex).Trim();
				sortExpressions = orderBy.Substring(9).Split(",");
			}
			else
			{
				fromAndWhere = querySqlString.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new[] {new SqlString("CURRENT_TIMESTAMP"),};
			}				
				
			result
				.Add("SELECT TOP (")
				.Add(limitParameterIndex == null ? Parameter.Placeholder : Parameter.WithIndex(limitParameterIndex.Value))
				.Add(") ")
				.Add(StringHelper.Join(", ", columnsOrAliases))
				.Add(" FROM (")
				.Add(select)
				.Add(", ROW_NUMBER() OVER(ORDER BY ");
			
			AppendSortExpressions(aliasToColumn, sortExpressions, result);

			result
				.Add(") as __hibernate_sort_row ")
				.Add(fromAndWhere)
				.Add(") as query WHERE query.__hibernate_sort_row > ")
				.Add(offsetParameterIndex == null ? Parameter.Placeholder : Parameter.WithIndex(offsetParameterIndex.Value))
				.Add(" ORDER BY query.__hibernate_sort_row");
			
			return result.ToSqlString();
		}

		private static SqlString RemoveSortOrderDirection(SqlString sortExpression)
		{
			SqlString trimmedExpression = sortExpression.Trim();
			if (trimmedExpression.EndsWithCaseInsensitive("asc"))
				return trimmedExpression.Substring(0, trimmedExpression.Length - 3).Trim();
			if (trimmedExpression.EndsWithCaseInsensitive("desc"))
				return trimmedExpression.Substring(0, trimmedExpression.Length - 4).Trim();
			return trimmedExpression.Trim();
		}

		private static void AppendSortExpressions(Dictionary<SqlString, SqlString> aliasToColumn, SqlString[] sortExpressions, SqlStringBuilder result)
		{
			for (int i = 0; i < sortExpressions.Length; i++)
			{
				if (i > 0)
				{
					result.Add(", ");
				}

				SqlString sortExpression = RemoveSortOrderDirection(sortExpressions[i]);
				if (aliasToColumn.ContainsKey(sortExpression))
				{
					result.Add(aliasToColumn[sortExpression]);
				}
				else
				{
					result.Add(sortExpression);
				}
				if (sortExpressions[i].Trim().EndsWithCaseInsensitive("desc"))
				{
					result.Add(" DESC");
				}
			}
		}

		private static int GetFromIndex(SqlString querySqlString)
		{
			string subselect = querySqlString.GetSubselectString().ToString();
			int fromIndex = querySqlString.IndexOfCaseInsensitive(subselect);
			if (fromIndex == -1)
			{
				fromIndex = querySqlString.ToString().ToLowerInvariant().IndexOf(subselect.ToLowerInvariant());
			}
			return fromIndex;
		}
		
		private int GetAfterSelectInsertPoint(SqlString sql)
		{
			if (sql.StartsWithCaseInsensitive("select distinct"))
			{
				return 15;
			}
			else if (sql.StartsWithCaseInsensitive("select"))
			{
				return 6;
			}
			throw new NotSupportedException("The query should start with 'SELECT' or 'SELECT DISTINCT'");
		}		

		private static void ExtractColumnOrAliasNames(SqlString select, out List<SqlString> columnsOrAliases,
				out Dictionary<SqlString, SqlString> aliasToColumn)
		{
			columnsOrAliases = new List<SqlString>();
			aliasToColumn = new Dictionary<SqlString, SqlString>();

			IList<SqlString> tokens = new QuotedAndParenthesisStringTokenizer(select).GetTokens();
			int index = 0;
			while (index < tokens.Count)
			{
				SqlString token = tokens[index];
				
				int nextTokenIndex = index += 1;
				
				if (token.StartsWithCaseInsensitive("select"))
					continue;

				if (token.StartsWithCaseInsensitive("distinct"))
					continue;

				if (token.StartsWithCaseInsensitive(","))
					continue;

				if (token.StartsWithCaseInsensitive("from"))
					break;

				// handle composite expressions like "2 * 4 as foo"
				while ((nextTokenIndex < tokens.Count) && (tokens[nextTokenIndex].StartsWithCaseInsensitive("as") == false && tokens[nextTokenIndex].StartsWithCaseInsensitive(",") == false))
				{
					SqlString nextToken = tokens[nextTokenIndex];
					token = token.Append(nextToken);
					nextTokenIndex = index += 1;
				}

				// if there is no alias, the token and the alias will be the same
				SqlString alias = token;

				bool isFunctionCallOrQuotedString = token.IndexOfCaseInsensitive("'") >= 0 || token.IndexOfCaseInsensitive("(") >= 0;
				
				// this is heuristic guess, if the expression contains ' or (, it is probably
				// not appropriate to just slice parts off of it
				if (isFunctionCallOrQuotedString == false)
				{
					// its a simple column reference, so lets set the alias to the
					// column name minus the table qualifier if it exists
					int dot = token.IndexOfCaseInsensitive(".");
					if (dot != -1)
						alias = token.Substring(dot + 1);
				}

				// notice! we are checking here the existence of "as" "alias", two
				// tokens from the current one
				if (nextTokenIndex + 1 < tokens.Count)
				{
					SqlString nextToken = tokens[nextTokenIndex];
					if (nextToken.IndexOfCaseInsensitive("as") >= 0)
					{
						SqlString tokenAfterNext = tokens[nextTokenIndex + 1];	
						alias = tokenAfterNext;
						index += 2; //skip the "as" and the alias
					}
				}

				columnsOrAliases.Add(alias);
				aliasToColumn[alias] = token;
			}
		}

		/// <summary>
		/// Indicates whether the string fragment contains matching parenthesis
		/// </summary>
		/// <param name="statement"> the statement to evaluate</param>
		/// <returns>true if the statment contains no parenthesis or an equal number of
		///  opening and closing parenthesis;otherwise false </returns>
		private static bool HasMatchingParens(IEnumerable<char> statement)
		{
			//unmatched paren count
			int unmatchedParen = 0;

			//increment the counts based in the opening and closing parens in the statement
			foreach (char item in statement)
			{
				switch (item)
				{
					case '(':
						unmatchedParen++;
						break;
					case ')':
						unmatchedParen--;
						break;
				}
			}

			return unmatchedParen == 0;
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
		
		public override bool BindLimitParametersInReverseOrder 
		{
			get { return true; }
		}
		
		public override bool SupportsVariableLimit 
		{ 
			get { return true; } 
		}
		
		public override bool BindLimitParametersFirst 
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
		/// This specialized string tokenizier will break a string to tokens, taking
		/// into account single quotes, parenthesis and commas and [ ]
		/// Notice that we aren't differenciating between [ ) and ( ] on purpose, it would complicate
		/// the code and it is not legal at any rate.
		/// </summary>
		public class QuotedAndParenthesisStringTokenizer : IEnumerable<SqlString>
		{
			private readonly SqlString original;

			public QuotedAndParenthesisStringTokenizer(SqlString original)
			{
				this.original = original;
			}

			IEnumerator<SqlString> IEnumerable<SqlString>.GetEnumerator()
			{
				TokenizerState state = TokenizerState.WhiteSpace;
				int parenthesisCount = 0;
				bool escapeQuote = false;
				int tokenStart = 0;
				int tokenLength = 0;
				string originalString = original.ToString();

				for (int i = 0; i < originalString.Length; i++)
				{
					char ch = originalString[i];
					switch (state)
					{
						case TokenizerState.WhiteSpace:
							if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								tokenLength += 1;
							}
							else if (ch == ',')
							{
								yield return new SqlString(",");
								//tokenLength += 1?
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								tokenLength += 1;
								parenthesisCount = 1;
							}
							else if (char.IsWhiteSpace(ch) == false)
							{
								state = TokenizerState.Token;
								tokenLength += 1;
							}
							break;
						case TokenizerState.Quoted:
							if (escapeQuote)
							{
								escapeQuote = false;
								tokenLength += 1;
							}
							// handle escaping of ' by using '' or \'
							else if (ch == '\\' || (ch == '\'' && i + 1 < originalString.Length && originalString[i + 1] == '\''))
							{
								escapeQuote = true;
								tokenLength += 1;
							}
							else if (ch == '\'')
							{
								yield return original.Substring(tokenStart, tokenLength);
								tokenStart += tokenLength + 1;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						case TokenizerState.InParenthesis:
							if (ch == ')' || ch == ']')
							{
								tokenLength += 1;
								parenthesisCount -= 1;
								if (parenthesisCount == 0)
								{
									yield return original.Substring(tokenStart, tokenLength);
									tokenStart += tokenLength + 1;
									tokenLength = 0;
									state = TokenizerState.WhiteSpace;
								}
							}
							else if (ch == '(' || ch == '[')
							{
								tokenLength += 1;
								parenthesisCount += 1;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						case TokenizerState.Token:
							if (char.IsWhiteSpace(ch))
							{
								yield return original.Substring(tokenStart, tokenLength);
								tokenStart += tokenLength + 1;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == ',') // stop current token, and send the , as well
							{
								yield return original.Substring(tokenStart, tokenLength);
								yield return new SqlString(",");
								tokenStart += tokenLength + 2;
								tokenLength = 0;
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								parenthesisCount = 1;
								tokenLength += 1;
							}
							else if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								tokenLength += 1;
							}
							else
							{
								tokenLength += 1;
							}
							break;
						default:
							throw new InvalidExpressionException("Could not understand the string " + original);
					}
				}
				if (tokenLength > 0)
				{
					yield return original.Substring(tokenStart, tokenLength);
				}
			}

			public IEnumerator GetEnumerator()
			{
				return ((IEnumerable<SqlString>)this).GetEnumerator();
			}

			public enum TokenizerState
			{
				WhiteSpace,
				Quoted,
				InParenthesis,
				Token
			}

			public IList<SqlString> GetTokens()
			{
				return new List<SqlString>(this);
			}
		}
	}
}