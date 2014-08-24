using System;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1922
{
	[TestFixture]
	public class Fixture : BugTestCase
	{    protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var joe = new Customer() {ValidUntil = new DateTime(2000,1,1)};
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
                    session.Delete("from Customer");
                    tx.Commit();
                }
            }
            base.OnTearDown();
        }


        [Test]
        public void CanExecuteQueryOnStatelessSessionUsingDetachedCriteria()
        {
            using(var stateless = sessions.OpenStatelessSession())
            {
            	var dc = DetachedCriteria.For<Customer>()
            		.Add(Restrictions.Eq("ValidUntil", new DateTime(2000,1,1)));

            	var cust = dc.GetExecutableCriteria(stateless)
					.UniqueResult();

				Assert.IsNotNull(cust);
            }
        }

       
    }
}
