using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1754
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
		}

		protected override void OnTearDown()
		{
			Sfi.Statistics.IsStatisticsEnabled = false;
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void PersistIdentityDoNotImmediateExecuteQuery()
		{
			using (var session = OpenSession())
			{
				Sfi.Statistics.Clear();
				session.Persist(new Entity {Name = "Test"});

				Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(0));

				session.Flush();

				Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(1));
			}
		}

		[Test]
		public void PersistIdentityDoNotSaveIfSessionIsNotFlushed()
		{
			using (var session = OpenSession())
			{
				session.Persist(new Entity {Name = "Test"});
			}

			using (var session = OpenSession())
			{
				var count = session.Query<Entity>().Count();
				Assert.That(count, Is.EqualTo(0));
			}
		}
	}
}
