namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Aliases tables and fields for Sql Statements.
	/// </summary>
	/// <remarks>
	/// Several methods of this class take an additional
	/// <see cref="Dialect.Dialect" /> parameter, while their Java counterparts
	/// do not. The dialect is used to correctly quote and unquote identifiers.
	/// Java versions do the quoting and unquoting themselves and fail to
	/// consider dialect-specific rules, such as escaping closing brackets in
	/// identifiers on MS SQL 2000.
	/// </remarks>
	public class Alias
	{
		private readonly int length;
		private readonly string suffix;
		private static readonly char[] Quotes = (Dialect.Dialect.PossibleQuoteChars + Dialect.Dialect.PossibleClosedQuoteChars).ToCharArray();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length"></param>
		/// <param name="suffix"></param>
		public Alias(int length, string suffix)
		{
			this.length = (suffix == null) ? length : length - suffix.Length;
			this.suffix = suffix;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="suffix"></param>
		public Alias(string suffix)
		{
			this.length = int.MaxValue;
			this.suffix = suffix;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlIdentifier"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string ToAliasString(string sqlIdentifier, Dialect.Dialect dialect)
		{
			bool isQuoted = dialect.IsQuoted(sqlIdentifier);
			string unquoted;

			if (isQuoted)
			{
				unquoted = dialect.UnQuote(sqlIdentifier);
			}
			else
			{
				unquoted = sqlIdentifier;
			}

			// Oracle doesn't like underscores at the start of identifiers (NH-320).
			// It should be safe to trim them here, because the aliases are postfixed
			// with a unique number anyway, so they won't collide.
			unquoted = unquoted.TrimStart('_');

			if (unquoted.Length > length)
			{
				unquoted = unquoted.Substring(0, length);
			}

			if (suffix != null)
			{
				unquoted += suffix;
			}

			if (isQuoted)
			{
				return dialect.QuoteForAliasName(unquoted);
			}
			else
			{
				return unquoted;
			}
		}

		public string ToAliasString(string sqlIdentifier)
		{
			var isQuoted = IsQuoted(sqlIdentifier, out var openQuote, out var closeQuote);

			var unquoted = isQuoted ? sqlIdentifier.Substring(1, sqlIdentifier.Length - 2) : sqlIdentifier;

			// Oracle doesn't like underscores at the start of identifiers (NH-320).
			// It should be safe to trim them here, because the aliases are postfixed
			// with a unique number anyway, so they won't collide.
			unquoted = unquoted.TrimStart('_');

			if (unquoted.Length > length)
			{
				unquoted = unquoted.Substring(0, length);
				unquoted = unquoted.TrimEnd(Quotes);
			}

			if (!string.IsNullOrEmpty(suffix))
			{
				unquoted += suffix;
			}

			if (isQuoted)
			{
				return openQuote +
					   unquoted +
					   closeQuote;
			}

			return unquoted;
		}

		private static bool IsQuoted(string sqlIdentifier, out char openQuote, out char closeQuote)
		{
			if (sqlIdentifier == null || sqlIdentifier.Length < 2)
			{
				openQuote = default(char);
				closeQuote = default(char);
				return false;
			}

			openQuote = sqlIdentifier[0];
			closeQuote = sqlIdentifier[sqlIdentifier.Length - 1];
			var quoteType = Dialect.Dialect.PossibleQuoteChars.IndexOf(openQuote);

			return quoteType >= 0 &&
				   closeQuote == Dialect.Dialect.PossibleClosedQuoteChars[quoteType];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlIdentifier"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string ToUnquotedAliasString(string sqlIdentifier, Dialect.Dialect dialect)
		{
			string unquoted = dialect.UnQuote(sqlIdentifier);

			// See comment in ToAliasString above
			unquoted = unquoted.TrimStart('_');

			if (unquoted.Length > length)
			{
				unquoted = unquoted.Substring(0, length);
			}

			if (suffix != null)
			{
				unquoted += suffix;
			}

			return unquoted;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlIdentifiers"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string[] ToUnquotedAliasStrings(string[] sqlIdentifiers, Dialect.Dialect dialect)
		{
			string[] aliases = new string[sqlIdentifiers.Length];
			for (int i = 0; i < sqlIdentifiers.Length; i++)
			{
				aliases[i] = ToUnquotedAliasString(sqlIdentifiers[i], dialect);
			}

			return aliases;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sqlIdentifiers"></param>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public string[] ToAliasStrings(string[] sqlIdentifiers, Dialect.Dialect dialect)
		{
			string[] aliases = new string[sqlIdentifiers.Length];

			for (int i = 0; i < sqlIdentifiers.Length; i++)
			{
				aliases[i] = ToAliasString(sqlIdentifiers[i], dialect);
			}
			return aliases;
		}
	}
}
