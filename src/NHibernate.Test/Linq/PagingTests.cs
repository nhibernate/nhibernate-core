using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class PagingTests : LinqTestCase
    {
        [Test]
        public void Customers1to5()
        {
            var q = (from c in db.Customers select c.CustomerId).Take(5);
            var query = q.ToList();

            Assert.AreEqual(5, query.Count);
        }

        [Test]
        public void Customers11to20()
        {
            var query = (from c in db.Customers
                         orderby c.CustomerId
                         select c.CustomerId).Skip(10).Take(10).ToList();
            Assert.AreEqual(query[0], "BSBEV");
            Assert.AreEqual(10, query.Count);
        }

        [Test]
        [Ignore("NHibernate does not currently support subqueries in from clause")]
        public void CustomersChainedTake()
        {
            var q = (from c in db.Customers
                     orderby c.CustomerId
                     select c.CustomerId).Take(5).Take(6);
            
            var query = q.ToList();

            Assert.AreEqual(5, query.Count);
            Assert.AreEqual("ALFKI", query[0]);
            Assert.AreEqual("BLAUS", query[4]);
        }

        [Test]
        [Ignore("NHibernate does not currently support subqueries in from clause")]
        public void CustomersChainedSkip()
        {
            var q = (from c in db.Customers select c.CustomerId).Skip(10).Skip(5);
            var query = q.ToList();
            Assert.AreEqual(query[0], "CONSH");
            Assert.AreEqual(76, query.Count);
        }



        [Test]
        [Ignore("NHibernate does not currently support subqueries in from clause")]
        public void CountAfterTakeShouldReportTheCorrectNumber()
        {
            var users = db.Customers.Skip(3).Take(10);
            Assert.AreEqual(10, users.Count());
        }
    }
}