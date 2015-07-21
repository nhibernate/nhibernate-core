using System;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH548
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH548"; }
		}

		[Test]
		public void ParentPropertyOnCacheHit()
		{
			if (cfg.Properties[Environment.CacheProvider] == null)
			{
				Assert.Ignore("Only applicable when a cache provider is enabled");
			}

			// make a new MainObject
			MainObject main = new MainObject();
			main.Name = "Parent";
			main.Component.Note = "Component";

			// save it to the DB
			using (ISession session = OpenSession())
			{
				session.Save(main);
				session.Flush();
			}

			// check parent
			Assert.IsNotNull(main.Component.Parent, "component parent null (saved)");

			MainObject getMain;

			using (ISession session = OpenSession())
			{
				getMain = (MainObject) session.Get(main.GetType(), main.ID);
				session.Clear();
				Assert.IsNotNull(getMain.Component.Parent, "component parent null (cache miss)");
			}

			using (ISession session = OpenSession())
			{
				getMain = (MainObject) session.Get(main.GetType(), main.ID);
				Assert.IsNotNull(getMain.Component.Parent, "component parent null (cache hit)");

				session.Delete(getMain);
				session.Flush();
			}
		}
	}
}