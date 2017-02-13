using System.Collections.Generic;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1182
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
		}
		[Test]
		public void DeleteWithoutUpdateVersion()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new ObjectA { Bs = new List<ObjectB> { new ObjectB(), new ObjectB() } });
				t.Commit();
			}

			using (var ls = new SqlLogSpy())
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var a = s.CreateCriteria<ObjectA>().UniqueResult<ObjectA>();
					s.Delete(a);
					t.Commit();
				}
				string wholeLog = ls.GetWholeLog();
				Assert.That(wholeLog, Does.Not.Contain("UPDATE ObjectA"));
				Assert.That(wholeLog, Does.Contain("UPDATE ObjectB"),"should create orphans");
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from ObjectB").ExecuteUpdate();
				s.CreateQuery("delete from ObjectA").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}
