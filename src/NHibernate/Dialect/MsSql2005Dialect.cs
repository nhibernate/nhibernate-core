namespace NHibernate.Dialect
{
using System;
	using System.Collections;
	using System.Collections.Generic;
using System.Data;
	using System.Text.RegularExpressions;
	using System.Text;
	using Mapping;
	using SqlCommand;
	using Util;

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
		/// SELECT TOP last (columns) FROM (
		/// SELECT ROW_NUMBER() OVER(ORDER BY __hibernate_sort_expr_1__ {sort direction 1} [, __hibernate_sort_expr_2__ {sort direction 2}, ...]) as row, (query.columns) FROM (
		///		{original select query part}, {sort field 1} as __hibernate_sort_expr_1__ [, {sort field 2} as __hibernate_sort_expr_2__, ...]
		///		{remainder of original query minus the order by clause}
		/// ) query
		/// ) page WHERE page.row > offset
		/// 
		/// </code>
		/// 
		/// Note that we need to add explicitly specify the columns, because we need to be able to use them
		/// in a paged subselect. NH-1155
		/// </remarks>
		public override SqlString GetLimitString(SqlString querySqlString, int offset, int last)
		{
			int fromIndex = GetFromIndex(querySqlString);
			SqlString select = querySqlString.Substring(0, fromIndex);
			List<string> columnsOrAliases;
			Dictionary<string, string> aliasToColumn;
			ExtractColumnOrAliasNames(select, out columnsOrAliases, out aliasToColumn);

			int orderIndex = querySqlString.LastIndexOfCaseInsensitive(" order by ");
			SqlString from;
			string[] sortExpressions;
			if (orderIndex > 0)
			{
				from = querySqlString.Substring(fromIndex, orderIndex - fromIndex).Trim();
				string orderBy = querySqlString.Substring(orderIndex).ToString().Trim();
				sortExpressions = orderBy.Substring(9).Split(',');
			}
			else
			{
				from = querySqlString.Substring(fromIndex).Trim();
				// Use dummy sort to avoid errors
				sortExpressions = new string[] { "CURRENT_TIMESTAMP" };
			}

			SqlStringBuilder result = new SqlStringBuilder()
				.Add("SELECT TOP ")
				.Add(last.ToString())
				.Add(" ")
				.Add(StringHelper.Join(", ", columnsOrAliases))
				.Add(" FROM (SELECT ROW_NUMBER() OVER(ORDER BY ");

			AppendSortExpressions(sortExpressions, result);

			result.Add(") as row, ");

			for (int i = 0; i < columnsOrAliases.Count; i++)
			{
				result.Add("query.").Add(columnsOrAliases[i]);
				bool notLastColumn = i != columnsOrAliases.Count - 1;
				if (notLastColumn)
					result.Add(", ");
			}

			for (int i = 1; i <= sortExpressions.Length; ++i)
			{
					result.Add(", query.__hibernate_sort_expr_")
						.Add(i.ToString())
						.Add("__");
			}

			result.Add(" FROM (")
				.Add(select);

			for (int i = 1; i <= sortExpressions.Length; i++)
			{
				// Drop the ASC/DESC at the end of the sort expression which might look like "count(distinct frog.Id)desc" or "frog.Name asc".
				string sortExpression = Regex.Replace(sortExpressions[i - 1].Trim(), @"(\)|\s)(?i:asc|desc)$", "$1").Trim();

				if (aliasToColumn.ContainsKey(sortExpression))
					sortExpression = aliasToColumn[sortExpression];

				result.Add(", ")
					.Add(sortExpression)
					.Add(" as __hibernate_sort_expr_")
					.Add(i.ToString())
					.Add("__");
			}

			result.Add(" ")
				.Add(from)
				.Add(") query ) page WHERE page.row > ")
				.Add(offset.ToString())
				.Add(" ORDER BY ");

			AppendSortExpressions(sortExpressions, result);

			return result.ToSqlString();
		}

		private void AppendSortExpressions(string[] sortExpressions, SqlStringBuilder result)
		{
			for (int i = 1; i <= sortExpressions.Length; i++)
			{
				if (i > 1)
					result.Add(", ");

				result.Add("__hibernate_sort_expr_")
					.Add(i.ToString())
					.Add("__");

				if (sortExpressions[i - 1].Trim().ToLower().EndsWith("desc"))
					result.Add(" DESC");
			}
		}

		private int GetFromIndex(SqlString querySqlString)
		{
			string subselect = querySqlString.GetSubselectString().ToString();
			int fromIndex = querySqlString.IndexOfCaseInsensitive(subselect);
			if(fromIndex == -1)
			{
				fromIndex = querySqlString.ToString().ToLowerInvariant().IndexOf(subselect.ToLowerInvariant());
			}
			return fromIndex;
		}

		private static void ExtractColumnOrAliasNames(SqlString select,
			out List<string> columnsOrAliases,
			out Dictionary<string, string> aliasToColumn)
		{
			columnsOrAliases = new List<string>();
			aliasToColumn = new Dictionary<string, string>();

			IList<string> tokens = new QuotedAndParenthesisStringTokenizer(select.ToString()).GetTokens();
			int index = 0;
			while (index < tokens.Count)
			{
				string token = tokens[index];
				index += 1;

				if ("select".Equals(token, StringComparison.InvariantCultureIgnoreCase))
					continue;
				if ("distinct".Equals(token, StringComparison.InvariantCultureIgnoreCase))
					continue;
				if ("," == token)
					continue;
					
				if ("from".Equals(token, StringComparison.InvariantCultureIgnoreCase))
					break;

				//handle composite expressions like 2 * 4 as foo
				while (index < tokens.Count && 
					"as".Equals(tokens[index],StringComparison.InvariantCultureIgnoreCase) == false &&
					"," != tokens[index])
				{
					token = token + " " + tokens[index];
					index += 1;
				}

				bool isFunctionCallOrQuotedString = token.Contains("'") || token.Contains("(");
				// this is heuristic guess, if the expression contains ' or (, it is probably
				// not appropriate to just slice parts off of it
				if (isFunctionCallOrQuotedString == false) 
				{
					int dot = token.IndexOf('.');
					if (dot != -1)
						token = token.Substring(dot + 1);
				}

				string alias = token;

				// notice! we are checking here the existence of "as" "alias", two
				// tokens from the current one
				if (index + 1 < tokens.Count &&
					"as".Equals(tokens[index], StringComparison.InvariantCultureIgnoreCase))
				{
					alias = tokens[index + 1];
					index += 2; //skip the "as" and the alias	\
				}

				columnsOrAliases.Add(alias);
				aliasToColumn[alias] = token;
			}
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset
		{
			get
			{
				return true;
			}
		}

		protected override string GetSelectExistingObject(string name, Table table)
		{
			string objName = table.GetQuotedSchemaName(this) + Quote(name);
			return string.Format("select 1 from sys.objects where object_id = OBJECT_ID(N'{0}') AND parent_object_id = OBJECT_ID('{1}')",
								 objName, table.GetQuotedName(this));
		}


		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit
		{
			get
			{
				return false;
			}
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
							else if (ch == ',')// stop current token, and send the , as well
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
				if (currentToken.Length>0)
					yield return currentToken.ToString();
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
