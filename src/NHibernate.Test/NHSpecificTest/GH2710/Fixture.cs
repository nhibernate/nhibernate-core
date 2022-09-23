using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2710
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e = new Entity {MbrId = 1, MrcDailyMoved = "N"};
				session.Save(e);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void Test()
		{
			var ids = Enumerable.Range(1, 10).ToList();
			Parallel.For(1, 50, i =>
			{
				UpdateEntity(ids);
			});
		}

		private void UpdateEntity(List<int> ids)
		{
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				session.EnableFilter("Filter").SetParameter("MbrId", 5);
				session.Query<Entity>()
				       .Where(o => ids.Contains(o.Id))
				       .Update(o => new Entity { MrcDailyMoved = "Y" });

				t.Commit();
			}
		}
	}
}
