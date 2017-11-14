﻿using System.Collections.Generic;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.Futures
{
	[TestFixture]
	public class LinqFutureFixture : FutureFixture
	{
		[Test]
		public void DefaultReadOnlyTest()
		{
			//NH-3575
			using (var s = Sfi.OpenSession())
			{
				s.DefaultReadOnly = true;

				var persons = s.Query<Person>().ToFuture();

				Assert.IsTrue(persons.GetEnumerable().All(p => s.IsReadOnly(p)));
			}
		}

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
		public void CanUseToFutureWithContains()
		{
			using (var s = Sfi.OpenSession())
			{
				var ids = new[] { 1, 2, 3 };
				var persons10 = s.Query<Person>()
					.Where(p => ids.Contains(p.Id))
					.FetchMany(p => p.Children)
					.Skip(5)
					.Take(10)
					.ToFuture().GetEnumerable().ToList();

				Assert.IsNotNull(persons10);
			}
		}

		[Test]
		public void CanUseToFutureWithContains2()
		{
			using (var s = Sfi.OpenSession())
			{
				var ids = new[] { 1, 2, 3 };
				var persons10 = s.Query<Person>()
					.Where(p => ids.Contains(p.Id))
					.ToFuture()
					.GetEnumerable()
					.ToList();

				Assert.IsNotNull(persons10);
			}
		}

		[Test]
		public void CanUseSkipAndFetchManyWithToFuture()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p1 = new Person { Name = "Parent" };
				var p2 = new Person { Parent = p1, Name = "Child" };
				p1.Children.Add(p2);
				s.Save(p1);
				s.Save(p2);
				tx.Commit();

				s.Clear(); // we don't want caching
			}

			using (var s = Sfi.OpenSession())
			{
				var persons10 = s.Query<Person>()
					.FetchMany(p => p.Children)
					.Skip(5)
					.Take(10)
					.ToFuture();

				var persons5 = s.Query<Person>()
					.ToFuture();

				using (var logSpy = new SqlLogSpy())
				{
					foreach (var person in persons5.GetEnumerable()) { }

					foreach (var person in persons10.GetEnumerable()) { }

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);
				}
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
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			{
				var persons10 = s.Query<Person>()
					.Take(10)
					.ToFuture();
				var persons5 = s.Query<Person>()
					.Take(5)
					.ToFuture();

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
		public void CanUseFutureQueryWithAnonymousType()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			{
				var persons = s.Query<Person>()
					.Select(p => new { Id = p.Id, Name = p.Name })
					.ToFuture();
				var persons5 = s.Query<Person>()
					.Select(p => new { Id = p.Id, Name = p.Name })
					.Take(5)
					.ToFuture();

				using (var logSpy = new SqlLogSpy())
				{
					persons5.GetEnumerable().ToList(); // initialize the enumerable
					persons.GetEnumerable().ToList();

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);
				}
			}
		}

		[Test]
		public void CanUseFutureFetchQuery()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p1 = new Person { Name = "Parent" };
				var p2 = new Person { Parent = p1, Name = "Child" };
				p1.Children.Add(p2);
				s.Save(p1);
				s.Save(p2);
				tx.Commit();

				s.Clear(); // we don't want caching
			}

			using (var s = Sfi.OpenSession())
			{
				var persons = s.Query<Person>()
					.FetchMany(p => p.Children)
					.ToFuture();
				var persons10 = s.Query<Person>()
					.FetchMany(p => p.Children)
					.Take(10)
					.ToFuture();

				using (var logSpy = new SqlLogSpy())
				{

					Assert.That(persons.GetEnumerable().Any(x => x.Children.Any()), "No children found");
					Assert.That(persons10.GetEnumerable().Any(x => x.Children.Any()), "No children found");

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(1, events.Length);
				}
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void TwoFuturesRunInTwoRoundTrips()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			{
				using (var logSpy = new SqlLogSpy())
				{
					var persons10 = s.Query<Person>()
						.Take(10)
						.ToFuture();

					foreach (var person in persons10.GetEnumerable()) { } // fire first future round-trip

					var persons5 = s.Query<Person>()
						.Take(5)
						.ToFuture();

					foreach (var person in persons5.GetEnumerable()) { } // fire second future round-trip

					var events = logSpy.Appender.GetEvents();
					Assert.AreEqual(2, events.Length);
				}
			}
		}

		[Test]
		public void CanCombineSingleFutureValueWithEnumerableFutures()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenSession())
			{
				var persons = s.Query<Person>()
					.Take(10)
					.ToFuture();

				var personCount = s.Query<Person>()
					.Select(x => x.Id)
					.ToFutureValue();

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

		[Test(Description = "NH-2385")]
		public void CanCombineSingleFutureValueWithFetchMany()
		{
			int personId;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p1 = new Person { Name = "inserted name" };
				var p2 = new Person { Name = null };

				s.Save(p1);
				s.Save(p2);
				personId = p2.Id;
				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				var meContainer = s.Query<Person>()
								   .Where(x => x.Id == personId)
								   .FetchMany(x => x.Children)
								   .ToFutureValue();

				Assert.AreEqual(personId, meContainer.Value.Id);
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void CanExecuteMultipleQueriesOnSameExpression()
		{
			using (var s = Sfi.OpenSession())
			{
				IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

				var meContainer = s.Query<Person>()
					.Where(x => x.Id == 1)
					.ToFutureValue();

				var possiblefriends = s.Query<Person>()
					.Where(x => x.Id != 2)
					.ToFuture();

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
		public void UsingManyParametersAndQueries_DoesNotCauseParameterNameCollisions()
		{
			//GH-1357
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p1 = new Person { Name = "Person name", Age = 15};
				var p2 = new Person { Name = "Person name", Age = 5 };

				s.Save(p1);
				s.Save(p2);
				tx.Commit();
			}
			using (var s = Sfi.OpenSession())
			{
				var list = new List<IFutureEnumerable<Person>>();
				for (var i = 0; i < 12; i++)
				{
					var query = s.Query<Person>();
					for (var j = 0; j < 12; j++)
					{
						query = query.Where(x => x.Age > j);
					}
					list.Add(query.SetOptions(x => x.SetCacheable(true)).ToFuture());
				}
				foreach (var query in list)
				{
					var result = query.ToList();
					Assert.That(result.Count,Is.EqualTo(1));
				}
			}
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}
	}
}
