using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
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

        private void AssertQueryReuseable()
        {
            IList<User> users = _query.ToList();
            Assert.AreEqual(3, users.Count);
        }

        [Test]
        public void CanReuseAfterFirst()
        {
            var user = _query.First(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterFirstOrDefault()
        {
            var user = _query.FirstOrDefault(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterSingle()
        {
            var user = _query.Single(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterSingleOrDefault()
        {
            User user = _query.SingleOrDefault(u => u.Name == "rahien");

            Assert.IsNotNull(user);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterAggregate()
        {
            User user = _query.Aggregate((u1, u2) => u1);

            Assert.IsNotNull(user);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterAverage()
        {
            double average = _query.Average(u => u.InvalidLoginAttempts);

            Assert.AreEqual(5.0, average);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterCount()
        {
            int totalCount = _query.Count();

            Assert.AreEqual(3, totalCount);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterCountWithPredicate()
        {
            int count = _query.Count(u => u.LastLoginDate != null);

            Assert.AreEqual(1, count);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterLongCount()
        {
            long totalCount = _query.LongCount();

            Assert.AreEqual(3, totalCount);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterLongCountWithPredicate()
        {
            long totalCount = _query.LongCount(u => u.LastLoginDate != null);

            Assert.AreEqual(1, totalCount);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterMax()
        {
            int max = _query.Max(u => u.InvalidLoginAttempts);

            Assert.AreEqual(6, max);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterMin()
        {
            int min = _query.Min(u => u.InvalidLoginAttempts);

            Assert.AreEqual(4, min);
            AssertQueryReuseable();
        }

        [Test]
        public void CanReuseAfterSum()
        {
            int sum = _query.Sum(u => u.InvalidLoginAttempts);

            Assert.AreEqual(4 + 5 + 6, sum);
            AssertQueryReuseable();
        }
    }
}