using System;
using System.Text;
using System.Collections;

using Iesi.Collections;

using NHibernate.Dialect;
using NHibernate.Util;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Summary description for QuerySelect.
	/// </summary>
	public class QuerySelect 
	{		
		private JoinFragment joins;
		
		// the selectBuilder could probably be a string if the Persister's methods that build
		// the SqlString instead returned a String.
		private SqlStringBuilder selectBuilder = new SqlStringBuilder();
		private SqlStringBuilder whereBuilder = new SqlStringBuilder();
		
		// groupBy, orderBy, and having will for sure have no parameters.
		private StringBuilder groupBy = new StringBuilder();
		private StringBuilder orderBy = new StringBuilder();
		private StringBuilder having = new StringBuilder();
		private bool distinct = false;

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
		private static readonly ISet dontSpace = new HashedSet();

		static QuerySelect() 
		{
			//dontSpace.add("'");
			dontSpace.Add( "." );
			dontSpace.Add( "+" );
			dontSpace.Add( "-" );
			dontSpace.Add( "/" );
			dontSpace.Add( "*" );
			dontSpace.Add( "<" );
			dontSpace.Add( ">" );
			dontSpace.Add( "=" );
			dontSpace.Add( "#" );
			dontSpace.Add( "~" );
			dontSpace.Add( "|" );
			dontSpace.Add( "&" );
			dontSpace.Add( "<=" );
			dontSpace.Add( ">=" );
			dontSpace.Add( "=>" );
			dontSpace.Add( "=<" );
			dontSpace.Add( "!=" );
			dontSpace.Add( "<>" );
			dontSpace.Add( "!#" );
			dontSpace.Add( "!~" );
			dontSpace.Add( "!<" );
			dontSpace.Add( "!>" );
			// MySQL doesn't like spaces around "(" or ")" also.
			dontSpace.Add( StringHelper.OpenParen ); 
			dontSpace.Add( StringHelper.ClosedParen );

			dontSpace.Add( new SqlString( "." ) );
			dontSpace.Add( new SqlString( "+" ) );
			dontSpace.Add( new SqlString( "-" ) );
			dontSpace.Add( new SqlString( "/" ) );
			dontSpace.Add( new SqlString( "*" ) );
			dontSpace.Add( new SqlString( "<" ) );
			dontSpace.Add( new SqlString( ">" ) );
			dontSpace.Add( new SqlString( "=" ) );
			dontSpace.Add( new SqlString( "#" ) );
			dontSpace.Add( new SqlString( "~" ) );
			dontSpace.Add( new SqlString( "|" ) );
			dontSpace.Add( new SqlString( "&" ) );
			dontSpace.Add( new SqlString( "<=" ) );
			dontSpace.Add( new SqlString( ">=" ) );
			dontSpace.Add( new SqlString( "=>" ) );
			dontSpace.Add( new SqlString( "=<" ) );
			dontSpace.Add( new SqlString( "!=" ) );
			dontSpace.Add( new SqlString( "<>" ) );
			dontSpace.Add( new SqlString( "!#" ) );
			dontSpace.Add( new SqlString( "!~" ) );
			dontSpace.Add( new SqlString( "!<" ) );
			dontSpace.Add( new SqlString( "!>" ) );
			// MySQL doesn't like spaces around "(" or ")" also.
			dontSpace.Add( new SqlString( StringHelper.OpenParen ) ); 
			dontSpace.Add( new SqlString( StringHelper.ClosedParen ) );
		}

		public QuerySelect(Dialect.Dialect dialect) 
		{
			joins = new QueryJoinFragment(dialect, false);
		}

		public JoinFragment JoinFragment 
		{
			get { return joins; }
		}
	
		public void AddSelectFragmentString(string fragment) 
		{
			AddSelectFragmentString( new SqlString(fragment) );
		}
	
		public void AddSelectFragmentString(SqlString fragment) 
		{
			if( fragment.SqlParts.Length>0 && fragment.StartsWith(",") )
			{
				fragment = fragment.Substring(1);
			}

			fragment = fragment.Trim();

			if( fragment.SqlParts.Length > 0 ) 
			{
				if( selectBuilder.Count > 0 ) 
				{
					selectBuilder.Add(StringHelper.CommaSpace);
				}
				
				selectBuilder.Add(fragment);
			}
		}

		public void AddSelectColumn(string columnName, string alias) 
		{
			AddSelectFragmentString(columnName + ' ' + alias);
		}
	
		public bool Distinct 
		{
			get { return distinct; }
			set { this.distinct = value; }
		}
	
		public void SetWhereTokens(ICollection tokens) 
		{
			//if ( conjunctiveWhere.length()>0 ) conjunctiveWhere.append(" and ");
			AppendTokens(whereBuilder, tokens);
			//AppendTokens(where, tokens);
		}
		
		public void SetGroupByTokens(ICollection tokens)
		{
			//if ( groupBy.length()>0 ) groupBy.append(" and ");
			AppendTokens(groupBy, tokens); 
		}
		
		public void SetOrderByTokens(ICollection tokens) 
		{
			//if ( orderBy.length()>0 ) orderBy.append(" and ");
			AppendTokens(orderBy, tokens);
		}
		
		public void SetHavingTokens(ICollection tokens)
		{
			//if ( having.length()>0 ) having.append(" and ");
			AppendTokens(having, tokens); 
		}

		public void AddOrderBy(string orderByString) 
		{
			if( orderBy.Length > 0 ) orderBy.Append(StringHelper.CommaSpace);
			orderBy.Append(orderByString);
		}

		public SqlString ToQuerySqlString() 
		{
			SqlStringBuilder builder = new SqlStringBuilder();
			
			builder.Add("select ");

			if (distinct) builder.Add("distinct ");
			
			SqlString from = joins.ToFromFragmentString;
			if ( from.StartsWith(",") ) 
			{
				from = from.Substring(1);
			}
			else if ( from.StartsWith(" inner join") ) 
			{
				from = from.Substring(11);
			}

			builder.Add(selectBuilder.ToSqlString())
				.Add(" from")
				.Add( from );
			
			SqlString part1 = joins.ToWhereFragmentString.Trim();
			SqlString part2 =  whereBuilder.ToSqlString().Trim();
			bool hasPart1 = part1.SqlParts.Length > 0;
			bool hasPart2 = part2.SqlParts.Length > 0;
			
			if (hasPart1 || hasPart2) builder.Add(" where ");
			if (hasPart1) builder.Add( part1.Substring(4) );
			if (hasPart2) 
			{
				if (hasPart1) builder.Add(" and (");
				builder.Add(part2);
				if (hasPart1) builder.Add(")");
			}
			if ( groupBy.Length > 0 ) builder.Add(" group by ").Add( groupBy.ToString() );
			if ( having.Length > 0 ) builder.Add(" having ").Add( having.ToString() );
			if ( orderBy.Length > 0 ) builder.Add(" order by ").Add( orderBy.ToString() );
			return builder.ToSqlString();
		}

		private void AppendTokens(StringBuilder buf, ICollection iter) 
		{
			bool lastSpaceable = true;
			bool lastQuoted = false;

			int debugIndex = 0;
			foreach(string token in iter) 
			{
				bool spaceable = !dontSpace.Contains(token);
				bool quoted = token.StartsWith("'");

				if (spaceable && lastSpaceable) 
				{
					if (!quoted || !lastQuoted) buf.Append(' ');
				}

				lastSpaceable = spaceable;
				buf.Append(token);
				lastQuoted = token.EndsWith("'");
				debugIndex++;
			}
		}

		private void AppendTokens(SqlStringBuilder builder, ICollection iter) 
		{
			bool lastSpaceable = true;
			bool lastQuoted = false;

			int debugIndex = 0;
			foreach(object token in iter) 
			{
				string tokenString = token as string;
				SqlString tokenSqlString = token as SqlString;

				bool spaceable = !dontSpace.Contains(token);
				bool quoted = false;

				//TODO: seems HACKish to cast between String and SqlString
				if(tokenString!=null) 
				{
					quoted = tokenString.StartsWith("'");
				}
				else 
				{
					quoted = tokenSqlString.StartsWith("'");
				}

				if (spaceable && lastSpaceable) 
				{
					if (!quoted || !lastQuoted) builder.Add(" ");
				}

				lastSpaceable = spaceable;
				
				if( token.Equals(StringHelper.SqlParameter) ) 
				{
					Parameter param = new Parameter();
					param.Name = "placholder";
					builder.Add(param);
				}
				else 
				{
					// not sure if we have a string or a SqlString here and token is a 
					// reference to an object - so let the builder figure out what the
					// actual object is
					builder.AddObject(token);
				}
				debugIndex++;

				if( tokenString!=null ) 
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