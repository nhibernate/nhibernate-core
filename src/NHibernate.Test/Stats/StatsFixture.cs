using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NHibernate.Stat;
using NUnit.Framework;

namespace NHibernate.Test.Stats
{
	[TestFixture]
	public class StatsFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Stats.Continent.hbm.xml" }; }
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.GenerateStatistics, "true");
		}

		private static Continent FillDb(ISession s)
		{
			Continent europe = new Continent();
			europe.Name="Europe";
			Country france = new Country();
			france.Name="France";
			europe.Countries=new HashSet<Country>();
			europe.Countries.Add(france);
			s.Save(france);
			s.Save(europe);
			return europe;
		}

		private static void CleanDb(ISession s)
		{
			s.Delete("from Locality");
			s.Delete("from Country");
			s.Delete("from Continent");
		}

		[Test]
		public void CollectionFetchVsLoad()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();

			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			Continent europe = FillDb(s);
			tx.Commit();
			s.Clear();

			tx = s.BeginTransaction();
			Assert.AreEqual(0, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount);
			Continent europe2 = s.Get<Continent>(europe.Id);
			Assert.AreEqual(0, stats.CollectionLoadCount, "Lazy true: no collection should be loaded");
			Assert.AreEqual(0, stats.CollectionFetchCount);

			int cc = europe2.Countries.Count;
			Assert.AreEqual(1, stats.CollectionLoadCount);
			Assert.AreEqual(1, stats.CollectionFetchCount, "Explicit fetch of the collection state");
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			stats.Clear();
			europe = FillDb(s);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			Assert.AreEqual(0, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount);

			europe2 = s.CreateQuery("from Continent a join fetch a.Countries where a.id = " + europe.Id).UniqueResult<Continent>();
			Assert.AreEqual(1, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount, "collection should be loaded in the same query as its parent");
			tx.Commit();
			s.Close();

			Mapping.Collection coll = cfg.GetCollectionMapping("NHibernate.Test.Stats.Continent.Countries");
			coll.FetchMode = FetchMode.Join;
			coll.IsLazy = false;
			ISessionFactory sf = cfg.BuildSessionFactory();
			stats = sf.Statistics;
			stats.Clear();
			stats.IsStatisticsEnabled = true;
			s = sf.OpenSession();
			tx = s.BeginTransaction();
			europe = FillDb(s);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			Assert.AreEqual(0, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount);

			europe2 = s.Get<Continent>(europe.Id);
			Assert.AreEqual(1, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount,
							"Should do direct load, not indirect second load when lazy false and JOIN");
			tx.Commit();
			s.Close();
			sf.Close();

			coll = cfg.GetCollectionMapping("NHibernate.Test.Stats.Continent.Countries");
			coll.FetchMode = FetchMode.Select;
			coll.IsLazy = false;
			sf = cfg.BuildSessionFactory();
			stats = sf.Statistics;
			stats.Clear();
			stats.IsStatisticsEnabled = true;
			s = sf.OpenSession();
			tx = s.BeginTransaction();
			europe = FillDb(s);
			tx.Commit();
			s.Clear();
			tx = s.BeginTransaction();
			Assert.AreEqual(0, stats.CollectionLoadCount);
			Assert.AreEqual(0, stats.CollectionFetchCount);
			europe2 = s.Get<Continent>(europe.Id);
			Assert.AreEqual(1, stats.CollectionLoadCount);
			Assert.AreEqual(1, stats.CollectionFetchCount, "Should do explicit collection load, not part of the first one");
			foreach (Country country in europe2.Countries)
			{
				s.Delete(country);
			}
			CleanDb(s);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void QueryStatGathering()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();

			ISession s = OpenSession();
			ITransaction tx = s.BeginTransaction();
			FillDb(s);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			string continents = "from Continent";
			int results = s.CreateQuery(continents).List().Count;
			QueryStatistics continentStats = stats.GetQueryStatistics(continents);
			Assert.IsNotNull(continentStats, "stats were null");
			Assert.AreEqual(1, continentStats.ExecutionCount, "unexpected execution count");
			Assert.AreEqual(results, continentStats.ExecutionRowCount, "unexpected row count");
			var maxTime = continentStats.ExecutionMaxTime;
			Assert.AreEqual(maxTime, stats.QueryExecutionMaxTime);
			Assert.AreEqual( continents, stats.QueryExecutionMaxTimeQueryString );

			IEnumerable itr = s.CreateQuery(continents).Enumerable();
			// Enumerable() should increment the execution count
			Assert.AreEqual(2, continentStats.ExecutionCount, "unexpected execution count");
			// but should not effect the cumulative row count
			Assert.AreEqual(results, continentStats.ExecutionRowCount, "unexpected row count");
			NHibernateUtil.Close(itr);
			tx.Commit();
			s.Close();

			// explicitly check that statistics for "split queries" get collected
			// under the original query
			stats.Clear();
			s = OpenSession();
			tx = s.BeginTransaction();
			string localities = "from Locality";
			results = s.CreateQuery(localities).List().Count;
			QueryStatistics localityStats = stats.GetQueryStatistics(localities);
			Assert.IsNotNull(localityStats, "stats were null");
			// ...one for each split query
			Assert.AreEqual(2, localityStats.ExecutionCount, "unexpected execution count");
			Assert.AreEqual(results, localityStats.ExecutionRowCount, "unexpected row count");
			maxTime = localityStats.ExecutionMaxTime;
			Assert.AreEqual(maxTime, stats.QueryExecutionMaxTime);
			Assert.AreEqual( localities, stats.QueryExecutionMaxTimeQueryString );
			tx.Commit();
			s.Close();
			Assert.IsFalse(s.IsOpen);

			// native sql queries
			stats.Clear();
			s = OpenSession();
			tx = s.BeginTransaction();
			string sql = "select Id, Name from Country";
			results = s.CreateSQLQuery(sql).AddEntity(typeof (Country)).List().Count;
			QueryStatistics sqlStats = stats.GetQueryStatistics(sql);
			Assert.IsNotNull(sqlStats, "sql stats were null");
			Assert.AreEqual(1, sqlStats.ExecutionCount, "unexpected execution count");
			Assert.AreEqual(results, sqlStats.ExecutionRowCount, "unexpected row count");
			maxTime = sqlStats.ExecutionMaxTime;
			Assert.AreEqual(maxTime, stats.QueryExecutionMaxTime);
			Assert.AreEqual( sql, stats.QueryExecutionMaxTimeQueryString);
			tx.Commit();
			s.Close();

			s = OpenSession();
			tx = s.BeginTransaction();
			CleanDb(s);
			tx.Commit();
			s.Close();
		}

		[Test]
		public void IncrementQueryExecutionCount_WhenExplicitQueryIsExecuted()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				FillDb(s);
				tx.Commit();
			}

			IStatistics stats = sessions.Statistics;
			stats.Clear();
			using (ISession s = OpenSession())
			{
				var r = s.CreateCriteria<Country>().List();
			}
			Assert.AreEqual(1, stats.QueryExecutionCount);

			stats.Clear();
			using (ISession s = OpenSession())
			{
				var r = s.CreateQuery("from Country").List();
			}
			Assert.AreEqual(1, stats.QueryExecutionCount);

			stats.Clear();

			var driver = sessions.ConnectionProvider.Driver;
			if (driver.SupportsMultipleQueries)
			{
				using (var s = OpenSession())
				{
					var r = s.CreateMultiQuery().Add("from Country").Add("from Continent").List();
				}
				Assert.AreEqual(1, stats.QueryExecutionCount);

				stats.Clear();
				using (var s = OpenSession())
				{
					var r = s.CreateMultiCriteria().Add(DetachedCriteria.For<Country>()).Add(DetachedCriteria.For<Continent>()).List();
				}
				Assert.AreEqual(1, stats.QueryExecutionCount);
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				CleanDb(s);
				tx.Commit();
			}
		}
	}
}
