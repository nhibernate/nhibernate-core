using System.Linq;
using NHibernate.Criterion;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Futures
{
    [TestFixture]
    public class FutureCriteriaFixture : FutureFixture
    {
		[Test]
		public void DefaultReadOnlyTest()
		{
			//NH-3575
			using (var s = Sfi.OpenSession())
			{
				s.DefaultReadOnly = true;

				var persons = s.CreateCriteria(typeof(Person)).Future<Person>();

				Assert.IsTrue(persons.All(p => s.IsReadOnly(p)));
			}
		}

        [Test]
        public void CanUseFutureCriteria()
        {
            using (var s = Sfi.OpenSession())
            {
                IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                var persons10 = s.CreateCriteria(typeof(Person))
                    .SetMaxResults(10)
                    .Future<Person>();
                var persons5 = s.CreateCriteria(typeof(Person))
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
            using (var s = Sfi.OpenSession())
            {
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

                using (var logSpy = new SqlLogSpy())
                {
                    var persons10 = s.CreateCriteria(typeof(Person))
                        .SetMaxResults(10)
                        .Future<Person>();

                    foreach (var person in persons10) { } // fire first future round-trip

                    var persons5 = s.CreateCriteria(typeof(Person))
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
			using (var s = Sfi.OpenSession())
			{
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

				var persons = s.CreateCriteria(typeof(Person))
					.SetMaxResults(10)
					.Future<Person>();

				var personCount = s.CreateCriteria(typeof(Person))
					.SetProjection(Projections.RowCount())
					.FutureValue<int>();

				using (var logSpy = new SqlLogSpy())
				{
					int count = personCount.Value;

					foreach (var person in persons)
					{

					}

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);
				}
			}
		}
	}
}
