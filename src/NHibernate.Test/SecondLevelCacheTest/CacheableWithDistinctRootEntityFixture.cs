using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using NHibernate.Stat;
using NHibernate.Transform;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.SecondLevelCacheTests
{
    [TestFixture]
    public class CacheableWithDistinctRootEntityFixture : ReadonlyTestCase
    {
        protected override void Configure(Configuration configuration)
        {
            configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "true");
        }
        
        [Test]
        public void TestWorkingQueryOverNoCachable()
        {
            int resultCount = 0;
            OrderLine orderLines = null;
            IList<Order> orders;

            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                orders = s.QueryOver<Order>()
                    .Left.JoinAlias(t => t.OrderLines, () => orderLines)
                    .Fetch(x => x.OrderLines).Eager
                    .OrderBy(x => x.OrderDate).Desc
                    .TransformUsing(Transformers.DistinctRootEntity)
                    // .Cacheable()
                    .List();

                tx.Commit();
            }

            if ((orders != null) && (orders.Count>0))
            {
                resultCount = orders.Count;
            }

            Assert.Greater(resultCount, 0, "Cannot fetch orders.");
        }

        [Test]
        public void TestWorkingQueryOverTransformDistinctRootEntity()
        {
            int resultCount = 0;
            OrderLine orderLines = null;
            IList<Order> orders;

            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                orders = s.QueryOver<Order>()
                    .Left.JoinAlias(t => t.OrderLines, () => orderLines)
                    .Fetch(x => x.OrderLines).Eager
                    .OrderBy(x => x.OrderDate).Desc
                    // .TransformUsing(Transformers.DistinctRootEntity)
                    .Cacheable()
                    .List();

                tx.Commit();
            }

            if ((orders != null) && (orders.Count > 0))
            {
                resultCount = orders.Count;
            }

            Assert.Greater(resultCount, 0, "Cannot fetch orders.");
        }

        [Test]
        public void TestNonWorkingQueryOverCachableAndTransformDistinctRootEntity()
        {
            int resultCount = 0;
            OrderLine orderLines = null;
            IList<Order> orders;

            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                orders = s.QueryOver<Order>()
                    .Left.JoinAlias(t => t.OrderLines, () => orderLines)
                    .Fetch(x => x.OrderLines).Eager
                    .OrderBy(x => x.OrderDate).Desc
                    .TransformUsing(Transformers.DistinctRootEntity)
                    .Cacheable()
                    .List();

                tx.Commit();
            }

            if ((orders != null) && (orders.Count > 0))
            {
                resultCount = orders.Count;
            }

            Assert.Greater(resultCount, 0, "Cannot fetch orders.");
        }
    }
}
