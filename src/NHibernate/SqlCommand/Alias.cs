using System;

using NHibernate.Dialect;

namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Aliases tables and fields for Sql Statements.
	/// </summary>
	public class Alias
	{
		private readonly int length;
		private readonly string suffix;

		public Alias(int length, string suffix)
		{
			this.length = (suffix==null) ? length : length - suffix.Length;
			this.suffix = suffix;
		}

		public Alias(string suffix)
		{
			this.length = int.MaxValue;
			this.suffix = suffix;
		}

		public string ToAliasString(string sqlIdentifier, Dialect.Dialect dialect) 
		{
			bool isQuoted = dialect.IsQuoted(sqlIdentifier);
			string unquoted;

			if(isQuoted) 
			{
				unquoted = dialect.UnQuote(sqlIdentifier);
			}
			else 
			{
				unquoted = sqlIdentifier;
			}

			if ( unquoted.Length > length ) 
			{
				unquoted = unquoted.Substring(0, length);
			}

			if (suffix!=null) unquoted += suffix;

			if ( isQuoted ) 
			{
				return dialect.QuoteForAliasName(unquoted);
			}
			else 
			{
				return unquoted;
			}

		}

		public string ToUnquotedAliasString(string sqlIdentifier, Dialect.Dialect dialect)
		{
			string unquoted = dialect.UnQuote(sqlIdentifier);

			if(unquoted.Length > length) 
			{
				unquoted = unquoted.Substring(0, length);
			}

			if(suffix!=null) unquoted += suffix;

			return unquoted;
		}

		public string[] ToUnquotedAliasStrings(string[] sqlIdentifiers, Dialect.Dialect dialect) 
		{
			string[] aliases = new string[sqlIdentifiers.Length];
			for(int i = 0; i < sqlIdentifiers.Length; i++) 
			{
				aliases[i] = ToUnquotedAliasString(sqlIdentifiers[i], dialect);
			}

			return aliases;
		}

		
		public string[] ToAliasStrings(string[] sqlIdentifiers, Dialect.Dialect dialect) 
		{
			string[] aliases = new string[ sqlIdentifiers.Length ];

			for ( int i=0; i<sqlIdentifiers.Length; i++ ) 
			{
				aliases[i] = ToAliasString(sqlIdentifiers[i], dialect);
			}
			return aliases;
		}
	}
}
