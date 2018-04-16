using System.Collections;
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

		protected override IList Mappings
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
			using (ISession session = OpenSession())
			{
				session.BeginTransaction();
				user = new User();
				user.UserName = "steve";
				user.Email = "steve@hibernate.org";
				user.Password = "brewhaha";
				session.Save(user);
				session.Transaction.Commit();
			}
			// 'user' is now a detached entity, so lets change a property and reattch...
			user.Password = "homebrew";
			using (ISession session = OpenSession())
			{
				session.BeginTransaction();
				session.Update(user);
				session.Transaction.Commit();
			}

			// clean up
			using (ISession session = OpenSession())
			{
				session.BeginTransaction();
				session.Delete(user);
				session.Transaction.Commit();
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
			ISession s = OpenSession();
			s.BeginTransaction();
			User u = new User("steve", "superSecret");
			s.Persist(u);
			s.Transaction.Commit();
			s.Close();

			Sfi.Statistics.Clear();

			s = OpenSession();
			s.BeginTransaction();
			u =
				(User)
				s.CreateCriteria(typeof (User)).Add(Restrictions.NaturalId().Set("UserName", "steve")).SetCacheable(true).
					UniqueResult();
			Assert.That(u, Is.Not.Null);
			s.Transaction.Commit();
			s.Close();

			Assert.AreEqual(1, Sfi.Statistics.QueryExecutionCount);
			Assert.AreEqual(0, Sfi.Statistics.QueryCacheHitCount);
			Assert.AreEqual(1, Sfi.Statistics.QueryCachePutCount);

			s = OpenSession();
			s.BeginTransaction();
			User v = new User("gavin", "supsup");
			s.Persist(v);
			s.Transaction.Commit();
			s.Close();

			Sfi.Statistics.Clear();

			s = OpenSession();
			s.BeginTransaction();
			u =
				(User)
				s.CreateCriteria(typeof(User)).Add(Restrictions.NaturalId().Set("UserName", "steve")).SetCacheable(true).
					UniqueResult();
			Assert.That(u, Is.Not.Null);
			Assert.AreEqual(0, Sfi.Statistics.QueryExecutionCount);
			Assert.AreEqual(1, Sfi.Statistics.QueryCacheHitCount);
			u =
				(User)
				s.CreateCriteria(typeof(User)).Add(Restrictions.NaturalId().Set("UserName", "steve")).SetCacheable(true).
					UniqueResult();
			Assert.That(u, Is.Not.Null);
			Assert.AreEqual(0, Sfi.Statistics.QueryExecutionCount);
			Assert.AreEqual(2, Sfi.Statistics.QueryCacheHitCount);
			s.Transaction.Commit();
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from User");
			s.Transaction.Commit();
			s.Close();
		}
	}
}