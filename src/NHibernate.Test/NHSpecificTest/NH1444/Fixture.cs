using NHibernate.Cfg;
using NHibernate.Driver;
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
				    string paramPrefix = ((DriverBase) Sfi.ConnectionProvider.Driver).NamedPrefix;
					Assert.That(message, Is.StringContaining("xchild0_.ParentId=xparent1_.Id and (" + paramPrefix + "p0=" + Dialect.ToBooleanValueString(true) + " or xparent1_.A<" + paramPrefix + "p1)"));
				}
			}
		}
	}
}