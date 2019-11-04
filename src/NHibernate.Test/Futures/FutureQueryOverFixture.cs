using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	[TestFixture]
	public class FutureQueryOverFixture : FutureFixture
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new Person());
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void DefaultReadOnlyTest()
		{
			//NH-3575
			using (var s = Sfi.OpenSession())
			{
				s.DefaultReadOnly = true;

				var persons = s.QueryOver<Person>().Future<Person>();

				Assert.IsTrue(persons.GetEnumerable().All(p => s.IsReadOnly(p)));
			}
		}

		[Test]
		public void CanUseFutureCriteria()
		{
			using (var s = Sfi.OpenSession())
			{
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

				var persons10 = s.QueryOver<Person>()
					.Take(10)
					.Future();
				var persons5 = s.QueryOver<Person>()
					.Select(p => p.Id)
					.Take(5)
					.Future<int>();

				using (var logSpy = new SqlLogSpy())
				{
					int actualPersons5Count = 0;
					foreach (var person in persons5.GetEnumerable())
						actualPersons5Count++;

					int actualPersons10Count = 0;
					foreach (var person in persons10.GetEnumerable())
						actualPersons10Count++;

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);

					Assert.That(actualPersons5Count, Is.EqualTo(1));
					Assert.That(actualPersons10Count, Is.EqualTo(1));
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
					var persons10 = s.QueryOver<Person>()
						.Take(10)
						.Future();

					foreach (var person in persons10.GetEnumerable()) { } // fire first future round-trip

					var persons5 = s.QueryOver<Person>()
						.Select(p => p.Id)
						.Take(5)
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

				var persons = s.QueryOver<Person>()
					.Take(10)
					.Future();

				var personIds = s.QueryOver<Person>()
					.Select(p => p.Id)
					.FutureValue<int>();

				var singlePerson = s.QueryOver<Person>()
					.FutureValue();

				using (var logSpy = new SqlLogSpy())
				{
					Person singlePersonValue = singlePerson.Value;
					int personId = personIds.Value;

					foreach (var person in persons.GetEnumerable())
					{
					}

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);

					Assert.That(singlePersonValue, Is.Not.Null);
					Assert.That(personId, Is.Not.EqualTo(0));
				}
			}
		}
	}
}
