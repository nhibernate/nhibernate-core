using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1704
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Country = "Greece", City = "Athens", Budget = 100000m};
				session.Save(e1);

				var e2 = new Entity {Country = "Greece", City = "Chania", Budget = 50000m};
				session.Save(e2);

				var e3 = new Entity {Country = "Italy", City = "Rome", Budget = 200000m};
				session.Save(e3);

				var e4 = new Entity {Country = "Italy", City = "Milan", Budget = 100000m};
				session.Save(e4);

				var e5 = new Entity {Country = "France", City = "Paris", Budget = 300000m};
				session.Save(e5);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test(Description = "GH-1704")]
		public void GroupByKeySelectToCustomClass()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session.Query<Entity>()
				.GroupBy(a => new GroupByEntity(a.Country))
				.Select(a => new { groupby = a.Key, cnt = a.Count(), sum = a.Sum(o => o.Budget) })
				.ToList();

				Assert.That(result, Has.Count.EqualTo(3));
			}
		}
	}
}
