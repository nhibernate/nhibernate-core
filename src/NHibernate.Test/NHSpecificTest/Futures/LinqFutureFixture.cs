using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.Futures
{
    using System.Collections;

    [TestFixture]
    public class LinqFutureFixture : FutureFixture
    {

        [Test]
        public void CoalesceShouldWorkForFutures()
        {
            int personId;
            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                var p1 = new Person { Name = "inserted name" };
                var p2 = new Person { Name = null };

                s.Save(p1);
                s.Save(p2);
                personId = p2.Id;
                tx.Commit();
            }

            using (ISession s = OpenSession())
            using (s.BeginTransaction())
            {
                var person = s.Query<Person>().Where(p => (p.Name ?? "e") == "e").ToFutureValue();
                Assert.AreEqual(personId, person.Value.Id);
            }

            using (ISession s = OpenSession())
            using (ITransaction tx = s.BeginTransaction())
            {
                s.Delete("from Person");
                tx.Commit();
            }
        }
        [Test]
        public void CanUseFutureQuery()
        {
            using (var s = sessions.OpenSession())
            {
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                var persons10 = s.Query<Person>()
                    .Take(10)
                    .ToFuture();
                var persons5 = s.Query<Person>()
                    .Take(5)
                    .ToFuture();

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
                    var persons10 = s.Query<Person>()
                        .Take(10)
                        .ToFuture();

                    foreach (var person in persons10) { } // fire first future round-trip

                    var persons5 = s.Query<Person>()
                        .Take(5)
                        .ToFuture();

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

                var persons = s.Query<Person>()
                    .Take(10)
                    .ToFuture();

                var personCount = s.Query<Person>()
                    .Select(x=>x.Id)
                    .ToFutureValue();

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
        public void CanExecuteMultipleQueriesOnSameExpression()
        {
            using (var s = sessions.OpenSession())
            {
                IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();
		    
                var meContainer = s.Query<Person>()
                    .Where(x=>x.Id == 1)
                    .ToFutureValue();
		    
                var possiblefriends = s.Query<Person>()
                    .Where(x => x.Id != 2)
                    .ToFuture();
		    
                using (var logSpy = new SqlLogSpy())
                {
                    var me = meContainer.Value;
			
                    foreach (var person in possiblefriends)
                    {
                    }
		    
                    var events = logSpy.Appender.GetEvents();
                    Assert.AreEqual(1, events.Length);
                	var wholeLog = logSpy.GetWholeLog();
                	Assert.True(wholeLog.Contains("@p0 = 1 [Type: Int32 (0)], @p1 = 2 [Type: Int32 (0)]"));
                }
            }

        }
    }
}
