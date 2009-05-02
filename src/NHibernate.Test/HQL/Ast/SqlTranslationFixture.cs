using NUnit.Framework;

namespace NHibernate.Test.HQL.Ast
{
	[TestFixture]
	public class SqlTranslationFixture : BaseFixture
	{
		[Test]
		public void ParseFloatConstant()
		{
			const string query = "select 123.5, s from SimpleClass s";

			Assert.That(GetSql(query), Text.StartsWith("select 123.5"));
		}
	}
}