using System;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1714
{
    [TestFixture]
    public class SimpleReproductionFixture : BugTestCase
    {
        protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
        {
            return dialect as MsSql2005Dialect != null;
        }

        [Test]
        public void DbCommandsFromEventListenerShouldBeEnlistedInRunningTransaction()
        {
            using (ISession session = OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    var entity = new DomainClass();
                    session.Save(entity);

                    using (var otherSession = session.SessionWithOptions().Connection().OpenSession())
                    {
                        otherSession.Save(new DomainClass());
                        otherSession.Flush();
                    }

                    tx.Commit();
                }
            }

			using(var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from DomainClass");
				tx.Commit();
			}
        }
    }
}