using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Event;
using System.Diagnostics;

namespace NHibernate.Test.NHSpecificTest.NH2322
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "false");
			configuration.SetListener(ListenerType.PostUpdate, new PostUpdateEventListener());
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Person");
				s.Flush();
			}
			base.OnTearDown();
		}

		[Test]
		public void ShouldNotThrowWhenCommitingATransaction()
		{
			int id;
			
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var p = new Person { Name = "inserted name" };
				s.Save(p);
				id = p.Id;
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var p = s.Get<Person>(id);
				p.Name = "changing the name...";

				Assert.That(delegate() { t.Commit(); }, Throws.Nothing);
			}
		}
	}
}