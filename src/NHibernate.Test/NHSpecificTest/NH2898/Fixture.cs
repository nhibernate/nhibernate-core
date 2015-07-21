using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2898
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			cfg.Properties[Environment.CacheProvider] = typeof (BinaryFormatterCacheProvider).AssemblyQualifiedName;
			cfg.Properties[Environment.UseQueryCache] = "true";
			sessions = (ISessionFactoryImplementor) cfg.BuildSessionFactory();

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				for (var i = 0; i < 5; i++)
				{
					var obj = new ItemWithLazyProperty
								  {
									  Id = i + 1,
									  Name = "Name #" + i,
									  Description = "Description #" + i,
								  };
					session.Save(obj);
				}

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				session.Delete("from ItemWithLazyProperty");
				session.Flush();
			}
		}

		[Test]
		public void UnfetchedLazyPropertyEquality()
		{
			var first = new UnfetchedLazyProperty();
			var second = new UnfetchedLazyProperty();

			Assert.That(Equals(first, second), Is.True);
		}

		[Test]
		public void UnfetchedLazyPropertyIsNotEqualToUnknownBackrefProperty()
		{
			var first = new UnfetchedLazyProperty();
			var second = new UnknownBackrefProperty();

			Assert.That(Equals(first, second), Is.False);
		}

		[Test]
		public void SecondLevelCacheWithCriteriaQueries()
		{
			using (var session = OpenSession())
			{
				var list = session.CreateCriteria(typeof (ItemWithLazyProperty))
					.Add(Restrictions.Gt("Id", 2))
					.SetCacheable(true)
					.List();
				Assert.AreEqual(3, list.Count);

				using (var cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM ItemWithLazyProperty";
					cmd.ExecuteNonQuery();
				}
			}

			using (var session = OpenSession())
			{
				//should bring from cache
				var list = session.CreateCriteria(typeof (ItemWithLazyProperty))
					.Add(Restrictions.Gt("Id", 2))
					.SetCacheable(true)
					.List();
				Assert.AreEqual(3, list.Count);
			}
		}

		[Test]
		public void SecondLevelCacheWithHqlQueries()
		{
			using (var session = OpenSession())
			{
				var list = session.CreateQuery("from ItemWithLazyProperty i where i.Id > 2")
					.SetCacheable(true)
					.List();
				Assert.AreEqual(3, list.Count);

				using (var cmd = session.Connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM ItemWithLazyProperty";
					cmd.ExecuteNonQuery();
				}
			}

			using (var session = OpenSession())
			{
				//should bring from cache
				var list = session.CreateQuery("from ItemWithLazyProperty i where i.Id > 2")
					.SetCacheable(true)
					.List();
				Assert.AreEqual(3, list.Count);
			}
		}
	}
}