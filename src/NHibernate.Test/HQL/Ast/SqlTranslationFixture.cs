using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.HQL.Ast
{
	[TestFixture]
	public class SqlTranslationFixture : BaseFixture
	{
		[Test, Ignore("Bug not fixed yet")]
		public void ParseFloatConstant()
		{
			var query = "select 123.5, s from SimpleClass s";

			Assert.That(GetSql(query), Text.StartsWith("select 123.5"));
		}
	}
}