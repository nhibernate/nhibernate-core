using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class OrderByTests : LinqTestCase
    {
        [Test]
        public void AscendingOrderByClause()
        {
            var query = from c in db.Customers
                        orderby c.CustomerId
                        select c.CustomerId;

            var ids = query.ToList();

            if (ids.Count > 1)
            {
                Assert.Greater(ids[1], ids[0]);
            }
        }

        [Test]
        public void DescendingOrderByClause()
        {
            var query = from c in db.Customers
                        orderby c.CustomerId descending
                        select c.CustomerId;

            var ids = query.ToList();

            if (ids.Count > 1)
            {
                Assert.Greater(ids[0], ids[1]);
            }
        }

        [Test]
        [Ignore("NHibernate does not currently support subqueries in select clause (no way to specify a projection from a detached criteria).")]
        public void AggregateAscendingOrderByClause()
        {
            var query = from c in db.Customers
                        orderby c.Orders.Count
                        select c;

            var customers = query.ToList();

            if (customers.Count > 1)
            {
                Assert.Less(customers[0].Orders.Count, customers[1].Orders.Count);
            }
        }

        [Test]
        [Ignore("NHibernate does not currently support subqueries in select clause (no way to specify a projection from a detached criteria).")]
        public void AggregateDescendingOrderByClause()
        {
            var query = from c in db.Customers
                        orderby c.Orders.Count descending
                        select c;

            var customers = query.ToList();

            if (customers.Count > 1)
            {
                Assert.Greater(customers[0].Orders.Count, customers[1].Orders.Count);
            }
        }

        [Test]
        public void ComplexAscendingOrderByClause()
        {
            var query = from c in db.Customers
                        where c.Address.Country == "Belgium"
                        orderby c.Address.Country, c.Address.City
                        select c.Address.City;

            var ids = query.ToList();

            if (ids.Count > 1)
            {
                Assert.Greater(ids[1], ids[0]);
            }
        }

        [Test]
        public void ComplexDescendingOrderByClause()
        {
            var query = from c in db.Customers
                        where c.Address.Country == "Belgium"
                        orderby c.Address.Country descending, c.Address.City descending
                        select c.Address.City;

            var ids = query.ToList();

            if (ids.Count > 1)
            {
                Assert.Greater(ids[0], ids[1]);
            }
        }

        [Test]
        public void ComplexAscendingDescendingOrderByClause()
        {
            var query = from c in db.Customers
                        where c.Address.Country == "Belgium"
                        orderby c.Address.Country ascending, c.Address.City descending
                        select c.Address.City;

            var ids = query.ToList();

            if (ids.Count > 1)
            {
                Assert.Greater(ids[0], ids[1]);
            }
        }
    }
}