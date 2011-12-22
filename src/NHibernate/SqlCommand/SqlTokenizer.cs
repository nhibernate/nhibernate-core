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
        private readonly SqlString _sql;

        public SqlTokenizer(SqlString sql)
        {
            if (sql == null) throw new ArgumentNullException("sql");
            _sql = sql;
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
                    yield return new SqlToken(SqlTokenType.Parameter, _sql, sqlIndex++, 1);
                    continue;
                }

                var text = part as string;
                if (text != null)
                {
                    int offset = 0;
                    int maxOffset = text.Length;
                    int tokenOffset = 0;

                    while (offset < maxOffset)
                    {
                        var ch = text[offset];

                        SqlToken nextToken = null;
                        int nextTokenOffset = -1;

                        switch (ch)
                        {
                            case '(':
                                nextTokenOffset = offset;
                                nextToken = new SqlToken(SqlTokenType.BlockBegin, _sql, sqlIndex + offset, 1);
                                break;
                            case ')':
                                nextTokenOffset = offset;
                                nextToken = new SqlToken(SqlTokenType.BlockEnd, _sql, sqlIndex + offset, 1);
                                break;
                            case '\'':      // String literals
                            case '\"':      // ANSI quoted identifiers
                            case '[':       // Sql Server quoted indentifiers
                                nextTokenOffset = offset;
                                nextToken = new SqlToken(SqlTokenType.QuotedText, _sql, 
                                    sqlIndex + nextTokenOffset, ReadQuotedText(text, maxOffset, ref offset));
                                break;
                            case ',':
                                nextTokenOffset = offset;
                                nextToken = new SqlToken(SqlTokenType.ListSeparator, _sql, sqlIndex + offset, 1);
                                break;
                            case '*':
                                if (offset > 0 && text[offset - 1] == '/')
                                {
                                    nextTokenOffset = offset - 1;
                                    nextToken = new SqlToken(SqlTokenType.Comment, _sql, 
                                        sqlIndex + nextTokenOffset, ReadMultilineComment(text, maxOffset, ref offset));
                                }
                                break;
                            case '-':
                                if (offset > 0 && text[offset - 1] == '-')
                                {
                                    nextTokenOffset = offset - 1;
                                    nextToken = new SqlToken(SqlTokenType.Comment, _sql, 
                                        sqlIndex + nextTokenOffset, ReadLineComment(text, maxOffset, ref offset));
                                }
                                break;
                            default:
                                if (char.IsWhiteSpace(ch))
                                {
                                    nextTokenOffset = offset;
                                    nextToken = new SqlToken(SqlTokenType.Whitespace, _sql, 
                                        sqlIndex + nextTokenOffset, ReadWhitespace(text, maxOffset, ref offset));
                                }
                                break;
                        }

                        if (nextTokenOffset > tokenOffset)
                        {
                            yield return new SqlToken(SqlTokenType.Text, _sql, 
                                sqlIndex + tokenOffset, nextTokenOffset - tokenOffset);
                            tokenOffset = nextTokenOffset;
                        }
                        if (nextToken != null)
                        {
                            yield return nextToken;
                            tokenOffset += nextToken.Length;
                        }

                        offset++;
                    }

                    if (offset > tokenOffset) yield return new SqlToken(SqlTokenType.Text, _sql, 
                        sqlIndex + tokenOffset, maxOffset - tokenOffset);
                    sqlIndex += maxOffset;
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
