using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2951
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
        [Ignore("Not working.")]
		public void UpdateWithSubqueryToJoinedSubclass()
		{
            using (ISession session = OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {

                var c = new Customer { Name = "Bob" };
                session.Save(c);

                var i = new Invoice { Amount = 10 };
                session.Save(i);

                session.Flush();
                transaction.Commit();
            }

            using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
                // Using (select c.Id ...) works.
                string hql = "update Invoice i set i.Customer = (select c from Customer c where c.Name = 'Bob')";

			    int result = session.CreateQuery(hql).ExecuteUpdate();

                Assert.AreEqual(1, result);
			}
		}
	}
}