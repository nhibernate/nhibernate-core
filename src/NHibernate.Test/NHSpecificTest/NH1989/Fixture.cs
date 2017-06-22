using System.Data.Common;

using NUnit.Framework;

using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Engine;

namespace NHibernate.Test.NHSpecificTest.NH1989
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Environment.UseQueryCache] = "true";
		}

		protected override void OnSetUp()
		{
			// Clear cache at each test.
			RebuildSessionFactory();
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from User");
				tx.Commit();
			}
		}

		private static void DeleteObjectsOutsideCache(ISession s)
		{
			using (var cmd = s.Connection.CreateCommand())
			{
				cmd.CommandText = "DELETE FROM UserTable";
				cmd.ExecuteNonQuery();
			}
		}

		[Test]
		public void SecondLevelCacheWithSingleCacheableFuture()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User user = new User() { Name = "test" };
				s.Save(user);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				// Query results should be cached
				User user =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Not.Null);

				DeleteObjectsOutsideCache(s);
			}

			using (ISession s = OpenSession())
			{
				User user =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Not.Null,
					"entity not retrieved from cache");
			}
		}

		[Test]
		public void SecondLevelCacheWithDifferentRegionsFuture()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User user = new User() { Name = "test" };
				s.Save(user);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				// Query results should be cached
				User user =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.SetCacheRegion("region1")
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Not.Null);

				DeleteObjectsOutsideCache(s);
			}

			using (ISession s = OpenSession())
			{
				User user =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.SetCacheRegion("region2")
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Null,
					"entity from different region should not be retrieved");
			}
		}

		[Test]
		public void SecondLevelCacheWithMixedCacheableAndNonCacheableFuture()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User user = new User() { Name = "test" };
				s.Save(user);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				// cacheable Future, not evaluated yet
				IFutureValue<User> userFuture =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.FutureValue<User>();

				// non cacheable Future causes batch to be non-cacheable
				int count =
					s.CreateCriteria<User>()
						.SetProjection(Projections.RowCount())
						.FutureValue<int>()
						.Value;

				Assert.That(userFuture.Value, Is.Not.Null);
				Assert.That(count, Is.EqualTo(1));

				DeleteObjectsOutsideCache(s);
			}

			using (ISession s = OpenSession())
			{
				IFutureValue<User> userFuture =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.FutureValue<User>();

				int count =
					s.CreateCriteria<User>()
						.SetProjection(Projections.RowCount())
						.FutureValue<int>()
						.Value;

				Assert.That(userFuture.Value, Is.Null,
					"query results should not come from cache");
			}
		}

		[Test]
		public void SecondLevelCacheWithMixedCacheRegionsFuture()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User user = new User() { Name = "test" };
				s.Save(user);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				// cacheable Future, not evaluated yet
				IFutureValue<User> userFuture =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.SetCacheRegion("region1")
						.FutureValue<User>();

				// different cache-region causes batch to be non-cacheable
				int count =
					s.CreateCriteria<User>()
						.SetProjection(Projections.RowCount())
						.SetCacheable(true)
						.SetCacheRegion("region2")
						.FutureValue<int>()
						.Value;

				Assert.That(userFuture.Value, Is.Not.Null);
				Assert.That(count, Is.EqualTo(1));

				DeleteObjectsOutsideCache(s);
			}

			using (ISession s = OpenSession())
			{
				IFutureValue<User> userFuture =
					s.CreateCriteria<User>()
						.Add(Restrictions.NaturalId().Set("Name", "test"))
						.SetCacheable(true)
						.SetCacheRegion("region1")
						.FutureValue<User>();

				int count =
					s.CreateCriteria<User>()
						.SetProjection(Projections.RowCount())
						.SetCacheable(true)
						.SetCacheRegion("region2")
						.FutureValue<int>()
						.Value;

				Assert.That(userFuture.Value, Is.Null,
					"query results should not come from cache");
			}
		}

		[Test]
		public void SecondLevelCacheWithSingleCacheableQueryFuture()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				User user = new User() { Name = "test" };
				s.Save(user);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				// Query results should be cached
				User user =
					s.CreateQuery("from User u where u.Name='test'")
						.SetCacheable(true)
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Not.Null);

				DeleteObjectsOutsideCache(s);
			}

			using (ISession s = OpenSession())
			{
				User user =
					s.CreateQuery("from User u where u.Name='test'")
						.SetCacheable(true)
						.FutureValue<User>()
						.Value;

				Assert.That(user, Is.Not.Null,
					"entity not retrieved from cache");
			}
		}
	}
}
