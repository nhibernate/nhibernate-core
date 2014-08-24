using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class QueryCacheableTests : LinqTestCase
    {
        protected override void Configure(Configuration cfg)
        {
            cfg.SetProperty(Environment.UseQueryCache, "true");
            cfg.SetProperty(Environment.GenerateStatistics, "true");
            base.Configure(cfg);
        }

        [Test]
        public void QueryIsCacheable()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            var x = (from c in db.Customers
                     select c).Cacheable().ToList();

            var x2 = (from c in db.Customers
                     select c).Cacheable().ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
        }

        [Test]
        public void QueryIsCacheable2()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            var x = (from c in db.Customers
                     select c).Cacheable().ToList();

            var x2 = (from c in db.Customers
                      select c).ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
        }

        [Test]
        public void QueryIsCacheable3()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            var x = (from c in db.Customers.Cacheable()
                     select c).ToList();

            var x2 = (from c in db.Customers
                      select c).ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
        }

        [Test]
        public void QueryIsCacheableWithRegion()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            var x = (from c in db.Customers
                     select c).Cacheable().CacheRegion("test").ToList();

            var x2 = (from c in db.Customers
                      select c).Cacheable().CacheRegion("test").ToList();

            var x3 = (from c in db.Customers
                      select c).Cacheable().CacheRegion("other").ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
        }

        [Test]
        public void CacheableBeforeOtherClauses()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            db.Customers.Cacheable().Where(c => c.ContactName != c.CompanyName).Take(1).ToList();
            db.Customers.Where(c => c.ContactName != c.CompanyName).Take(1).ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
        }

        [Test]
        public void CacheableRegionBeforeOtherClauses()
        {
            Sfi.Statistics.Clear();
            Sfi.QueryCache.Clear();

            db.Customers.Cacheable().CacheRegion("test").Where(c => c.ContactName != c.CompanyName).Take(1).ToList();
            db.Customers.Cacheable().CacheRegion("test").Where(c => c.ContactName != c.CompanyName).Take(1).ToList();
            db.Customers.Cacheable().CacheRegion("other").Where(c => c.ContactName != c.CompanyName).Take(1).ToList();

            Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
            Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
        }
    }
}
