using NHibernate.Driver;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Futures
{
    using System.Collections;

    [TestFixture]
    public class FutureQueryFixture : FutureFixture
    {
        [Test]
        public void CanUseFutureQuery()
        {
            using (var s = sessions.OpenSession())
            {
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                var persons10 = s.CreateQuery("from Person")
                    .SetMaxResults(10)
                    .Future<Person>();
                var persons5 = s.CreateQuery("from Person")
                    .SetMaxResults(5)
                    .Future<int>();

                using (var logSpy = new SqlLogSpy())
                {
                    foreach (var person in persons5)
                    {

                    }

                    foreach (var person in persons10)
                    {

                    }

                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                }
            }
        }

        [Test]
        public void TwoFuturesRunInTwoRoundTrips()
        {
            using (var s = sessions.OpenSession())
            {
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                using (var logSpy = new SqlLogSpy())
                {
                    var persons10 = s.CreateQuery("from Person")
                        .SetMaxResults(10)
                        .Future<Person>();

                    foreach (var person in persons10) { } // fire first future round-trip

                    var persons5 = s.CreateQuery("from Person")
                        .SetMaxResults(5)
                        .Future<int>();

                    foreach (var person in persons5) { } // fire second future round-trip

                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(2, events.Length);
                }
            }
        }

        [Test]
        public void CanCombineSingleFutureValueWithEnumerableFutures()
        {
            using (var s = sessions.OpenSession())
            {
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                var persons = s.CreateQuery("from Person")
                    .SetMaxResults(10)
                    .Future<Person>();

                var personCount = s.CreateQuery("select count(*) from Person")
                    .FutureValue<long>();

                using (var logSpy = new SqlLogSpy())
                {
                    long count = personCount.Value;

                    foreach (var person in persons)
                    {
                    }

                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                }
            }
        }

        [Test]
        public void CanExecuteMultipleQueryWithSameParameterName()
        {
            using (var s = sessions.OpenSession())
            {
                IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();
		    
                var meContainer = s.CreateQuery("from Person p where p.Id = :personId")
                    .SetParameter("personId", 1)
                    .FutureValue<Person>();
		    
                var possiblefriends = s.CreateQuery("from Person p where p.Id != :personId")
                    .SetParameter("personId", 2)
                    .Future<Person>();

                using (var logSpy = new SqlLogSpy())
                {
                    var me = meContainer.Value;
			
                    foreach (var person in possiblefriends)
                    {
                    }
		    
                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                	var wholeLog = logSpy.GetWholeLog();
                    string paramPrefix = ((DriverBase) Sfi.ConnectionProvider.Driver).NamedPrefix;
                	Assert.True(wholeLog.Contains(paramPrefix + "p0 = 1 [Type: Int32 (0)], " + paramPrefix + "p1 = 2 [Type: Int32 (0)]"));
                }
            }

        }
    }
}
