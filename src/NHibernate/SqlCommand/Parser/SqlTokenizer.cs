using System;
using System.Collections.Generic;
using System.Collections;

namespace NHibernate.SqlCommand.Parser
{
	/// <summary>
	/// Splits a <see cref="SqlString"/> into <see cref="SqlToken"/>s.
	/// </summary>
	public class SqlTokenizer : IEnumerable<SqlToken>
	{
		private readonly SqlString _sql;
		private SqlTokenType _includeTokens = SqlTokenType.AllExceptWhitespaceOrComment;

		public SqlTokenizer(SqlString sql)
		{
			if (sql == null) throw new ArgumentNullException("sql");
			_sql = sql;
		}

		public bool IgnoreWhitespace
		{
			get { return !CanYield(SqlTokenType.Whitespace); }
			set { Ignore(SqlTokenType.Whitespace, value); }
		}

		public bool IgnoreComments
		{
			get { return !CanYield(SqlTokenType.Comment); }
			set { Ignore(SqlTokenType.Comment, value); }
		}

		private bool CanYield(SqlTokenType tokenType)
		{
			return (_includeTokens & tokenType) == tokenType;
		}

		private void Ignore(SqlTokenType tokenType, bool canYield)
		{
			_includeTokens = canYield
				? _includeTokens & ~tokenType
				: _includeTokens | tokenType;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<SqlToken> GetEnumerator()
		{
			int sqlIndex = 0;
			foreach (var part in _sql)
			{
				var parameter = part as Parameter;
				if (parameter != null)
				{
					if (CanYield(SqlTokenType.Parameter))
					{
						yield return new SqlToken(SqlTokenType.Parameter, _sql, sqlIndex, 1);
					}
					sqlIndex++;
					continue;
				}

				var text = part as string;
				if (text != null)
				{
					int offset = 0;
					int maxOffset = text.Length;
					int tokenOffset = 0;
					SqlTokenType nextTokenType = 0;
					int nextTokenLength = 0;

					while (offset < maxOffset)
					{
						var ch = text[offset];
						switch (ch)
						{
							case '(':
								nextTokenType = SqlTokenType.BracketOpen;
								nextTokenLength = 1;
								break;
							case ')':
								nextTokenType = SqlTokenType.BracketClose;
								nextTokenLength = 1;
								break;
							case '\'':      // String literals
							case '\"':      // ANSI quoted identifiers
							case '[':       // Sql Server quoted indentifiers
								nextTokenType = SqlTokenType.DelimitedText;
								nextTokenLength = SqlParserUtils.ReadDelimitedText(text, maxOffset, offset);
								break;
							case ',':
								nextTokenType = SqlTokenType.Comma;
								nextTokenLength = 1;
								break;
							case '/':
								if (offset + 1 < maxOffset && text[offset + 1] == '*')
								{
									nextTokenType = SqlTokenType.Comment;
									nextTokenLength = SqlParserUtils.ReadMultilineComment(text, maxOffset, offset);
								}
								break;
							case '-':
								if (offset + 1 < maxOffset && text[offset + 1] == '-')
								{
									nextTokenType = SqlTokenType.Comment;
									nextTokenLength = SqlParserUtils.ReadLineComment(text, maxOffset, offset);
								}
								break;
							default:
								if (char.IsWhiteSpace(ch))
								{
									nextTokenType = SqlTokenType.Whitespace;
									nextTokenLength = SqlParserUtils.ReadWhitespace(text, maxOffset, offset);
								}
								break;
						}

						if (nextTokenType != 0)
						{
							if (offset > tokenOffset)
							{
								if (CanYield(SqlTokenType.Text))
								{
									yield return new SqlToken(SqlTokenType.Text, _sql, sqlIndex + tokenOffset, offset - tokenOffset);
								}
							}

							if (CanYield(nextTokenType))
							{
								yield return new SqlToken(nextTokenType, _sql, sqlIndex + offset, nextTokenLength);
							}

							offset += nextTokenLength;
							tokenOffset = offset;

							nextTokenType = 0;
							nextTokenLength = 0;
						}
						else
						{
							offset++;
						}
					}

					if (maxOffset > tokenOffset && CanYield(SqlTokenType.Text))
					{
						yield return new SqlToken(SqlTokenType.Text, _sql, sqlIndex + tokenOffset, maxOffset - tokenOffset);
					}
					sqlIndex += maxOffset;
				}
			}
		}
	}
}
