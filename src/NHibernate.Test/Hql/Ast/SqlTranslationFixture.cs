using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	[TestFixture]
	public class SqlTranslationFixture : BaseFixture
	{
		[Test]
		public void ParseFloatConstant()
		{
			const string query = "select 123.5, s from SimpleClass s";

			Assert.That(GetSql(query), Is.StringStarting("select 123.5"));
		}

		[Test]
		public void CaseClauseWithMath()
		{
			const string query = "from SimpleClass s where (case when s.IntValue > 0 then (cast(s.IntValue as long) * :pAValue) else 1 end) > 0";
			Assert.DoesNotThrow(() => GetSql(query));

			const string queryWithoutParen = "from SimpleClass s where (case when s.IntValue > 0 then cast(s.IntValue as long) * :pAValue else 1 end) > 0";
			Assert.DoesNotThrow(() => GetSql(queryWithoutParen));
		}

		[Test]
		public void Union()
		{
			const string queryForAntlr = "from SimpleClass s where s.id in (select s1.id from SimpleClass s1 union select s2.id from SimpleClass s2)";
			Assert.DoesNotThrow(() => GetSql(queryForAntlr));
		}
	}
}