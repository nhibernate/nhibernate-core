using System;
using System.Collections.Generic;
using System.Transactions;
using log4net;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Impl;
using NUnit.Framework;
using Data = System.Data;

namespace NHibernate.Test.NHSpecificTest.NH2941
{
    [TestFixture]
    public class SampleTest : BugTestCase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SampleTest));

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = this.OpenSession())
            {
                string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
        {
            return dialect as MsSql2005Dialect != null;
        }

        [Test]
        public void SaveNewParentWithChildren()
        {
            ((SessionFactoryImpl)sessions).EventListeners.SaveOrUpdateEventListeners =
                new ISaveOrUpdateEventListener[] { new NHSaveOrUpdateEventListener() };

            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required,
                                                                         new TimeSpan(0, 0, 30)))
            {
                using (ISession session = this.OpenSession())
                using (ITransaction txn = session.BeginTransaction(Data.IsolationLevel.ReadCommitted))
                {
                    NHSaveOrUpdateEventListener.ParentSaveEventCount = 0;
                    NHSaveOrUpdateEventListener.ChildSaveEventCount = 0;

                    #region setup data

                    Parent parent;
                    parent = new Parent();
                    parent.Id = 101;
                    parent.Name = "Mom";
                    parent.Children = new List<Child>();

                    Child child1 = new Child();
                    child1.Id = 101;
                    child1.Name = "Child1";

                    Child child2 = new Child();
                    child1.Id = 102;
                    child2.Name = "Child2";

                    parent.Children.Add(child1);
                    child1.Parent = parent;

                    parent.Children.Add(child2);
                    child2.Parent = parent;

                    #endregion setup data

                    session.SaveOrUpdate(parent);
                    txn.Commit();
                }
                transactionScope.Complete();
            }
            Assert.IsTrue(NHSaveOrUpdateEventListener.ParentSaveEventCount > 0, "Parent save event should be fired on save");
            Assert.IsTrue(NHSaveOrUpdateEventListener.ChildSaveEventCount > 0, "Child save event should be fired on save");

            ((SessionFactoryImpl)sessions).EventListeners.SaveOrUpdateEventListeners =
                new ISaveOrUpdateEventListener[0];
        }

    }
}
