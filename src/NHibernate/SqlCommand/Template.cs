using System;
using System.Collections;
using System.Text;

using Iesi.Collections;

using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.SqlCommand 
{

	/// <summary>
	/// Summary description for Template.
	/// </summary>
	public sealed class Template 
	{
		public const string PlaceHolder = "$PlaceHolder";
		
		private static ISet Keywords = new HashedSet();
		private static ArrayList delimiterList = new ArrayList(13);
		private static string delimiters = null;

		static Template() 
		{
			Keywords.Add( "and" );
			Keywords.Add( "or" );
			Keywords.Add( "not" );
			Keywords.Add( "like" );
			Keywords.Add( "is" );
			Keywords.Add( "in" );
			Keywords.Add( "between" );
			Keywords.Add( "null" );
			Keywords.Add( "select" );
			Keywords.Add( "from" );
			Keywords.Add( "where" );
			Keywords.Add( "having" );
			Keywords.Add( "group" );
			Keywords.Add( "order" );
			Keywords.Add( "by" );

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
			Keywords.Add( keyword );
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
						!Keywords.Contains( token.ToLower() ) &&
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
			StringTokenizer tokens = new StringTokenizer(sqlOrderByString, ",", false);
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
				return column;
			}
		}

	}
}
