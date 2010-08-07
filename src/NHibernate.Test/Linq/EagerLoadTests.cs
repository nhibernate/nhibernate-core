using System.Linq;
using NHibernate.Linq;
using NHibernate.Test.Linq.Entities;
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
    }
}
