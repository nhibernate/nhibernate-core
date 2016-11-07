using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;

namespace NHibernate.Test.Hql.Ast
{
	public class QuerySubstitutionTest: BaseFixture
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SessionFactory().Integrate.CreateCommands.WithHqlToSqlSubstitutions("pizza 1, calda 'bobrock'");
		}

		[Test]
		public void WhenSubstitutionsConfiguredThenUseItInTranslation()
		{
			const string query = "from SimpleClass s where s.IntValue > pizza";
			var sql = GetSql(query, new Dictionary<string, string>{{"pizza","1"}});
			Assert.That(sql, Is.Not.StringContaining("pizza"));
		}

		[Test]
		public void WhenExecutedThroughSessionThenUseSubstitutions()
		{
			const string query = "from SimpleClass s where s.IntValue > pizza";
			using (var s = OpenSession())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.CreateQuery(query).List();
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql, Is.Not.StringContaining("pizza"));
				}
			}
		}

		[Test]
		public void WhenSubstitutionsWithStringConfiguredThenUseItInTranslation()
		{
			const string query = "from SimpleClass s where s.Description > calda";
			var sql = GetSql(query, new Dictionary<string, string> { { "calda", "'bobrock'" } });
			Assert.That(sql, Is.Not.StringContaining("pizza").And.Contains("'bobrock'"));
		}

		[Test]
		public void WhenExecutedThroughSessionThenUseSubstitutionsWithString()
		{
			const string query = "from SimpleClass s where s.Description > calda";
			using (var s = OpenSession())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.CreateQuery(query).List();
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql, Is.Not.StringContaining("pizza").And.Contains("'bobrock'"));
				}
			}
		}
	}
}