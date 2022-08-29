using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2626
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHibernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void SubqueryWithSelectOnSubclassProperty()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var capabilitiesQuery = session
										.Query<UserCapabilityAssignment>()
										.Where(x => x.Name == "aaa")
										.Select(x => x.UserId);

				session.Query<ApplicationUser>()
						.Where(x => capabilitiesQuery.Contains(x.Id))
						.ToList();
				Assert.That(logSpy.GetWholeLog(), Does.Contain("UserId").IgnoreCase);
			}
		}

		[Test]
		public void SubqueryWithOfTypeAndSelectOnSubclassProperty()
		{
			using (var logSpy = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var capabilitiesQuery = session
										.Query<CapabilityAssignment>().OfType<UserCapabilityAssignment>()
										.Where(x => x.Name == "aaa")
										.Select(x => x.UserId);

				session.Query<ApplicationUser>()
						.Where(x => capabilitiesQuery.Contains(x.Id))
						.ToList();
				Assert.That(logSpy.GetWholeLog(), Does.Contain("UserId").IgnoreCase);
			}
		}
	}
}
