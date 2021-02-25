using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1730
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void HitCacheInSameSession()
		{
			Sfi.EvictQueries();
			Sfi.Statistics.Clear();
			var entities = new System.Collections.Generic.List<Entity>();

			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					for (int i = 0; i < 3; i++)
					{
						var e = new Entity { Name = "Name" + i };
						entities.Add(e);
						session.Save(e);
					}
					transaction.Commit();
				}

				var queryString = "from Entity";

				using (var tx = session.BeginTransaction())
				{
					// this query will hit the database and create the cache
					session.CreateQuery(queryString).SetCacheable(true).List();
					tx.Commit();
				}

				using (var transaction = session.BeginTransaction())
				{
					//and this one SHOULD served by the cache
					session.CreateQuery(queryString).SetCacheable(true).List();
					transaction.Commit();
				}

				var qs = Sfi.Statistics.GetQueryStatistics(queryString);
				Assert.AreEqual(1, qs.CacheHitCount);
				Assert.AreEqual(1, qs.CachePutCount);
			}
		}
	}
}
