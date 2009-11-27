using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Test.Linq.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class QueryReuseTests : LinqTestCase
    {
        private IQueryable<User> _query;

        protected override void OnSetUp()
        {
            base.OnSetUp();

            _query = db.Users;
        }

        private void AssertQueryReuseable(IQueryable<User> query)
        {
            IList<User> users = _query.ToList();
            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public void CanReuseAfterFirst()
        {
            User user = _query.First(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterFirstOrDefault()
        {
            User user = _query.FirstOrDefault(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterSingle()
        {
            User user = _query.Single(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterSingleOrDefault()
        {
            User user = _query.SingleOrDefault(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterAggregate()
        {
            User user = _query.Aggregate((u1, u2) => u1);

            Assert.IsNotNull(user);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterAverage()
        {
            double average = _query.Average(u => u.InvalidLoginAttempts);

            Assert.AreEqual(5.0, average);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterCount()
        {
            int totalCount = _query.Count();

            Assert.AreEqual(3, totalCount);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterCountWithPredicate()
        {
            int count = _query.Count(u => u.LastLoginDate != null);

            Assert.AreEqual(1, count);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterLongCount()
        {
            long totalCount = _query.LongCount();

            Assert.AreEqual(3, totalCount);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterLongCountWithPredicate()
        {
            long totalCount = _query.LongCount(u => u.LastLoginDate != null);

            Assert.AreEqual(1, totalCount);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterMax()
        {
            int max = _query.Max(u => u.InvalidLoginAttempts);

            Assert.AreEqual(6, max);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterMin()
        {
            int min = _query.Min(u => u.InvalidLoginAttempts);

            Assert.AreEqual(4, min);
            AssertQueryReuseable(_query);
        }

        [Test]
        public void CanReuseAfterSum()
        {
            int sum = _query.Sum(u => u.InvalidLoginAttempts);

            Assert.AreEqual(4 + 5 + 6, sum);
            AssertQueryReuseable(_query);
        }
    }
}