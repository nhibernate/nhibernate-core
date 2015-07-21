using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1488
{
	// http://jira.nhibernate.org/browse/NH-1488
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void WorkBut()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new CustomerNoSmart("Somebody"));
				CustomerNoSmart c = new CustomerNoSmart("Somebody else");
				c.Category = new CustomerCategory("User");
				s.Save(c.Category);
				s.Save(c);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList result = s.CreateQuery("select c.Name, cat.Name from CustomerNoSmart c left outer join c.Category cat").List();
				Assert.That(result.Count, Is.EqualTo(2));
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from CustomerNoSmart");
				s.Delete("from Category");
				tx.Commit();
			}
		}

		[Test]
		public void Bug()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new Customer("Somebody"));
				Customer c = new Customer("Somebody else");
				c.Category = new CustomerCategory("User");
				s.Save(c.Category);
				s.Save(c);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				IList result = s.CreateQuery("select c.Name, cat.Name from Customer c left outer join c.Category cat").List();
				Assert.That(result.Count, Is.EqualTo(2), "should return Customers, on left outer join, even empty Category");
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Customer");
				s.Delete("from Category");
				tx.Commit();
			}
		}
	}
}