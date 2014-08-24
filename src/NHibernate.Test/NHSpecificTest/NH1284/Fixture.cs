using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1284
{
	[TestFixture, Ignore("Not supported yet.")]
	public class Fixture : BugTestCase
	{
		[Test]
		public void EmptyValueTypeComponent()
		{
			Person jimmy;
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("Jimmy Hendrix");
				s.Save(p);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				jimmy = (Person)s.Get(typeof(Person), "Jimmy Hendrix");
				tx.Commit();
			}
			Assert.IsFalse(jimmy.Address.HasValue);

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}
	}
}