using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2344
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Person");
				s.Flush();
			}
			base.OnTearDown();
		}

		[Test]
		public void CoalesceShouldWork()
		{
			int personId;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var p1 = new Person { Name = "inserted name" };
				var p2 = new Person { Name = null};
				
				s.Save(p1);
				s.Save(p2);
				personId = p2.Id;
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (s.BeginTransaction())
			{
				var person = s.Query<Person>().Where(p => (p.Name ?? "e") == "e").First();
				Assert.AreEqual(personId, person.Id);
			}
		}
	}
}