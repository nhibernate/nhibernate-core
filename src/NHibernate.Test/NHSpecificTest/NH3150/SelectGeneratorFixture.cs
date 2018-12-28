using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3150
{
	[TestFixture]
	public class SelectGeneratorFixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Test uses SQL-Server "instead of insert" triggers.
			return dialect is MsSql2008Dialect;
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				// Delete loads in memory and handle the cascade.
				s.Delete("from Worker2");
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void CanUseNaturalIdWithMoreThanOneProperty()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var worker = new Worker
				{
					Name = "Mr Black",
					Position = "Managing Director"
				};
				s.Save(worker);

				var worker2 = new Worker
				{
					Name = "Mr Black",
					Position = "Director"
				};
				s.Save(worker2);

				tx.Commit();
				Assert.That(worker.Id, Is.EqualTo(1), "Id of first worker should be 1");
				Assert.That(worker2.Id, Is.EqualTo(2), "Id of second worker should be 2");
			}
		}

		[Test]
		public void CanUseKeyWithMoreThanOneProperty()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var worker = new WorkerWithExplicitKey
				{
					Name = "Mr Black",
					Position = "Managing Director"
				};
				s.Save(worker);

				var worker2 = new WorkerWithExplicitKey
				{
					Name = "Mr Black",
					Position = "Director"
				};
				s.Save(worker2);

				tx.Commit();
				Assert.That(worker.Id, Is.EqualTo(1), "Id of first worker should be 1");
				Assert.That(worker2.Id, Is.EqualTo(2), "Id of second worker should be 2");
			}
		}

		// Non-regression test case.
		[Test]
		public void CanUseComponentAsNaturalId()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var worker = new WorkerWithComponent
				{
					Nid = new WorkerWithComponent.NidComponent
					{
						Name = "Mr Black",
						Position = "Managing Director"
					}
				};
				s.Save(worker);

				var worker2 = new WorkerWithComponent
				{
					Nid = new WorkerWithComponent.NidComponent
					{
						Name = "Mr Black",
						Position = "Director"
					}
				};
				s.Save(worker2);

				tx.Commit();
				Assert.That(worker.Id, Is.EqualTo(1), "Id of first worker should be 1");
				Assert.That(worker2.Id, Is.EqualTo(2), "Id of second worker should be 2");
			}
		}

		[Test]
		public void IdBagWithSelectPOID()
		{
			int workerId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var worker = new Worker2();
				var role = new Role { Description = "keeper" };

				worker.Roles = new List<Role>() { role };

				s.Save(worker);
				s.Save(role);

				tx.Commit();

				workerId = worker.Id;
			}

			using (var s = OpenSession())
			{
				var saved_worker = s.Get<Worker2>(workerId);
				Assert.That(saved_worker.Roles, Is.Not.Null, "roles should not be null");
				Assert.That(saved_worker.Roles.Count, Is.EqualTo(1), "roles count should be 1");
			}
		}
	}
}
