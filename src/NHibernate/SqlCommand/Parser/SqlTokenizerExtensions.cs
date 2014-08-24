using System;
using System.Collections.Generic;

namespace NHibernate.SqlCommand.Parser
{
	internal static class SqlTokenizerExtensions
	{
		public static bool TryParseUntil(this IEnumerator<SqlToken> tokenEnum, string keyword)
		{
			var nestLevel = 0;
			do
			{
				var token = tokenEnum.Current;
				if (token != null)
				{
					switch (token.TokenType)
					{
						case SqlTokenType.BracketOpen:
							nestLevel++;
							break;
						case SqlTokenType.BracketClose:
							nestLevel--;
							break;
						case SqlTokenType.Text:
							if (nestLevel == 0 && token.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)) return true;
							break;
					}
				}
			} while (tokenEnum.MoveNext());

			return false;
		}

		public static bool TryParseUntilFirstMsSqlSelectColumn(this IEnumerator<SqlToken> tokenEnum)
		{
			SqlToken selectToken;
			bool isDistinct;
			return TryParseUntilFirstMsSqlSelectColumn(tokenEnum, out selectToken, out isDistinct);
		}

		public static bool TryParseUntilFirstMsSqlSelectColumn(this IEnumerator<SqlToken> tokenEnum, out SqlToken selectToken, out bool isDistinct)
		{
			selectToken = null;
			isDistinct = false;

			while (tokenEnum.TryParseUntil("select"))
			{
				selectToken = tokenEnum.Current;
				if (!tokenEnum.MoveNext()) return false;

				// [ DISTINCT | ALL ]
				if (tokenEnum.Current.Equals("distinct", StringComparison.InvariantCultureIgnoreCase))
				{
					isDistinct = true;
					if (!tokenEnum.MoveNext()) return false;
				}
				else if	(tokenEnum.Current.Equals("all", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!tokenEnum.MoveNext()) return false;
				}

				// [ TOP { integer | ( expression ) } [PERCENT] [ WITH TIES ] ] 
				if (tokenEnum.Current.Equals("top", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!tokenEnum.MoveNext()) return false;
					if (tokenEnum.Current.TokenType == SqlTokenType.BracketOpen)
					{
						do
						{
							if (!tokenEnum.MoveNext()) return false;
						} while (tokenEnum.Current.TokenType != SqlTokenType.BracketClose);
					}
					if (!tokenEnum.MoveNext()) return false;

					if (tokenEnum.Current.Equals("percent", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!tokenEnum.MoveNext()) return false;
					}
					if (tokenEnum.Current.Equals("with", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!tokenEnum.MoveNext()) return false;
						if (tokenEnum.Current.Equals("ties", StringComparison.InvariantCultureIgnoreCase))
						{
							if (!tokenEnum.MoveNext()) return false;
						}
					}
				}

				if (!tokenEnum.Current.Value.StartsWith("@")) return true;
			}

			return false;
		}

		public static bool TryParseUntilFirstOrderColumn(this IEnumerator<SqlToken> tokenEnum, out SqlToken orderToken)
		{
			if (tokenEnum.TryParseUntil("order"))
			{
				orderToken = tokenEnum.Current;
				if (tokenEnum.MoveNext())
				{
					return tokenEnum.Current.Equals("by", StringComparison.InvariantCultureIgnoreCase)
						? tokenEnum.MoveNext()
						: false;
				}
			}

			orderToken = null;
			return false;
		}
	}
}
