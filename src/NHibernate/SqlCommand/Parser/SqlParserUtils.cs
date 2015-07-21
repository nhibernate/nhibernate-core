using NHibernate.Exceptions;

namespace NHibernate.SqlCommand.Parser
{
	internal static class SqlParserUtils
	{
		public static int ReadDelimitedText(string text, int maxOffset, int offset)
		{
			var startOffset = offset;
			char quoteEndChar;

			// Determine end delimiter
			var quoteChar = text[offset++];
			switch (quoteChar)
			{
				case '\'':
				case '"':
					quoteEndChar = quoteChar;
					break;
				case '[':
					quoteEndChar = ']';
					break;
				default:
					throw new SqlParseException(string.Format("Quoted text cannot start with '{0}' character", text[offset]));
			}

			// Find end delimiter, but ignore escaped end delimiters
			while (offset < maxOffset)
			{
				if (text[offset++] == quoteEndChar)
				{
					if (offset >= maxOffset || text[offset] != quoteEndChar)
					{
						// Non-escaped delimiter char
						return offset - startOffset;
					}

					// Escaped delimiter char
					offset++;
				}
			}

			throw new SqlParseException(string.Format("Cannot find terminating '{0}' character for quoted text.", quoteEndChar));
		}

		public static int ReadLineComment(string text, int maxOffset, int offset)
		{
			var startOffset = offset;

			offset += 2;
			for (; offset < maxOffset; offset++)
			{
				switch (text[offset])
				{
					case '\r':
					case '\n':
						return offset - startOffset;
				}
			}

			return offset - startOffset;
		}

		public static int ReadMultilineComment(string text, int maxOffset, int offset)
		{
			var startOffset = offset;
			offset += 2;

			var prevChar = '\0';
			for (; offset < maxOffset; offset++)
			{
				var ch = text[offset];
				if (ch == '/' && prevChar == '*')
				{
					return offset + 1 - startOffset;
				}

				prevChar = ch;
			}

			throw new SqlParseException(string.Format("Cannot find terminating '*/' string for multiline comment."));
		}

		public static int ReadWhitespace(string text, int maxOffset, int offset)
		{
			var startOffset = offset;

			offset++;
			while (offset < maxOffset)
			{
				if (!char.IsWhiteSpace(text[offset])) break;
				offset++;
			}

			var result = offset - startOffset;
			return result;
		}
	}
}
