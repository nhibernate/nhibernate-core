﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Hql.Ast
{
	using System.Threading.Tasks;
	[TestFixture]
	public class QuerySubstitutionTestAsync: BaseFixture
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.ByCode().SessionFactory().Integrate.CreateCommands.WithHqlToSqlSubstitutions("pizza 1, calda 'bobrock'");
		}

		[Test]
		public async Task WhenExecutedThroughSessionThenUseSubstitutionsAsync()
		{
			const string query = "from SimpleClass s where s.IntValue > pizza";
			using (var s = OpenSession())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					await (s.CreateQuery(query).ListAsync());
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql, Does.Not.Contain("pizza"));
				}
			}
		}

		[Test]
		public async Task WhenExecutedThroughSessionThenUseSubstitutionsWithStringAsync()
		{
			const string query = "from SimpleClass s where s.Description > calda";
			using (var s = OpenSession())
			{
				using (SqlLogSpy sqlLogSpy = new SqlLogSpy())
				{
					await (s.CreateQuery(query).ListAsync());
					string sql = sqlLogSpy.Appender.GetEvents()[0].RenderedMessage;
					Assert.That(sql, Does.Not.Contain("pizza").And.Contains("'bobrock'"));
				}
			}
		}
	}
}
