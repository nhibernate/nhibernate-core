using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1444
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}
		[Test]
		public void Bug()
		{
			using (ISession s = OpenSession())
			{
				long? filter = null;
				using (var ls = new SqlLogSpy())
				{
					s.CreateQuery(@"SELECT c FROM xchild c WHERE (:filternull = true OR c.Parent.A < :filterval)")
						.SetParameter("filternull", !filter.HasValue)
						.SetParameter("filterval", filter.HasValue ? filter.Value : 0).List<xchild>();
					var message = ls.GetWholeLog();
					Assert.That(message, Text.Contains("xchild0_.ParentId=xparent1_.Id and (@p0=1 or xparent1_.A<@p1)"));
				}
			}
		}
	}
}