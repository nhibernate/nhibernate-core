using NUnit.Framework;
using NHibernate.SqlCommand;
using NHibernate.SqlCommand.Parser;

namespace NHibernate.Test.SqlCommandTest
{
	[TestFixture]
	public class SqlTokenizerFixture
	{
		[Test]
		public void TokenizeSimpleSelectStatement()
		{
			VerifyTokenizer("SELECT * FROM some_table WHERE key = ? ORDER BY some_field DESC",
				Text("SELECT"), Whitespace(" "), Text("*"),
				Whitespace(" "),
				Text("FROM"), Whitespace(" "), Text("some_table"),
				Whitespace(" "),
				Text("WHERE"), Whitespace(" "), Text("key"), Whitespace(" "), Text("="), Whitespace(" "), Parameter(),
				Whitespace(" "),
				Text("ORDER"), Whitespace(" "), Text("BY"), Whitespace(" "), Text("some_field"), Whitespace(" "), Text("DESC"));
		}

		[Test]
		public void TokenizeLineComments()
		{
			VerifyTokenizer("--", Comment("--"));
			VerifyTokenizer("---", Comment("---"));
			VerifyTokenizer("--\r", Comment("--"), Whitespace("\r"));
			VerifyTokenizer("-- Any comment will do", 
				Comment("-- Any comment will do"));
			VerifyTokenizer("-- Two comments\n--will do too", 
				Comment("-- Two comments"), Whitespace("\n"), Comment("--will do too"));
		}

		[Test]
		public void TokenizeBlockComments()
		{
			VerifyTokenizer("/**/", Comment("/**/"));
			VerifyTokenizer("/***/", Comment("/***/"));
			VerifyTokenizer("/*/*/", Comment("/*/*/"));
			VerifyTokenizer("/****/", Comment("/****/"));
			VerifyTokenizer("//**//", Text("/"), Comment("/**/"), Text("/"));
			VerifyTokenizer("/*\n*/", Comment("/*\n*/"));
			VerifyTokenizer("/**/\n", Comment("/**/"), Whitespace("\n"));
			VerifyTokenizer("/**/*", Comment("/**/"), Text("*"));
			VerifyTokenizer("/**/*/", Comment("/**/"), Text("*/"));
			VerifyTokenizer("*//**/", Text("*/"), Comment("/**/"));
			VerifyTokenizer("SELECT/**/*", Text("SELECT"), Comment("/**/"), Text("*"));
		}

		[Test]
		public void TokenizeBrackets()
		{
			VerifyTokenizer("()", BracketOpen(), BracketClose());
			VerifyTokenizer("(())", BracketOpen(), BracketOpen(), BracketClose(), BracketClose());
			VerifyTokenizer("()()", BracketOpen(), BracketClose(), BracketOpen(), BracketClose());
			VerifyTokenizer("(\n)", BracketOpen(), Whitespace("\n"), BracketClose());
			VerifyTokenizer("()--()", BracketOpen(), BracketClose(), Comment("--()"));
			VerifyTokenizer("(--\n)", BracketOpen(), Comment("--"), Whitespace("\n"), BracketClose());
			VerifyTokenizer("(SELECT)", BracketOpen(), Text("SELECT"), BracketClose());
			
			VerifyTokenizer("SELECT (SELECT COUNT(*) FROM table), ?",
				Text("SELECT"), Whitespace(" "),
				BracketOpen(),
					Text("SELECT"), Whitespace(" "), Text("COUNT"),
					BracketOpen(),
						Text("*"),
					BracketClose(),
					Whitespace(" "), Text("FROM"), Whitespace(" "), Text("table"),
				BracketClose(),
				Comma(), Whitespace(" "), Parameter());
		}

		[Test]
		public void TokenizeQuotedString()
		{
			VerifyTokenizer("''", DelimitedText("''"));
			VerifyTokenizer("''''", DelimitedText("''''"));
			VerifyTokenizer("'string literal'", DelimitedText("'string literal'"));
			VerifyTokenizer("'x''s value'", DelimitedText("'x''s value'"));
		}

		[Test]
		public void TokenizeQuotedIdentifier()
		{
			VerifyTokenizer(@"""Identifier""", DelimitedText(@"""Identifier"""));
			VerifyTokenizer(@"""""""Identifier""""""", DelimitedText(@"""""""Identifier"""""""));
			VerifyTokenizer("[Identifier]", DelimitedText("[Identifier]"));
			VerifyTokenizer("[[Identifier]]]", DelimitedText("[[Identifier]]]"));
		}

		[Test]
		public void TokenizeParameters()
		{
			VerifyTokenizer("?", Parameter());
			VerifyTokenizer("'?'", DelimitedText("'?'"));
			VerifyTokenizer(@"""?""", DelimitedText(@"""?"""));
			VerifyTokenizer("[?]", DelimitedText("[?]"));
			VerifyTokenizer("--?", Comment("--?"));
			VerifyTokenizer("/*?*/", Comment("/*?*/"));
			VerifyTokenizer("(?)", BracketOpen(), Parameter(), BracketClose());
			VerifyTokenizer("EXEC InsertSomething ?, ?", 
				Text("EXEC"), Whitespace(" "), Text("InsertSomething"), 
				Whitespace(" "), Parameter(), Comma(), 
				Whitespace(" "), Parameter());
		}

		private static void VerifyTokenizer(string sql, params ExpectedToken[] expectedTokens)
		{
			var sqlString = SqlString.Parse(sql);

			int tokenIndex = 0;
			int sqlIndex = 0;
			foreach (var token in new SqlTokenizer(sqlString) { IgnoreComments = false, IgnoreWhitespace = false })
			{
				if (tokenIndex >= expectedTokens.Length)
				{
					Assert.Fail("Tokenizer returns more than expected '{0}' tokens. \nSQL: {1}\nLast Token: {2}({3})",
						expectedTokens.Length, sql, token.TokenType, token.Value);
				}

				var expectedToken = expectedTokens[tokenIndex];
				Assert.That(token.TokenType, Is.EqualTo(expectedToken.TokenType), "[Token #{0} in '{1}']TokenType", tokenIndex, sql);
				Assert.That(token.Value, Is.EqualTo(expectedToken.Value), "[Token #{0} in {1}]Value", tokenIndex, sql);
				Assert.That(token.SqlIndex, Is.EqualTo(sqlIndex), "[Token #{0} in {1}]SqlIndex", tokenIndex, sql);
				Assert.That(token.Length, Is.EqualTo(expectedToken.Length), "[Token #{0} in {1}]Length", tokenIndex, sql);

				tokenIndex++;
				sqlIndex += expectedToken.Length;
			}

			if (tokenIndex < expectedTokens.Length)
			{
				Assert.Fail("Tokenizer returns less than expected '{0}' tokens.\nSQL: {1}", expectedTokens.Length, sql);
			}
		}

		private static ExpectedToken Comma()
		{
			return new ExpectedToken(SqlTokenType.Comma, ",");
		}

		private static ExpectedToken Parameter()
		{
			return new ExpectedToken(SqlTokenType.Parameter, "?");
		}

		private static ExpectedToken BracketOpen()
		{
			return new ExpectedToken(SqlTokenType.BracketOpen, "(");
		}

		private static ExpectedToken BracketClose()
		{
			return new ExpectedToken(SqlTokenType.BracketClose, ")");
		}

		private static ExpectedToken DelimitedText(string text)
		{
			return new ExpectedToken(SqlTokenType.DelimitedText, text);
		}

		private static ExpectedToken Text(string text)
		{
			return new ExpectedToken(SqlTokenType.Text, text);
		}

		private static ExpectedToken Comment(string text)
		{
			return new ExpectedToken(SqlTokenType.Comment, text);
		}

		private static ExpectedToken Whitespace(string text)
		{
			return new ExpectedToken(SqlTokenType.Whitespace, text);
		}

		private class ExpectedToken
		{
			public SqlTokenType TokenType { get; private set; }
			public string Value  { get; private set; }

			public ExpectedToken(SqlTokenType tokenType, string value)
			{
				this.TokenType = tokenType;
				this.Value = value;
			}

			public int Length
			{
				get { return this.Value != null ? this.Value.Length : 0; }
			}
		}
	}
}
