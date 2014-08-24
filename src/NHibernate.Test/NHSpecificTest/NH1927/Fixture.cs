using System;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1927
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
        private static readonly DateTime MAX_DATE = new DateTime(3000, 1, 1);
        private static readonly DateTime VALID_DATE = new DateTime(2000, 1, 1);

        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var joe = new Customer() {ValidUntil = MAX_DATE};
                    session.Save(joe);

                    tx.Commit();
                }
            }
        }

        protected override void OnTearDown()
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete("from Invoice");
                    session.Delete("from Customer");
                    tx.Commit();
                }
            }
            base.OnTearDown();
        }

	    private delegate Customer QueryFactoryFunc(ISession session);

        private void TestQuery(QueryFactoryFunc queryFactoryFunc)
        {
            // test without filter
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                Assert.That(queryFactoryFunc(session), Is.Not.Null, "failed with filter off");
                tx.Commit();
            }

            // test with the validity filter
            using (ISession session = OpenSession())
            using (ITransaction tx = session.BeginTransaction())
            {
                session.EnableFilter("validity").SetParameter("date", VALID_DATE);
                Assert.That(queryFactoryFunc(session), Is.Not.Null, "failed with filter on");
                tx.Commit();
            }

        }

        [Test]
        public void CriteriaWithEagerFetch()
        {
            TestQuery(s => s.CreateCriteria(typeof (Customer))
				.SetFetchMode("Invoices", FetchMode.Eager)
				.UniqueResult<Customer>()
				);
        }

        [Test]
        public void CriteriaWithoutEagerFetch()
        {
            TestQuery(s => s
				.CreateCriteria(typeof(Customer))
				.UniqueResult<Customer>()
				);
        }

        [Test]
        public void HqlWithEagerFetch()
        {
            TestQuery(s => s.CreateQuery(@"
                    select c
                    from Customer c
                        left join fetch c.Invoices"
                    )
                    .UniqueResult<Customer>());
        }
        
        [Test]
        public void HqlWithoutEagerFetch()
        {
            TestQuery(s => s.CreateQuery(@"
                    select c
                    from Customer c"
                    )
                    .UniqueResult<Customer>());
        }
    }
}
