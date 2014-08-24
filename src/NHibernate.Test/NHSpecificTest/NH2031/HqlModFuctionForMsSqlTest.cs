using NHibernate.Dialect;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2031
{
	public class MyClass
	{
		
	}
	public class HqlModFuctionForMsSqlTest : BugTestCase
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void TheModuleOperationShouldAddParenthesisToAvoidWrongSentence()
		{
			// The expected value should be "(5+1)%(1+1)" instead "5+ 1%1 +1"
			var sqlQuery = GetSql("select mod(5+1,1+1) from MyClass");
			sqlQuery.Should().Contain("(5+1)").And.Contain("(1+1)");
		}

		public string GetSql(string query)
		{
			var qt = new QueryTranslatorImpl(null, new HqlParseEngine(query, false, sessions).Parse(), new CollectionHelper.EmptyMapClass<string, IFilter>(), sessions);
			qt.Compile(null, false);
			return qt.SQLString;
		}
	}
}