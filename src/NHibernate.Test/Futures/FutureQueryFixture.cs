using System.Linq;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	[TestFixture]
	public class FutureQueryFixture : FutureFixture
	{
		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Person");
				transaction.Commit();
			}
		}

		[Test]
		public void DefaultReadOnlyTest()
		{
			CreatePersons();

			//NH-3575
			using (var s = Sfi.OpenSession())
			{
				s.DefaultReadOnly = true;

				var persons = s.CreateQuery("from Person").Future<Person>();

				Assert.IsTrue(persons.GetEnumerable().All(p => s.IsReadOnly(p)));
			}
		}

		[Test]
		public void CanUseFutureQuery()
		{
			using (var s = Sfi.OpenSession())
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
					foreach (var person in persons5.GetEnumerable())
					{

					}

					foreach (var person in persons10.GetEnumerable())
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
					var persons10 = s.CreateQuery("from Person")
						.SetMaxResults(10)
						.Future<Person>();

					foreach (var person in persons10.GetEnumerable()) { } // fire first future round-trip

					var persons5 = s.CreateQuery("from Person")
						.SetMaxResults(5)
						.Future<int>();

					foreach (var person in persons5.GetEnumerable()) { } // fire second future round-trip

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

				var persons = s.CreateQuery("from Person")
					.SetMaxResults(10)
					.Future<Person>();

				var personCount = s.CreateQuery("select count(*) from Person")
					.FutureValue<long>();

				using (var logSpy = new SqlLogSpy())
				{
					long count = personCount.Value;

					foreach (var person in persons.GetEnumerable())
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
			using (var s = Sfi.OpenSession())
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

					foreach (var person in possiblefriends.GetEnumerable())
					{
					}

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);
					var wholeLog = logSpy.GetWholeLog();
					string paramPrefix = ((DriverBase) Sfi.ConnectionProvider.Driver).NamedPrefix;
					Assert.That(
						wholeLog,
						Does.Contain(paramPrefix + "p0 = 1 [Type: Int32 (0:0:0)], " + paramPrefix + "p1 = 2 [Type: Int32 (0:0:0)]"));
				}
			}
		}

		[Test]
		public void FutureExecutedOnGetEnumerable()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
			try
			{
				using (var s = Sfi.OpenSession())
				{
					var persons = s.CreateQuery("from Person").Future<Person>();
					Sfi.Statistics.Clear();
					persons.GetEnumerable();
					Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
				}
			}
			finally
			{
				Sfi.Statistics.IsStatisticsEnabled = false;
			}
		}

		//NH-1953 - Future<> doesn't work on CreateFilter
		[Test]
		public void FutureOnFilter()
		{
			CreatePersons();

			using (var s = Sfi.OpenSession())
			{
				var person = s.Query<Person>().Where(n => n.Name == "ParentTwoChildren").FirstOrDefault();

				var f1 = s.CreateFilter(person.Children, "where Age > 30").Future<Person>();
				var f2 = s.CreateFilter(person.Children, "where Age > 5").Future<Person>();

				Assert.That(person.Children.Count, Is.EqualTo(2), "invalid test set up");
				Assert.That(f1.GetEnumerable().ToList().Count, Is.EqualTo(0), "Invalid filtered results");
				Assert.That(f2.GetEnumerable().ToList().Count, Is.EqualTo(1), "Invalid filtered results");
			}
		}
	}
}
