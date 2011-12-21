using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.SqlCommand
{
    using NHibernate.Exceptions;

    /// <summary>
    /// Splits a <see cref="SqlString"/> into <see cref="SqlToken"/>s.
    /// </summary>
    public class SqlTokenizer : IEnumerable<SqlToken>
    {
        private readonly IEnumerable<SqlPart> _parts;

        public SqlTokenizer(SqlString sql)
            : this(sql, 0)
        {}

        public SqlTokenizer(SqlString sql, int startIndex)
        {
            if (sql == null) throw new ArgumentNullException("sql");
            _parts = GetParts(sql, startIndex);
        }

        private static IEnumerable<SqlPart> GetParts(SqlString sql, int startIndex)
        {
            var index = 0;
            var parts = sql.Compact().Parts;
            
            foreach (var part in parts)
            {
                var partLength = SqlString.LengthOfPart(part);

                if (startIndex <= index)
                {
                    yield return new SqlPart(part, index);
                }
                else if (startIndex < index + partLength)
                {
                    yield return new SqlPart(part, index).Subpart(startIndex - index);
                }
                index += partLength;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<SqlToken> GetEnumerator()
        {
            foreach (var part in _parts)
            {
                var parameter = part.Content as Parameter;
                if (parameter != null)
                {
                    yield return SqlToken.Parameter(part);
                    continue;
                }

                var text = part.Content as string;
                if (text != null)
                {
                    int offset = 0;
                    int maxOffset = text.Length;
                    int tokenStartIndex = 0;

                    while (offset < maxOffset)
                    {
                        var ch = text[offset];

                        SqlToken nextToken = null;
                        int nextTokenStartIndex = -1;

                        switch (ch)
                        {
                            case '(':
                                nextTokenStartIndex = offset;
                                nextToken = SqlToken.BlockBegin(part, offset);
                                break;
                            case ')':
                                nextTokenStartIndex = offset;
                                nextToken = SqlToken.BlockEnd(part, offset);
                                break;
                            case '\'':      // String literals
                            case '\"':      // ANSI quoted identifiers
                            case '[':       // Sql Server quoted indentifiers
                                nextTokenStartIndex = offset;
                                nextToken = SqlToken.QuotedText(part, nextTokenStartIndex, ReadQuotedText(text, maxOffset, ref offset));
                                break;
                            case ',':
                                nextTokenStartIndex = offset;
                                nextToken = SqlToken.ListSeparator(part, offset);
                                break;
                            case '*':
                                if (offset > 0 && text[offset - 1] == '/')
                                {
                                    nextTokenStartIndex = offset - 1;
                                    nextToken = SqlToken.Comment(part, nextTokenStartIndex, ReadMultilineComment(text, maxOffset, ref offset));
                                }
                                break;
                            case '-':
                                if (offset > 0 && text[offset - 1] == '-')
                                {
                                    nextTokenStartIndex = offset - 1;
                                    nextToken = SqlToken.Comment(part, nextTokenStartIndex, ReadLineComment(text, maxOffset, ref offset));
                                }
                                break;
                            default:
                                if (char.IsWhiteSpace(ch))
                                {
                                    nextTokenStartIndex = offset;
                                    nextToken = SqlToken.Whitespace(part, nextTokenStartIndex, ReadWhitespace(text, maxOffset, ref offset));
                                }
                                break;
                        }

                        if (nextTokenStartIndex > tokenStartIndex)
                        {
                            var token = SqlToken.Text(part, tokenStartIndex, nextTokenStartIndex - tokenStartIndex);
                            yield return token;
                            tokenStartIndex = nextTokenStartIndex;
                        }
                        if (nextToken != null)
                        {
                            yield return nextToken;
                            tokenStartIndex += nextToken.SqlLength;
                        }

                        offset++;
                    }

                    if (offset > tokenStartIndex) yield return SqlToken.Text(part, tokenStartIndex, offset - tokenStartIndex);
                }
            }
        }

        private static int ReadQuotedText(string text, int maxOffset, ref int offset)
        {
            var startOffset = offset;
            char quoteEndChar;

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

            // TODO: handle quote escaping
            for (; offset < maxOffset; offset++)
            {
                if (text[offset] == quoteEndChar)
                {
                    return offset + 1 - startOffset;
                }
            }

            throw new SqlParseException(string.Format("Cannot find terminating '{0}' character for quoted text.", quoteEndChar));
        }

        private static int ReadLineComment(string text, int maxOffset, ref int offset)
        {
            var startOffset = offset - 1;

            offset++;
            for (; offset < maxOffset; offset++)
            {
                switch (text[offset])
                {
                    case '\r':
                    case '\n':
                        return offset + 1 - startOffset;
                }
            }

            return offset - startOffset;
        }

        private static int ReadMultilineComment(string text, int maxOffset, ref int offset)
        {
            var startOffset = offset - 1;

            var prevChar = text[offset++];
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

        private static int ReadWhitespace(string text, int maxOffset, ref int offset)
        {
            var startOffset = offset;

            offset++;
            while (offset < maxOffset)
            {
                if (!char.IsWhiteSpace(text[offset])) break;
                offset++;
            }

            var result = offset - startOffset;
            offset--;
            return result;
        }
    }
}
