using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1675
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ShouldWorkUsingDistinctAndLimits()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				for (int i = 0; i < 5; i++)
				{
					s.Save(new Person {FirstName = "Name" + i});
				}
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				var q =s.CreateQuery("select distinct p from Person p").SetFirstResult(0).SetMaxResults(10);
				Assert.That(q.List().Count, Is.EqualTo(5));
			}

			// clean up
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}
	}
}