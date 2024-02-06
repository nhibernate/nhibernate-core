using Antlr.Runtime;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Parser
{
	[TestFixture]
	public class HqlParserFixture
	{
		[Test]
		public void HandlesPathWithReservedWords()
		{
			Assert.DoesNotThrow(() => Parse("delete from System.Object"));
			Assert.DoesNotThrow(() => Parse("delete from Object.Object.Object.Object"));
		}

		private static void Parse(string hql)
		{
			var lex = new HqlLexer(new CaseInsensitiveStringStream(hql));
			var tokens = new CommonTokenStream(lex);

			var parser = new HqlParser(tokens)
			{
				TreeAdaptor = new ASTTreeAdaptor(),
				ParseErrorHandler = new WarningAsErrorReporter()
			}.statement();
		}
	}
}
