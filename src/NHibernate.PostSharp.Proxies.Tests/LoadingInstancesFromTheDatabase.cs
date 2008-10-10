using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.PostSharp.Proxies.Tests
{
    [TestFixture]
    public class LoadingInstancesFromTheDatabase : TestCase
    {
        private int customerId;
        protected override void OnSetUp()
        {
            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var obj = new Customer {Name = "ayende"};
                s.Save(obj);
                customerId = obj.Id;
                tx.Commit();
            }
        }


        protected override void OnTearDown()
        {
            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                s.Delete("from Customer");
                tx.Commit();
            }
        }

        [Test]
        public void CanLoadItemFromDatabase()
        {
            using (var s = sessions.OpenSession())
            using (var tx = s.BeginTransaction())
            {
                var customer = s.Load<Customer>(customerId );
                Assert.IsNotNull(customer);
                tx.Commit();
            }   
        }

        [Test]
        public void CanGetItemIdFromDatabase_WithoutGoingtoDb()
        {
            using (var s = sessions.OpenSession())
            //using (var tx = s.BeginTransaction()) // intentionally removed
            {
                s.Disconnect();// intentional

                var customer = s.Load<Customer>(customerId);
                Assert.AreEqual(customerId, customer.Id);
            }
        }

        [Test]
        public void CanCheckNotInitializedStatus()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.IsFalse(
                    NHibernateUtil.IsInitialized(customer)
                    );
            }
        }

        [Test]
        public void CanGetIdentifier()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.AreEqual(
                    customerId,
                    s.GetIdentifier(customer));
            }
        }

        [Test]
        public void CanSetIdentifier_AndGetNewValueFromSession()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                customer.Id = 2;
                Assert.AreEqual(
                    customerId,
                    s.GetIdentifier(customer));
            }
        }

        [Test]
        public void CanLazyLoadProperties()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.AreEqual("ayende", customer.Name);
            }
        }

        [Test]
        public void CanCallMethodsWithParameters()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                customer.AddAddress(new Address());
            }
        }

        [Test]
        public void CallingMethodsWillForceLoading()
        {
            using (var s = sessions.OpenSession())
            {
                var customer = s.Load<Customer>(customerId);
                Assert.IsFalse(NHibernateUtil.IsInitialized(customer));
                customer.AddAddress(new Address());
                Assert.IsTrue(NHibernateUtil.IsInitialized(customer));
            }
        }
    }
}