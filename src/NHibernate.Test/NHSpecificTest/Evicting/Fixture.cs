using NHibernate.Cfg;
using NUnit.Framework;
using NHibernate.Stat;

namespace NHibernate.Test.NHSpecificTest.Evicting
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "Evicting"; }
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = sessions.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(new Employee
				{
                    Id = 1,
					FirstName = "a",
					LastName = "b"
				});
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = sessions.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete(session.Load<Employee>(1));

				tx.Commit();
			}
			base.OnTearDown();
		}


		[Test]
		public void Can_evict_entity_from_session()
		{
			using (var session = sessions.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var employee = session.Load<Employee>(1);
				Assert.IsTrue(session.Contains(employee));

				session.Evict(employee);

				Assert.IsFalse(session.Contains(employee));

				tx.Commit();
			}
		}

		[Test]
		public void Can_evict_non_persistent_object()
		{

			using (var session = sessions.OpenSession())
			using (var tx = session.BeginTransaction())
			{
				var employee = new Employee();
				Assert.IsFalse(session.Contains(employee));

				session.Evict(employee);

				Assert.IsFalse(session.Contains(employee));

				tx.Commit();
			}
		}

		[Test]
		public void Can_evict_when_trying_to_evict_entity_from_another_session()
		{

			using (var session1 = sessions.OpenSession())
			using (var tx1 = session1.BeginTransaction())
			{

				using (var session2 = sessions.OpenSession())
				using (var tx2 = session2.BeginTransaction())
				{
					var employee = session2.Load<Employee>(1);
					Assert.IsFalse(session1.Contains(employee));
					Assert.IsTrue(session2.Contains(employee));

					session1.Evict(employee);

					Assert.IsFalse(session1.Contains(employee));

					Assert.IsTrue(session2.Contains(employee));

					tx2.Commit();
				}
				
				tx1.Commit();
			}
		}
	
	}
}
