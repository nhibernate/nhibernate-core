using NUnit.Framework;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH2112
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void Configure(Configuration configuration)
        {
            configuration.SetProperty(Environment.GenerateStatistics, "true");
            configuration.SetProperty(Environment.BatchSize, "0");
        }

        protected override void OnTearDown()
        {
            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                s.CreateSQLQuery("DELETE FROM AMapB").ExecuteUpdate();
                s.CreateSQLQuery("DELETE FROM TableA").ExecuteUpdate();
                s.CreateSQLQuery("DELETE FROM TableB").ExecuteUpdate();
                tx.Commit();
            }
        }

        [Test]
        public void Test()
        {
            A a;
            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                a = new A();
                a.Name = "A";
                B b1 = new B{ Name = "B1"};
                s.Save(b1);
                B b2 = new B{ Name = "B2"};
                s.Save(b2);
                a.Map.Add(b1 , "B1Text");
                a.Map.Add(b2, "B2Text");
                s.Save(a);
                s.Flush();
                tx.Commit();
            }
            ClearCounts();
            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                A aCopy = (A)s.Merge(a);
                s.Flush();
                tx.Commit();
            }
            AssertUpdateCount(0);
            AssertInsertCount(0);
        }
        protected void ClearCounts()
        {
            Sfi.Statistics.Clear();
        }

        protected void AssertInsertCount(long expected)
        {
            Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(expected), "unexpected insert count");
        }

        protected void AssertUpdateCount(int expected)
        {
            Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(expected), "unexpected update count");
        }

        protected void AssertDeleteCount(int expected)
        {
            Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(expected), "unexpected delete count");
        }

    }
}
