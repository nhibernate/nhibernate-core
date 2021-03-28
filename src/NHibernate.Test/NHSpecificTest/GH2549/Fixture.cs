using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2549
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person {Id = 1, Name = "Name"});
				s.Save(new Customer {Deleted = false, Name = "Name", Id = 1});
				s.Save(new Customer {Deleted = true, Name = "Name", Id = 2});

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from System.Object").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void EntityJoinFilterLinq()
		{
			using (var s = OpenSession())
			{
				var list = (from p in s.Query<Person>()
							join c in s.Query<Customer>() on p.Name equals c.Name
							select p).ToList();

				s.EnableFilter("DeletedCustomer").SetParameter("deleted", false);

				var filteredList = (from p in s.Query<Person>()
									join c in s.Query<Customer>() on p.Name equals c.Name
									select p).ToList();

				Assert.That(list, Has.Count.EqualTo(2));
				Assert.That(filteredList, Has.Count.EqualTo(1));
			}
		}

		[Test]
		public void EntityJoinFilterQueryOver()
		{
			using (var s = OpenSession())
			{
				Customer c = null;
				Person p = null;
				var list = s.QueryOver(() => p).JoinEntityAlias(() => c, () => c.Name == p.Name).List();

				s.EnableFilter("DeletedCustomer").SetParameter("deleted", false);

				var filteredList = s.QueryOver(() => p).JoinEntityAlias(() => c, () => c.Name == p.Name).List();

				Assert.That(list, Has.Count.EqualTo(2));
				Assert.That(filteredList, Has.Count.EqualTo(1));
			}
		}
	}
}
