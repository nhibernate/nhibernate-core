using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Summary description for QuerySelect.
	/// </summary>
	public class QuerySelect
	{
		private readonly JoinFragment joins;

		private readonly SqlStringBuilder selectBuilder = new SqlStringBuilder();
		private readonly SqlStringBuilder whereBuilder = new SqlStringBuilder();

		// groupBy, orderBy, and having will for sure have no parameters.
		private readonly SqlStringBuilder groupBy = new SqlStringBuilder();
		private readonly SqlStringBuilder orderBy = new SqlStringBuilder();
		private readonly SqlStringBuilder having = new SqlStringBuilder();

		// false by default
		private bool distinct;

		/// <summary>
		/// Certain databases don't like spaces around these operators.
		/// </summary>
		/// <remarks>
		/// This needs to contain both a plain string and a 
		/// SqlString version of the operator because the portions in 
		/// the WHERE clause will come in as SqlStrings since there
		/// might be parameters, other portions of the clause come in
		/// as strings since there are no parameters.
		/// </remarks>
		private static readonly HashSet<object> dontSpace = new HashSet<object>();

		/// <summary></summary>
		static QuerySelect()
		{
			//dontSpace.add("'");
			dontSpace.Add(".");
			dontSpace.Add("+");
			dontSpace.Add("-");
			dontSpace.Add("/");
			dontSpace.Add("*");
			dontSpace.Add("<");
			dontSpace.Add(">");
			dontSpace.Add("=");
			dontSpace.Add("#");
			dontSpace.Add("~");
			dontSpace.Add("|");
			dontSpace.Add("&");
			dontSpace.Add("<=");
			dontSpace.Add(">=");
			dontSpace.Add("=>");
			dontSpace.Add("=<");
			dontSpace.Add("!=");
			dontSpace.Add("<>");
			dontSpace.Add("!#");
			dontSpace.Add("!~");
			dontSpace.Add("!<");
			dontSpace.Add("!>");
			// MySQL doesn't like spaces around "(" or ")" also.
			dontSpace.Add(StringHelper.OpenParen);
			dontSpace.Add(StringHelper.ClosedParen);

			dontSpace.Add(new SqlString("."));
			dontSpace.Add(new SqlString("+"));
			dontSpace.Add(new SqlString("-"));
			dontSpace.Add(new SqlString("/"));
			dontSpace.Add(new SqlString("*"));
			dontSpace.Add(new SqlString("<"));
			dontSpace.Add(new SqlString(">"));
			dontSpace.Add(new SqlString("="));
			dontSpace.Add(new SqlString("#"));
			dontSpace.Add(new SqlString("~"));
			dontSpace.Add(new SqlString("|"));
			dontSpace.Add(new SqlString("&"));
			dontSpace.Add(new SqlString("<="));
			dontSpace.Add(new SqlString(">="));
			dontSpace.Add(new SqlString("=>"));
			dontSpace.Add(new SqlString("=<"));
			dontSpace.Add(new SqlString("!="));
			dontSpace.Add(new SqlString("<>"));
			dontSpace.Add(new SqlString("!#"));
			dontSpace.Add(new SqlString("!~"));
			dontSpace.Add(new SqlString("!<"));
			dontSpace.Add(new SqlString("!>"));
			// MySQL doesn't like spaces around "(" or ")" also.
			dontSpace.Add(new SqlString(StringHelper.OpenParen));
			dontSpace.Add(new SqlString(StringHelper.ClosedParen));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		public QuerySelect(Dialect.Dialect dialect)
		{
			joins = new QueryJoinFragment(dialect, false);
		}

		/// <summary></summary>
		public JoinFragment JoinFragment
		{
			get { return joins; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fragment"></param>
		public void AddSelectFragmentString(SqlString fragment)
		{
			if (fragment.StartsWithCaseInsensitive(","))
			{
				fragment = fragment.Substring(1);
			}

			fragment = fragment.Trim();

			if (fragment.Length > 0)
			{
				if (selectBuilder.Count > 0)
				{
					selectBuilder.Add(StringHelper.CommaSpace);
				}

				selectBuilder.Add(fragment);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="alias"></param>
		public void AddSelectColumn(string columnName, string alias)
		{
			AddSelectFragmentString(new SqlString(columnName + ' ' + alias));
		}

		/// <summary></summary>
		public bool Distinct
		{
			get { return distinct; }
			set { distinct = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tokens"></param>
		public void SetWhereTokens(IEnumerable tokens)
		{
			//if ( conjunctiveWhere.length()>0 ) conjunctiveWhere.append(" and ");
			AppendTokens(whereBuilder, tokens);
			//AppendTokens(where, tokens);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tokens"></param>
		public void SetGroupByTokens(IEnumerable tokens)
		{
			//if ( groupBy.length()>0 ) groupBy.append(" and ");
			AppendTokens(groupBy, tokens);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tokens"></param>
		public void SetOrderByTokens(IEnumerable tokens)
		{
			//if ( orderBy.length()>0 ) orderBy.append(" and ");
			AppendTokens(orderBy, tokens);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tokens"></param>
		public void SetHavingTokens(IEnumerable tokens)
		{
			//if ( having.length()>0 ) having.append(" and ");
			AppendTokens(having, tokens);
		}

		/// <summary>
		/// Adds a string containing a valid "order by" sql statement
		/// to this QuerySelect
		/// </summary>
		/// <param name="orderBySql">The "order by" sql statement.</param>
		public void AddOrderBy(string orderBySql)
		{
			if (orderBy.Count > 0)
			{
				orderBy.Add(StringHelper.CommaSpace);
			}
			orderBy.Add(orderBySql);
		}

		public SqlString ToQuerySqlString()
		{
			var builder = new SqlStringBuilder();

			builder.Add("select ");

			if (distinct)
			{
				builder.Add("distinct ");
			}

			SqlString from = joins.ToFromFragmentString;
			if (from.StartsWithCaseInsensitive(","))
			{
				from = from.Substring(1);
			}
			else if (from.StartsWithCaseInsensitive(" inner join"))
			{
				from = from.Substring(11);
			}

			builder.Add(selectBuilder.ToSqlString())
				.Add(" from")
				.Add(from);

			SqlString part1 = joins.ToWhereFragmentString.Trim();
			SqlString part2 = whereBuilder.ToSqlString();
			bool hasPart1 = part1.Count > 0;
			bool hasPart2 = part2.Count > 0;

			if (hasPart1 || hasPart2)
			{
				builder.Add(" where ");
			}
			if (hasPart1)
			{
				builder.Add(part1.Substring(4));
			}
			if (hasPart2)
			{
				if (hasPart1)
				{
					builder.Add(" and (");
				}
				builder.Add(part2);
				if (hasPart1)
				{
					builder.Add(")");
				}
			}
			if (groupBy.Count > 0)
			{
				builder.Add(" group by ").Add(groupBy.ToSqlString());
			}
			if (having.Count > 0)
			{
				builder.Add(" having ").Add(having.ToSqlString());
			}
			if (orderBy.Count > 0)
			{
				builder.Add(" order by ").Add(orderBy.ToSqlString());
			}
			return builder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="iter"></param>
		private static void AppendTokens(SqlStringBuilder builder, IEnumerable iter)
		{
			bool lastSpaceable = true;
			bool lastQuoted = false;

			foreach (object token in iter)
			{
				var tokenString = token as string;
				var tokenSqlString = token as SqlString;

				bool spaceable = !dontSpace.Contains(token);
				bool quoted;

				//TODO: seems HACKish to cast between String and SqlString
				if (tokenString != null)
				{
					quoted = tokenString.StartsWith("'");
				}
				else
				{
					quoted = tokenSqlString.StartsWithCaseInsensitive("'");
				}

				if (spaceable && lastSpaceable)
				{
					if (!quoted || !lastQuoted)
					{
						builder.Add(" ");
					}
				}

				lastSpaceable = spaceable;

				if (token.Equals(StringHelper.SqlParameter))
				{
					Parameter param = Parameter.Placeholder;
					builder.Add(param);
				}
				else
				{
					// not sure if we have a string or a SqlString here and token is a 
					// reference to an object - so let the builder figure out what the
					// actual object is
					builder.AddObject(token);
				}

				if (tokenString != null)
				{
					lastQuoted = tokenString.EndsWith("'");
				}
				else
				{
					lastQuoted = tokenSqlString.EndsWith("'");
				}
			}
		}
	}
}
