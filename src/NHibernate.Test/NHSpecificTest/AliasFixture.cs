using System;
using NHibernate.Dialect;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest
{
	[TestFixture]
	public class AliasFixture
	{
		[Test]
		public void NoLeadingUnderscores()
		{
			var alias = new Alias("suffix");
			Dialect.Dialect dialect = new MsSql2000Dialect();

			Assert.That(alias.ToAliasString("__someIdentifier", dialect), Does.Not.StartWith("_"));

			Assert.That(alias.ToUnquotedAliasString("__someIdentifier", dialect), Does.Not.StartsWith("_"));
		}

		[Test]
		public void TestDialectQuotedAlias()
		{
			var alias = new Alias(15, "PK");
			var aliasString = alias.ToAliasString("[Hello]]World]", new MsSql2000Dialect());
			Assert.That(aliasString, Is.EqualTo("[Hello]]WorldPK]"));
		}
		
		[Test]
		public void TestDialectQuotedAliasWithEscapedStringAtTheEdge()
		{
			var alias = new Alias(15, "PK");
			var aliasString = alias.ToAliasString("[Table0123456]]7890]", new MsSql2000Dialect());
			Assert.That(aliasString, Is.EqualTo("[Table0123456]]PK]"));
		}
		
		[Test]
		public void TestQuotedAlias()
		{
			var alias = new Alias(15, "PK");
			var aliasString = alias.ToAliasString("[Hello]]World]");
			Assert.That(aliasString, Is.EqualTo("[Hello]]WorldPK]"));
		}
		
		[Test]
		public void TestQuotedAliasWithEscapedStringAtTheEdge()
		{
			var alias = new Alias(15, "PK");
			var aliasString = alias.ToAliasString("[Table0123456]]7890]");
			Assert.That(aliasString, Is.EqualTo("[Table0123456PK]"));
		}
	}
}