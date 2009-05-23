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
		/// <param name="last">Maximum number of rows to be returned by the query</param>
		/// <returns>A new <see cref="SqlString"/> with the <c>LIMIT</c> clause applied.</returns>
		/// <remarks>
		/// The <c>LIMIT</c> SQL will look like
		/// <code>
		/// 
		/// SELECT
		///		TOP last (columns)
		///	FROM
		///		(SELECT (columns), ROW_NUMBER() OVER(ORDER BY {original order by, with un-aliased column names) as __hibernate_sort_row
		///		{original from}) as query
		/// WHERE query.__hibernate_sort_row > offset
		/// ORDER BY query.__hibernate_sort_row
		/// 
		/// </code>
		/// 
		/// Note that we need to add explicitly specify the columns, because we need to be able to use them
		/// in a paged subselect. NH-1155
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
		{
			//dont do this paging code if there is no offset, use the 
			//sql 2000 dialect since it wont just uses a top statement
			if (offset == 0)
			{
				return base.GetLimitString(querySqlString, offset, last);
			}
			// we have to do this in order to support parameters in order clause, the foramt 
			// that sql 2005 uses for paging means that we move the parameters around, which means,
			// that positions are lost, so we record them before making any changes.
			//  NH-1528
			int parameterPositon = 0;
			foreach (var part in querySqlString.Parts)
			{
				Parameter param = part as Parameter;
				if (param == null)
					continue;
				param.OriginalPositionInQuery = parameterPositon;
				parameterPositon += 1;
			}

			int fromIndex = GetFromIndex(querySqlString);
			SqlString select = querySqlString.Substring(0, fromIndex);
			List<SqlString> columnsOrAliases;
			Dictionary<SqlString, SqlString> aliasToColumn;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn);

			int orderIndex = querySqlString.LastIndexOfCaseInsensitive(" order by ");
			SqlString from;
			SqlString[] sortExpressions;

			//don't use the order index if it is contained within a larger statement(assuming 
			//a statement with non matching parenthesis is part of a larger block)
			if (orderIndex > 0 && HasMatchingParens(querySqlString.Substring(orderIndex).ToString()))
			{
				from = querySqlString.Substring(fromIndex, orderIndex - fromIndex).Trim();
				SqlString orderBy = querySqlString.Substring(orderIndex).Trim();
				sortExpressions = orderBy.Substring(9).Split(",");
			}
			else
			{
				from = querySqlString.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new[] {new SqlString("CURRENT_TIMESTAMP"),};
			}

			SqlStringBuilder result =
				new SqlStringBuilder()
					.Add("SELECT TOP ")
					.Add(last.ToString())
					.Add(" ")
					.Add(StringHelper.Join(", ", columnsOrAliases))
					.Add(" FROM (")
					.Add(select)
					.Add(", ROW_NUMBER() OVER(ORDER BY ");

			AppendSortExpressions(aliasToColumn, sortExpressions, result);

			result.Add(") as __hibernate_sort_row ")
					.Add(from)
					.Add(") as query WHERE query.__hibernate_sort_row > ")
					.Add(offset.ToString()).Add(" ORDER BY query.__hibernate_sort_row");
				
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

		private static void AppendSortExpressions(Dictionary<SqlString, SqlString> aliasToColumn, SqlString[] sortExpressions,
																							SqlStringBuilder result)
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

		private static void ExtractColumnOrAliasNames(SqlString select, out List<SqlString> columnsOrAliases,
				out Dictionary<SqlString, SqlString> aliasToColumn)
		{
			columnsOrAliases = new List<SqlString>();
			aliasToColumn = new Dictionary<SqlString, SqlString>();

			IList<string> tokens = new QuotedAndParenthesisStringTokenizer(select.ToString()).GetTokens();
			int index = 0;
			while (index < tokens.Count)
			{
				string token = tokens[index];
				index += 1;

				if ("select".Equals(token, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				if ("distinct".Equals(token, StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				if ("," == token)
				{
					continue;
				}

				if ("from".Equals(token, StringComparison.InvariantCultureIgnoreCase))
				{
					break;
				}

				//handle composite expressions like 2 * 4 as foo
				while (index < tokens.Count && "as".Equals(tokens[index], StringComparison.InvariantCultureIgnoreCase) == false
							 && "," != tokens[index])
				{
					token = token + " " + tokens[index];
					index += 1;
				}

				string alias = token;

				bool isFunctionCallOrQuotedString = token.Contains("'") || token.Contains("(");
				// this is heuristic guess, if the expression contains ' or (, it is probably
				// not appropriate to just slice parts off of it
				if (isFunctionCallOrQuotedString == false)
				{
					int dot = token.IndexOf('.');
					if (dot != -1)
					{
						alias = token.Substring(dot + 1);
					}
				}

				// notice! we are checking here the existence of "as" "alias", two
				// tokens from the current one
				if (index + 1 < tokens.Count && "as".Equals(tokens[index], StringComparison.InvariantCultureIgnoreCase))
				{
					alias = tokens[index + 1];
					index += 2; //skip the "as" and the alias	\
				}

				columnsOrAliases.Add(new SqlString(alias));
				aliasToColumn[SqlString.Parse(alias)] = SqlString.Parse(token);
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
		public class QuotedAndParenthesisStringTokenizer : IEnumerable<String>
		{
			private readonly string original;

			public QuotedAndParenthesisStringTokenizer(string original)
			{
				this.original = original;
			}

			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				StringBuilder currentToken = new StringBuilder();
				TokenizerState state = TokenizerState.WhiteSpace;
				int parenthesisCount = 0;
				bool escapeQuote = false;
				for (int i = 0; i < original.Length; i++)
				{
					char ch = original[i];
					switch (state)
					{
						case TokenizerState.WhiteSpace:
							if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								currentToken.Append(ch);
							}
							else if (ch == ',')
							{
								yield return ",";
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								currentToken.Append(ch);
								parenthesisCount = 1;
							}
							else if (char.IsWhiteSpace(ch) == false)
							{
								state = TokenizerState.Token;
								currentToken.Append(ch);
							}
							break;
						case TokenizerState.Quoted:
							if (escapeQuote)
							{
								escapeQuote = false;
								currentToken.Append(ch);
							}
							// handle escaping of ' by using '' or \'
							else if (ch == '\\' || (ch == '\'' && i + 1 < original.Length && original[i + 1] == '\''))
							{
								escapeQuote = true;
								currentToken.Append(ch);
							}
							else if (ch == '\'')
							{
								currentToken.Append(ch);
								yield return currentToken.ToString();
								state = TokenizerState.WhiteSpace;
								currentToken.Length = 0;
							}
							else
							{
								currentToken.Append(ch);
							}
							break;
						case TokenizerState.InParenthesis:
							if (ch == ')' || ch == ']')
							{
								currentToken.Append(ch);
								parenthesisCount -= 1;
								if (parenthesisCount == 0)
								{
									yield return currentToken.ToString();
									currentToken.Length = 0;
									state = TokenizerState.WhiteSpace;
								}
							}
							else if (ch == '(' || ch == '[')
							{
								currentToken.Append(ch);
								parenthesisCount += 1;
							}
							else
							{
								currentToken.Append(ch);
							}
							break;
						case TokenizerState.Token:
							if (char.IsWhiteSpace(ch))
							{
								yield return currentToken.ToString();
								currentToken.Length = 0;
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == ',') // stop current token, and send the , as well
							{
								yield return currentToken.ToString();
								currentToken.Length = 0;
								yield return ",";
								state = TokenizerState.WhiteSpace;
							}
							else if (ch == '(' || ch == '[')
							{
								state = TokenizerState.InParenthesis;
								parenthesisCount = 1;
								currentToken.Append(ch);
							}
							else if (ch == '\'')
							{
								state = TokenizerState.Quoted;
								currentToken.Append(ch);
							}
							else
							{
								currentToken.Append(ch);
							}
							break;
						default:
							throw new InvalidExpressionException("Could not understand the string " + original);
					}
				}
				if (currentToken.Length > 0)
				{
					yield return currentToken.ToString();
				}
			}

			public IEnumerator GetEnumerator()
			{
				return ((IEnumerable<string>)this).GetEnumerator();
			}

			public enum TokenizerState
			{
				WhiteSpace,
				Quoted,
				InParenthesis,
				Token
			}

			public IList<string> GetTokens()
			{
				return new List<string>(this);
			}
		}
	}
}

