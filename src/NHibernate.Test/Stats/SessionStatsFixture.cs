using System.Collections;
using System.Collections.Generic;
using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.Stats
{
	using Criterion;

	[TestFixture]
	public class SessionStatsFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Stats.Continent2.hbm.xml" }; }
		}

		private static Continent FillDb(ISession s)
		{
			Continent europe = new Continent();
			europe.Name="Europe";
			Country france = new Country();
			france.Name="France";
			europe.Countries= new HashSet<Country>();
			europe.Countries.Add(france);
			s.Save(france);
			s.Save(europe);
			return europe;
		}

		private static void CleanDb(ISession s)
		{
			s.Delete("from Country");
			s.Delete("from Continent");
		}

		[Test]
		public void Can_use_cached_query_that_return_no_results()
		{
			Assert.IsTrue(Sfi.Settings.IsQueryCacheEnabled);

			using(ISession s = OpenSession())
			{
				IList list = s.CreateCriteria(typeof (Country))
					.Add(Restrictions.Eq("Name", "Narnia"))
					.SetCacheable(true)
					.List();

				Assert.AreEqual(0, list.Count);
			}

			using (ISession s = OpenSession())
			{
				IList list = s.CreateCriteria(typeof(Country))
					.Add(Restrictions.Eq("Name", "Narnia"))
					.SetCacheable(true)
					.List();

				Assert.AreEqual(0, list.Count);
			}
		}

		[Test]
		public void SessionStatistics()
		{
			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			IStatistics stats = Sfi.Statistics;
			stats.Clear();
			bool isStats = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			Continent europe = FillDb(s);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			ISessionStatistics sessionStats = s.Statistics;
			Assert.AreEqual(0, sessionStats.EntityKeys.Count);
			Assert.AreEqual(0, sessionStats.EntityCount);
			Assert.AreEqual(0, sessionStats.CollectionKeys.Count);
			Assert.AreEqual(0, sessionStats.CollectionCount);

			europe = s.Get<Continent>(europe.Id);
			NHibernateUtil.Initialize(europe.Countries);
			IEnumerator itr = europe.Countries.GetEnumerator();
			itr.MoveNext();
			NHibernateUtil.Initialize(itr.Current);
			Assert.AreEqual(2, sessionStats.EntityKeys.Count);
			Assert.AreEqual(2, sessionStats.EntityCount);
			Assert.AreEqual(1, sessionStats.CollectionKeys.Count);
			Assert.AreEqual(1, sessionStats.CollectionCount);

			CleanDb(s);
			tx.Commit();
			s.Close();

			stats.IsStatisticsEnabled = isStats;
		}
	}
}
