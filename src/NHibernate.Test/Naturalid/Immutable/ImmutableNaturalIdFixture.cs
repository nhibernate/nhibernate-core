using NHibernate.Cfg;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Naturalid.Immutable
{
	[TestFixture]
	public class ImmutableNaturalIdFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new string[] {"Naturalid.Immutable.User.hbm.xml"}; }
		}

		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.UseSecondLevelCache, "true");
			cfg.SetProperty(Environment.UseQueryCache, "true");
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void Update()
		{
			// prepare some test data...
			User user;
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				user = new User();
				user.UserName = "steve";
				user.Email = "steve@hibernate.org";
				user.Password = "brewhaha";
				session.Save(user);
				tran.Commit();
			}
			// 'user' is now a detached entity, so lets change a property and reattch...
			user.Password = "homebrew";
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Update(user);
				tran.Commit();
			}

			// clean up
			using (var session = OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete(user);
				tran.Commit();
			}
		}

		[Test]
		public void NaturalIdCheck()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();

			User u = new User("steve", "superSecret");
			s.Persist(u);
			u.UserName = "Steve";
			try
			{
				s.Flush();
				Assert.Fail();
			}
			catch (HibernateException) {}
			u.UserName = "steve";
			s.Delete(u);
			t.Commit();
			s.Close();
		}

		[Test]
		public void NaturalIdCache()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				User u = new User("steve", "superSecret");
				s.Persist(u);
				t.Commit();
				s.Close();
			}

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var u =
					(User)
					s.CreateCriteria(typeof(User)).Add(Restrictions.NaturalId().Set("UserName", "steve"))
					 .SetCacheable(true).UniqueResult();
				Assert.That(u, Is.Not.Null);
				t.Commit();
				s.Close();
			}

			Assert.AreEqual(1, Sfi.Statistics.QueryExecutionCount);
			Assert.AreEqual(0, Sfi.Statistics.QueryCacheHitCount);
			Assert.AreEqual(1, Sfi.Statistics.QueryCachePutCount);

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				User v = new User("gavin", "supsup");
				s.Persist(v);
				t.Commit();
				s.Close();
			}

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var u =
					(User)
					s.CreateCriteria(typeof(User)).Add(Restrictions.NaturalId().Set("UserName", "steve"))
					 .SetCacheable(true).UniqueResult();
				Assert.That(u, Is.Not.Null);
				Assert.AreEqual(0, Sfi.Statistics.QueryExecutionCount);
				Assert.AreEqual(1, Sfi.Statistics.QueryCacheHitCount);
				u =
					(User)
					s.CreateCriteria(typeof(User)).Add(Restrictions.NaturalId().Set("UserName", "steve"))
					 .SetCacheable(true).UniqueResult();
				Assert.That(u, Is.Not.Null);
				Assert.AreEqual(0, Sfi.Statistics.QueryExecutionCount);
				Assert.AreEqual(2, Sfi.Statistics.QueryCacheHitCount);
				t.Commit();
				s.Close();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from User");
				t.Commit();
				s.Close();
			}
		}
	}
}
