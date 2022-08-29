using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2286
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void FilterOnJoinedSubclassKey()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var c1 = new IndividualCustomer { Id = 2, Name = "2" };
				var c2 = new IndividualCustomer { Id = 4, Name = "4" };
				session.Save(c1);
				session.Save(c2);

				session.Flush();
				transaction.Commit();
			}

			using (var s = OpenSession())
			{
				var count = s.Query<IndividualCustomer>().Select(c => c.Id).ToList().Count;
				s.EnableFilter("filterName");
				var countFiltered = s.Query<IndividualCustomer>().Select(c => c.Id).ToList().Count;

				Assert.AreEqual(2, count);
				Assert.AreEqual(1, countFiltered);
			}
		}
	}
}
