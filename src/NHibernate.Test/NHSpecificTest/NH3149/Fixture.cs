using System.Diagnostics;
using System.Threading;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH3149
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();

            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var entity = new NH3149Entity();
                    session.Save(entity);
                    tx.Commit();
                }
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();

            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete("from NH3149Entity");
                    tx.Commit();
                }
            }
        }

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return ((dialect as MsSql2005Dialect != null) || (dialect as MsSql2008Dialect != null));
        }

        [Test]
        public void ShouldNotWaitForLock()
        {
            int timeout = 0;

            using (var session1 = OpenSession())
            {
                var thread = new Thread(() =>
                {
                    session1.BeginTransaction();

                    var entity = session1.CreateCriteria<NH3149Entity>()
                        .SetLockMode(LockMode.UpgradeNoWait)
                        .AddOrder(Order.Desc("Id"))
                        .SetMaxResults(1)
                        .UniqueResult();
                });

                thread.Start();
                
                Thread.Sleep(1000);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                try
                {
                    using (var session = OpenSession())
                    {
                        using (var tx = session.BeginTransaction())
                        {
                            var entity = session.CreateCriteria<NH3149Entity>()
                                .SetTimeout(3)
                                .SetLockMode(LockMode.UpgradeNoWait)
                                .AddOrder(Order.Desc("Id"))
                                .SetMaxResults(1)
                                .UniqueResult();
                        }
                    }
                }
                catch
                {
                    // Ctach lack time out error
                }

                watch.Stop();

                thread.Join();

                Assert.Greater(3000, watch.ElapsedMilliseconds);   
            }
        }
    }
}
