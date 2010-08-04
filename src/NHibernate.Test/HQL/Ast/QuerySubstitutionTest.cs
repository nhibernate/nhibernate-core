using System.Collections.Generic;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;
using SharpTestsEx;

namespace NHibernate.Test.HQL.Ast
{
	public class QuerySubstitutionTest: BaseFixture
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SessionFactory().Integrate.CreateCommands.WithHqlToSqlSubstitutions("pizza 1");
		}
		const string query = "from SimpleClass s where s.IntValue > pizza";

		[Test]
		public void WhenSubstitutionsConfiguredThenUseItInTranslation()
		{
			var sql = GetSql(query, new Dictionary<string, string>{{"pizza","1"}});
			sql.Should().Not.Contain("pizza");
		}

		[Test]
		public void WhenExecutedThroughSessionThenUseSubstitutions()
		{
			using (var s = OpenSession())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					s.CreateQuery(query).List();
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					sql.Should().Not.Contain("pizza");
				}
			}
		}
	}
}