using System.Collections.Generic;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.Futures
{
	[TestFixture]
	public class LinqFutureFixture : FutureFixture
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
		public void CanUseFutureQueryAndQueryOverForSatelessSession()
		{
			IgnoreThisTestIfMultipleQueriesArentSupportedByDriver();

			using (var s = Sfi.OpenStatelessSession())
			{
				var persons10 = s.Query<Person>()
					.Take(10)
					.ToFuture();
				var persons5 = s.QueryOver<Person>()
					.Take(5)
					.Future();

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
					list.Add(query.WithOptions(x => x.SetCacheable(true)).ToFuture());
				}
				foreach (var query in list)
				{
					var result = query.ToList();
					Assert.That(result.Count,Is.EqualTo(1));
				}
			}
		}

		[Test]
		public void FutureCombineCachedAndNonCachedQueries()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p1 = new Person
				{
					Name = "Person name",
					Age = 15
				};
				var p2 = new Person
				{
					Name = "Person name",
					Age = 20
				};

				s.Save(p1);
				s.Save(p2);
				tx.Commit();
			}

			using (var s = Sfi.OpenSession())
			{
				var list = new List<IFutureEnumerable<Person>>();
				for (var i = 0; i < 5; i++)
				{
					var i1 = i;
					var query = s.Query<Person>().Where(x => x.Age > i1);
					list.Add(query.WithOptions(x => x.SetCacheable(true)).ToFuture());
				}

				foreach (var query in list)
				{
					var result = query.GetEnumerable().ToList();
					Assert.That(result.Count, Is.EqualTo(2));
				}
			}

			//Check query.List returns data from cache
			Sfi.Statistics.IsStatisticsEnabled = true;
			using (var s = Sfi.OpenSession())
			{
				var list = new List<IEnumerable<Person>>();
				for (var i = 0; i < 5; i++)
				{
					var i1 = i;
					var query = s.Query<Person>().Where(x => x.Age > i1);

					list.Add(query.WithOptions(x => x.SetCacheable(true)).ToList());
				}

				foreach (var query in list)
				{
					var result = query.ToList();
					Assert.That(result.Count, Is.EqualTo(2));
				}

				Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Queries must be retrieved from cache");
			}

			//Check another Future returns data from cache
			Sfi.Statistics.Clear();
			using (var s = Sfi.OpenSession())
			{
				var list = new List<IFutureEnumerable<Person>>();
				//Reverse order of queries added to cache
				for (var i = 5 - 1; i >= 0; i--)
				{
					var i1 = i;
					var query = s.Query<Person>().Where(x => x.Age > i1);

					list.Add(query.WithOptions(x => x.SetCacheable(true)).ToFuture());
				}

				foreach (var query in list)
				{
					var result = query.GetEnumerable().ToList();
					Assert.That(result.Count, Is.EqualTo(2));
				}

				Assert.That(Sfi.Statistics.PrepareStatementCount , Is.EqualTo(0), "Future queries must be retrieved from cache");
			}
		}

		[Test]
		public void FutureAutoFlush()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.FlushMode = FlushMode.Auto;
				var p1 = new Person
				{
					Name = "Person name",
					Age = 15
				};
				s.Save(p1);
				s.Flush();

				s.Delete(p1);
				var count = s.QueryOver<Person>().ToRowCountQuery().FutureValue<int>().Value;
				tx.Commit();

				Assert.That(count, Is.EqualTo(0), "Session wasn't auto flushed.");
			}
		}

		[Test]
		public void FutureOnQueryableFilter()
		{
			CreatePersons();

			using (var s = Sfi.OpenSession())
			{
				var person = s.Query<Person>().Where(n => n.Name == "ParentTwoChildren").FirstOrDefault();
				var f1 = person.Children.AsQueryable().Where(p => p.Age > 30).ToFuture();
				var f2 = person.Children.AsQueryable().Where(p => p.Age > 5).ToFuture();

				Assert.That(person.Children.Count, Is.EqualTo(2), "invalid test set up");
				Assert.That(f1.GetEnumerable().ToList().Count, Is.EqualTo(0), "Invalid filtered results");
				Assert.That(f2.GetEnumerable().ToList().Count, Is.EqualTo(1), "Invalid filtered results");
			}
		}
	}
}
