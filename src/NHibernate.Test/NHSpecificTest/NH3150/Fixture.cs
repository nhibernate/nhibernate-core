using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3150
{

	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH3150"; }
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Worker");
				s.Delete("from Worker2");
				s.Delete("from Role");
				tx.Commit();
			}
		}

		[Test]
		public void CanHaveNaturalIdWithMoreThanOneProperty()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var worker = new Worker();
				worker.name = "Mr Black";
				worker.position = "Managing Director";
				s.Save(worker);

				tx.Commit();
				Assert.AreEqual(1, worker.id, "id should be 1");
			}
		}

		[Test]
		public void IdBagWithSelectPOID()
		{
			var worker_id = 0;
			var role_id = 0;

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var worker = new Worker2();
				var role = new Role() { description = "monkey keeper" };
				
				worker.roles = new List<Role>();
				worker.roles.Add(role);

				s.Save(worker);
				s.Save(role);

				tx.Commit();

				worker_id = worker.id.Value;
				role_id = role.id.Value;
			}

			using (ISession s = OpenSession())
			{
				var saved_worker = s.Get<Worker2>(worker_id);
				Assert.NotNull(saved_worker.roles, "roles should not be null");
				Assert.AreEqual(1, saved_worker.roles.Count, "roles count should be 1");
			}

		}
	}
}
