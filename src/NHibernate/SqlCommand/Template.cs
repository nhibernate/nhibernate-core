using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.SqlCommand 
{

	/// <summary>
	/// Summary description for Template.
	/// </summary>
	public sealed class Template 
	{

		private static StringDictionary Keywords = new StringDictionary();
		private static readonly string empty = "";
		public static readonly string PlaceHolder = "$PlaceHolder";
		private static ArrayList delimiterList = new ArrayList(13);
		private static string delimiters = null;

		static Template() 
		{
			Keywords["and"] = empty;
			Keywords["or"] = empty;
			Keywords["not"] = empty;
			Keywords["like"] = empty;
			Keywords["is"] = empty;
			Keywords["in"] = empty;
			Keywords["between"] = empty;
			Keywords["null"] = empty;
			Keywords["select"] = empty;
			Keywords["from"] = empty;
			Keywords["where"] = empty;
			Keywords["having"] = empty;
			Keywords["group"] = empty;
			Keywords["order"] = empty;
			Keywords["by"] = empty;

			delimiterList.Add(" ");
			delimiterList.Add("=");
			delimiterList.Add(">");
			delimiterList.Add("<");
			delimiterList.Add("!");
			delimiterList.Add("+");
			delimiterList.Add("-");
			delimiterList.Add("*");
			delimiterList.Add("/");
			delimiterList.Add("(");
			delimiterList.Add(")");
			delimiterList.Add("'");
			delimiterList.Add(",");

			foreach(string delim in delimiterList) 
			{
				delimiters += delim; 
			}

		}

		private Template() 
		{
		}

		/// <summary>
		/// Add a Keyword to this Template. 
		/// </summary>
		/// <param name="keyword">The Keyword to add.</param>
		public static void AddKeyword(string keyword) 
		{
			Keywords[keyword] = empty;
		}

		/// <summary>
		/// Add a Delimeter to use when parsing the string.
		/// </summary>
		/// <param name="delimiter">The delimiter to add.</param>
		public static void AddDelimiter(string delimiter) 
		{
			if ( !delimiterList.Contains(delimiter) ) 
			{
				delimiterList.Add(delimiter);
				delimiters += delimiter; 
			}
		}
		
		/// <summary>
		/// Takes the where condition provided in the mapping attribute and iterpolates the alias.
		/// </summary>
		/// <param name="sqlWhereString"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public static string RenderWhereStringTemplate(string sqlWhereString, Dialect.Dialect dialect) 
		{
			//TODO: make this a bit nicer.
			
			StringTokenizer tokens = new StringTokenizer(sqlWhereString, delimiters, true);
			
			StringBuilder result = new StringBuilder(sqlWhereString.Length + 10);

			bool quoted = false;
			bool afterFrom = false;
			bool afterFromTable = false;

			IEnumerator tokensEnum = tokens.GetEnumerator();
			bool hasMore = tokensEnum.MoveNext();
			string nextToken = hasMore ? (string)tokensEnum.Current : null;

			while(hasMore) 
			{
				string token = nextToken;
				hasMore = tokensEnum.MoveNext();
				nextToken = hasMore ? (string)tokensEnum.Current : null;

				// the begin or end "'" has been found
				if ( "'".Equals(token) ) quoted = !quoted;
				
				if (quoted || char.IsWhiteSpace(token[0]) ) 
				{
					result.Append(token);
				}
				else 
				{
					bool isIdentifier = token[0]=='`' || ( // allow any identifier quoted with backtick
						Char.IsLetter(token[0]) && // only recognizes identifiers beginning with a letter
						!Keywords.ContainsKey( token.ToLower() ) &&
						token.IndexOf('.') < 0
						);

					if (afterFrom) 
					{
						result.Append(token);
						afterFrom = false;
						afterFromTable = true;
					}
					else if (afterFromTable) 
					{
						afterFromTable = false;
						result.Append(token);
					}
					else if ( isIdentifier && ( nextToken==null || !nextToken.Equals("(") )) // not a function call
					{
						result.Append(PlaceHolder)
							.Append(StringHelper.Dot)
							.Append( Quote(token, dialect) );

					}
					else 
					{
						if ( "from".Equals( token.ToLower() ) ) afterFrom = true;
						result.Append(token);
					}
				}	
			}
			
			return result.ToString();

		}

		/// <summary>
		/// Takes order-by clause in the mapping attribute and iterpolates the alias
		/// </summary>
		/// <param name="sqlOrderByString"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public static string RenderOrderByStringTemplate(string sqlOrderByString, Dialect.Dialect dialect) 
		{
			//TODO: make this a bit nicer
			StringTokenizer tokens = new StringTokenizer(sqlOrderByString, ",");
			StringBuilder result = new StringBuilder(sqlOrderByString.Length + 2);
			bool commaNeeded = false; 
			IEnumerator tokenEnum = tokens.GetEnumerator();

			while( tokenEnum.MoveNext() ) 
			{
				string column = Quote(((string)tokenEnum.Current).Trim(), dialect);

				if (commaNeeded) result.Append(StringHelper.CommaSpace);
				commaNeeded = true;

				result.Append(PlaceHolder)
					.Append(StringHelper.Dot)
					.Append(column);
			}

			return result.ToString();
		}

		private static string Quote(string column, Dialect.Dialect dialect) 
		{

			if ( column[0] == '`' ) 
			{
				if (column[column.Length -1] != '`')
					throw new ArgumentException("missing ` in column " + column);
				return dialect.QuoteForAliasName(column.Substring(1, column.Length - 2));
			}
			else 
			{
				return dialect.QuoteForAliasName(column);
			}
		}

	}
}
