﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cfg;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1444
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync: BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}

		[Test]
		public async Task BugAsync()
		{
			using (ISession s = OpenSession())
			{
				long? filter = null;
				using (var ls = new SqlLogSpy())
				{
					await (s.CreateQuery(@"SELECT c FROM xchild c WHERE (:filternull = true OR c.Parent.A < :filterval)")
						.SetParameter("filternull", !filter.HasValue)
						.SetParameter("filterval", filter.HasValue ? filter.Value : 0).ListAsync<xchild>());
					var message = ls.GetWholeLog();
					var paramFormatter = (ISqlParameterFormatter)Sfi.ConnectionProvider.Driver;
					Assert.That(message, Does.Contain(
						"on xchild0_.ParentId=xparent1_.Id").And.Contain(
						$"where {paramFormatter.GetParameterName(0)}={Dialect.ToBooleanValueString(true)} or " +
						$"xparent1_.A<{paramFormatter.GetParameterName(1)};"));
				}
			}
		}
	}
}
