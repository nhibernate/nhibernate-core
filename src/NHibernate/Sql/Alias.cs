using System;

using NHibernate.Dialect;

namespace NHibernate.Sql
{
	/// <summary>
	/// Summary description for Alias.
	/// </summary>
	public sealed class Alias
	{
		private readonly int length;
		private readonly string suffix;

		public Alias(int length, string suffix) : base() {
			this.length = (suffix==null) ? length : length - suffix.Length;
			this.suffix = suffix;
		}

		public Alias(string suffix): base() {
			this.length = int.MaxValue;
			this.suffix = suffix;
		}

		public string ToAliasString(string sqlIdentifier) {
			char begin = sqlIdentifier[0];
			int quoteType = Dialect.Dialect.Quote.IndexOf(begin);

			string unquoted;

			if ( quoteType>=0 ) {
				unquoted = sqlIdentifier.Substring(1, sqlIdentifier.Length-1 );
			}
			else {
				unquoted = sqlIdentifier;
			}

			if ( unquoted.Length > length ) {
				unquoted = unquoted.Substring(0, length);
			}

			if (suffix!=null) unquoted += suffix;

			if ( quoteType >= 0 ) {
				char endQuote = Dialect.Dialect.ClosedQuote[quoteType];
				return endQuote + unquoted + endQuote;
			}
			else {
				return unquoted;
			}
		}

		public string[] ToAliasStrings(string[] sqlIdentifiers) {

			string[] aliases = new string[ sqlIdentifiers.Length ];

			for ( int i=0; i<sqlIdentifiers.Length; i++ ) {
				aliases[i] = ToAliasString(sqlIdentifiers[i]);
			}
			return aliases;
		}
	}
}