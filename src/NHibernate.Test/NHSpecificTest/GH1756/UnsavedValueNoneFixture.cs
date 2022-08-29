using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1756
{
	[TestFixture]
	public class UnsavedValueNoneFixture : BugTestCase
	{
		// disable second level cache enabled by default by the base class.
		protected override string CacheConcurrencyStrategy => null;

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Id = Guid.NewGuid(), Name = "Bob" };
				session.Save(e1);

				transaction.Commit();
			}
			Sfi.Statistics.IsStatisticsEnabled = true;
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldUpdateByDefault()
		{
			Entity e;
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				e = session.Query<Entity>().First();
				tx.Commit();
			}

			e.Name = "Sally";
			Sfi.Statistics.Clear();

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.SaveOrUpdate(e);
				tx.Commit();
			}

			// Checks that no select has been done for verifying the entity state.
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
		}

		[Test]
		public void ShouldFailByTryingToUpdate()
		{
			var e = new Entity { Id = Guid.NewGuid(), Name = "Sally" };

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.SaveOrUpdate(e);
				Assert.That(tx.Commit, Throws.InstanceOf<StaleStateException>());
			}
		}
	}
}
