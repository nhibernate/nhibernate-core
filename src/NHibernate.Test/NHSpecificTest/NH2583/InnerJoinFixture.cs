using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2583
{
    [TestFixture]
    public class InnerJoinFixture : BugTestCase
    {
        [Test]
        public void OrShouldBeOuterJoin()
        {
            using (var sqlLog = new SqlLogSpy())
            using (var session = OpenSession())
            {
                session.Query<MyBO>().Where(b => b.BO1.I1 == 1 || b.BO2.J1 == 1).ToList();
                var log = sqlLog.GetWholeLog();
                Assert.AreEqual(2, CountOuterJoins(log));
                Assert.AreEqual(0, CountInnerJoins(log));
            }
        }

        [Test]
        public void AndShouldBeInnerJoin()
        {
            using (var sqlLog = new SqlLogSpy())
            using (var session = OpenSession())
            {
                session.Query<MyBO>().Where(b => b.BO1.I1 == 1 && b.BO2.J1 == 1).ToList();
                var log = sqlLog.GetWholeLog();
                Assert.AreEqual(0, CountOuterJoins(log));
                Assert.AreEqual(2, CountInnerJoins(log));
            }
        }

        [Test]
        public void ComparisonToConstantShouldBeInnerJoin()
        {
            using (var sqlLog = new SqlLogSpy())
            using (var session = OpenSession())
            {
                session.Query<MyBO>().Where(b => b.BO1.I1 == 1).ToList();
                var log = sqlLog.GetWholeLog();
                Assert.AreEqual(0, CountOuterJoins(log));
                Assert.AreEqual(1, CountInnerJoins(log));
            }
        }

        [Test]
        public void NotEqualsNullShouldBeInnerJoin()
        {
            using (var sqlLog = new SqlLogSpy())
            using (var session = OpenSession())
            {
                session.Query<MyBO>().Where(b => b.BO1.BO2 != null).ToList();
                var log = sqlLog.GetWholeLog();
                Assert.AreEqual(0, CountOuterJoins(log));
                Assert.AreEqual(1, CountInnerJoins(log));
            }
        }

        [Test]
        public void EqualsNullShouldBeOuterJoin()
        {
            using (var sqlLog = new SqlLogSpy())
            using (var session = OpenSession())
            {
                session.Query<MyBO>().Where(b => b.BO1.BO2 == null).ToList();
                var log = sqlLog.GetWholeLog();
                Assert.AreEqual(1, CountOuterJoins(log));
                Assert.AreEqual(0, CountInnerJoins(log));
            }
        }

        private int CountOuterJoins(string log)
        {
            return log.Split(new[] { "left outer join" }, StringSplitOptions.None).Count() - 1;
        }

        private int CountInnerJoins(string log)
        {
            return log.Split(new[] { "inner join" }, StringSplitOptions.None).Count() - 1;
        }
    }
}
