using log4net;
using NHibernate.Cfg;
using NUnit.Framework;
using log4net.Core;

namespace NHibernate.Test.NHSpecificTest.NH1093
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.CacheProvider, string.Empty);
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.UseSecondLevelCache, "true");
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		private void Cleanup()
		{
			using (ISession s = sessions.OpenSession())
			{
				using (s.BeginTransaction())
				{
					s.CreateQuery("delete from SimpleCached").ExecuteUpdate();
					s.Transaction.Commit();
				}
			}
		}

		private void FillDb()
		{
			using (ISession s = sessions.OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(new SimpleCached {Description = "Simple 1"});
					s.Save(new SimpleCached {Description = "Simple 2"});
					tx.Commit();
				}
			}
		}

		[Test]
		[Description("Without configured cache, shouldn't throw exception")]
		public void NoException()
		{
			FillDb();
			NormalList();
			CriteriaQueryCache();
			HqlQueryCache();
			Cleanup();
		}

		private void HqlQueryCache()
		{
			using (ISession s = OpenSession())
			{
				s.CreateQuery("from SimpleCached").SetCacheable(true).List<SimpleCached>();
			}
		}

		private void CriteriaQueryCache()
		{
			using (ISession s = OpenSession())
			{
				s.CreateCriteria<SimpleCached>().SetCacheable(true).List<SimpleCached>();
			}
		}

		private void NormalList()
		{
			using (ISession s = OpenSession())
			{
				s.CreateCriteria<SimpleCached>().List<SimpleCached>();
			}
		}

		protected override void BuildSessionFactory()
		{
			// Without configured cache, should log warn.
			using (var ls = new LogSpy(LogManager.GetLogger(typeof(Fixture).Assembly, "NHibernate"), Level.Warn))
			{
				base.BuildSessionFactory();
				Assert.That(ls.GetWholeLog(), Does.Contain("Fake cache used"));
			}
		}
	}
}
