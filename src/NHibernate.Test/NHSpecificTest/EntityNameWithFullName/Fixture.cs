using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EntityNameWithFullName
{
    public class Fixture : BugTestCase
    {
        protected override void OnTearDown()
        {
            using (var s = OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    s.CreateSQLQuery("delete from Parent").ExecuteUpdate();
                    tx.Commit();
                }
            }
        }

        [Test]
        public void CanPersistAndRead()
        {
            using (var s = OpenSession())
            {
                using (var tx = s.BeginTransaction())
                {
                    s.Save("NHibernate.Test.NHSpecificTest.EntityNameWithFullName.Parent", new Dictionary<string, object>
					                      	{
					                      		{"SomeData", "hello"}
					                      	});
                    tx.Commit();
                }
            }
            using (var s = OpenSession())
            {
                using (s.BeginTransaction())
                {
                    var p = (IDictionary)s.CreateQuery(@"select p from NHibernate.Test.NHSpecificTest.EntityNameWithFullName.Parent p where p.SomeData = :data")
                            .SetString("data", "hello")
                            .List()[0];
                    Assert.AreEqual("hello", p["SomeData"]);
                }
            }
        }

        [Test]
        public void OnlyOneSelect()
        {
            using (var s = OpenSession())
            {
                var sf = s.SessionFactory;
                var onOffBefore = turnOnStatistics(s);
                try
                {
                    using (s.BeginTransaction())
                    {
                        s.CreateQuery(@"select p from NHibernate.Test.NHSpecificTest.EntityNameWithFullName.Parent p where p.SomeData = :data")
                                .SetString("data", "hello")
                                .List();
                    }
                    Assert.AreEqual(1, sf.Statistics.QueryExecutionCount);
                }
                finally
                {
                    sf.Statistics.IsStatisticsEnabled = onOffBefore;                    
                }
            }
        }

        private static bool turnOnStatistics(ISession session)
        {
            var onOff = session.SessionFactory.Statistics.IsStatisticsEnabled;
            session.SessionFactory.Statistics.IsStatisticsEnabled = true;
            session.SessionFactory.Statistics.Clear();
            return onOff;
        }
    }
}
