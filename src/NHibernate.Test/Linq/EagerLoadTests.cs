using System.Linq;
using NHibernate.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class EagerLoadTests : LinqTestCase
    {
        [Test]
        public void RelationshipsAreLazyLoadedByDefault()
        {
            var x = db.Customers.ToList();

            session.Close();

            Assert.AreEqual(91, x.Count);
            Assert.IsFalse(NHibernateUtil.IsInitialized(x[0].Orders));
        }

        [Test]
        public void RelationshipsCanBeEagerLoaded()
        {
            var x = db.Customers.Fetch(c => c.Orders).ToList();

            session.Close();

            Assert.AreEqual(91, x.Count);
            Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
            Assert.IsFalse(NHibernateUtil.IsInitialized(x[0].Orders.First().OrderLines));
        }

        [Test]
        public void MultipleRelationshipsCanBeEagerLoaded()
        {
            var x = db.Employees.Fetch(e => e.Subordinates).Fetch(e => e.Orders).ToList();

            session.Close();

            Assert.AreEqual(9, x.Count);
            Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
            Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Subordinates));
        }

        [Test]
        public void NestedRelationshipsCanBeEagerLoaded()
        {
            var x = db.Customers.FetchMany(c => c.Orders).ThenFetchMany(o => o.OrderLines).ToList();

            session.Close();

            Assert.AreEqual(91, x.Count);
            Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders));
            Assert.IsTrue(NHibernateUtil.IsInitialized(x[0].Orders.First().OrderLines));
        }

        [Test]
        public void WhenFetchSuperclassCollectionThenNotThrows()
        {
            // NH-2277
            session.Executing(s => s.Query<Lizard>().Fetch(x => x.Children).ToList()).NotThrows();
            session.Close();
        }

        [Test]
        public void FetchWithWhere()
        {
					// NH-2381 NH-2362
            (from p
                in session.Query<Product>().Fetch(a => a.Supplier)
             where p.ProductId == 1
             select p).ToList();
        }

        [Test]
        public void FetchManyWithWhere()
        {
					// NH-2381 NH-2362
            (from s
                in session.Query<Supplier>().FetchMany(a => a.Products)
             where s.SupplierId == 1
             select s).ToList();
        }

        [Test]
        public void FetchAndThenFetchWithWhere()
        {
            // NH-2362
            (from p
                in session.Query<User>().Fetch(a => a.Role).ThenFetch(a => a.Entity)
             where p.Id == 1
             select p).ToList();
        }

        [Test]
        public void FetchAndThenFetchManyWithWhere()
        {
            // NH-2362
            (from p
                in session.Query<Employee>().Fetch(a => a.Superior).ThenFetchMany(a => a.Orders)
             where p.EmployeeId == 1
             select p).ToList();
        }

        [Test]
        public void FetchManyAndThenFetchWithWhere()
        {
            // NH-2362
            (from s
                in session.Query<Supplier>().FetchMany(a => a.Products).ThenFetch(a => a.Category)
             where s.SupplierId == 1
             select s).ToList();
        }

        [Test]
        public void FetchManyAndThenFetchManyWithWhere()
        {
            // NH-2362
            (from s
                in session.Query<Supplier>().FetchMany(a => a.Products).ThenFetchMany(a => a.OrderLines)
             where s.SupplierId == 1
             select s).ToList();
        }
    }
}
