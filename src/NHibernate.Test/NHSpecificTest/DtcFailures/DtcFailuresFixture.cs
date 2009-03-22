using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Transactions;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Exceptions;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.DtcFailures
{
    using System.Collections;

    [TestFixture]
    public class DtcFailuresFixture : TestCase
    {

        protected override IList Mappings
        {
            get { return new string[] { "NHSpecificTest.DtcFailures.Mappings.hbm.xml" }; }
        }

        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        [Test]
        public void WillNotCrashOnDtcPrepareFailure()
        {
            var tx = new TransactionScope();
            using (var s = sessions.OpenSession())
            {
                s.Save(new Person
                {
                    CreatedAt = DateTime.MinValue // will cause SQL date failure
                });
            }

            new ForceEscalationToDistributedTx();

            tx.Complete();
            try
            {
                tx.Dispose();
                Assert.Fail("Expected failure");
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void CanDeleteItemInDtc()
        {
            object id;
            using (var tx = new TransactionScope())
            using (var s = sessions.OpenSession())
            {
                id = s.Save(new Person
                {
                    CreatedAt = DateTime.Today
                });

                new ForceEscalationToDistributedTx();
                
                tx.Complete();
            }

            using (var tx = new TransactionScope())
            using (var s = sessions.OpenSession())
            {
                new ForceEscalationToDistributedTx(); 
                
                s.Delete(s.Get<Person>(id));

                tx.Complete();
            }

        }

        public class ForceEscalationToDistributedTx : IEnlistmentNotification
        {
            private readonly int thread;
            public ForceEscalationToDistributedTx()
            {
                thread = Thread.CurrentThread.ManagedThreadId;
                System.Transactions.Transaction.Current.EnlistDurable(Guid.NewGuid(), this, EnlistmentOptions.None);
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                Assert.AreNotEqual(thread, Thread.CurrentThread.ManagedThreadId);
                preparingEnlistment.Prepared();
            }

            public void Commit(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void Rollback(Enlistment enlistment)
            {
                enlistment.Done();
            }

            public void InDoubt(Enlistment enlistment)
            {
                enlistment.Done();
            }
        }
    }
}
