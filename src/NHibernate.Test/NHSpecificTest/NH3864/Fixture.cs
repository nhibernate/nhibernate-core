using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3864
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			Clear2ndLevelCache();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var p1 = new Person() { Name = "A" };
				var p1c1 = new Person() { Name = "AA" };
				var p1c2 = new Person() { Name = "AB" };
				var p1c3 = new Person() { Name = "AC" };
				p1.Children = new HashSet<Person>(new[] { p1c1, p1c2, p1c3 });
				s.Save(p1);

				var p2 = new Person() { Name = "B" };
				var p2c1 = new Person() { Name = "BA" };
				var p2c2 = new Person() { Name = "BB" };
				var p2c3 = new Person() { Name = "BC" };
				p2.Children = new HashSet<Person>(new[] { p2c1, p2c2, p2c3 });
				s.Save(p2);

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
		public void CacheableMulticriteria_QueryOverWithAliasedJoinQueryOver()
		{
			ExecuteActionTwiceSecondRunEnsureNoSqlExecuted(() =>
				{
					using (var s = Sfi.OpenSession())
					{
						var query = CreateQueryOverWithAliasedJoinQueryOver(s);

						var multiCriteria = s.CreateMultiCriteria();
						multiCriteria.Add("myQuery", query);
						multiCriteria.SetCacheable(true);

						var list = (IList<Person>)multiCriteria.GetResult("myQuery");
						AssertQueryResult(list);
					}
				});
		}

		[Test]
		public void CacheableFuture_QueryOverWithAliasedJoinQueryOver()
		{
			ExecuteActionTwiceSecondRunEnsureNoSqlExecuted(() =>
				{
					using (var s = Sfi.OpenSession())
					{
						var query = CreateQueryOverWithAliasedJoinQueryOver(s)
							.Cacheable()
							.Future();

						var list = query.ToList();
						AssertQueryResult(list);
					}
				});
		}

		[Test]
		public void CacheableMulticriteria_QueryOverWithJoinAlias()
		{
			ExecuteActionTwiceSecondRunEnsureNoSqlExecuted(() =>
				{
					using (var s = Sfi.OpenSession())
					{
						var query = CreateQueryOverWithJoinAlias(s);

						var multiCriteria = s.CreateMultiCriteria();
						multiCriteria.Add("myQuery", query);
						multiCriteria.SetCacheable(true);

						var list = (IList<Person>)multiCriteria.GetResult("myQuery");
						AssertQueryResult(list);
					}
				});
		}

		[Test]
		public void CacheableFuture_QueryOverWithJoinAlias()
		{
			ExecuteActionTwiceSecondRunEnsureNoSqlExecuted(() =>
				{
					using (var s = Sfi.OpenSession())
					{
						var query = CreateQueryOverWithJoinAlias(s)
							.Cacheable()
							.Future();

						var list = query.ToList();
						AssertQueryResult(list);
					}
				});
		}

		private static void AssertQueryResult(IList<Person> list)
		{
			Assert.AreEqual(6, list.Count, "Returned records count is wrong.");
			var person1 = list.FirstOrDefault(p => p.Name == "A");
			Assert.NotNull(person1);
			var person2 = list.FirstOrDefault(p => p.Name == "B");
			Assert.NotNull(person2);

			CollectionAssert.AreEquivalent(person1.Children.Select(c => c.Name), new[] { "AA", "AB", "AC" });
			CollectionAssert.AreEquivalent(person2.Children.Select(c => c.Name), new[] { "BA", "BB", "BC" });
		}

		private static IQueryOver<Person, Person> CreateQueryOverWithJoinAlias(ISession session)
		{
			Person childAlias = null;
			return session.QueryOver<Person>()
						  .Where(p => p.Parent == null)
						  .JoinAlias(x => x.Children, () => childAlias);
		}

		private static IQueryOver<Person, Person> CreateQueryOverWithAliasedJoinQueryOver(ISession session)
		{
			Person childAlias = null;
			return session.QueryOver<Person>()
						  .Where(p => p.Parent == null)
						  .JoinQueryOver(x => x.Children, () => childAlias);
		}

		private void Clear2ndLevelCache()
		{
			Sfi.EvictQueries();
			Sfi.Evict(typeof(Person));
		}

		private static void ExecuteActionTwiceSecondRunEnsureNoSqlExecuted(System.Action action)
		{
			action();
			EnsureNoSqlExecutedOnDb(action);
		}

		private static void EnsureNoSqlExecutedOnDb(System.Action action)
		{
			using (var sqlLogSpy = new SqlLogSpy())
			{
				action();
				if (!string.IsNullOrEmpty(sqlLogSpy.GetWholeLog()))
				{
					Assert.Fail("SQL executed. 2nd level cache should be used instead.");
				}
			}
		}
	}
}
