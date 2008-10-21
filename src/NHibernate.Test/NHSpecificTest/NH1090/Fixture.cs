using System.Collections;
using log4net.Core;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Driver;
using NHibernate.Stat;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1090
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			using (ITransaction tran = s.BeginTransaction())
			{
				MainClass m = new MainClass();
				m.Description = "description1";
				m.Title = "title1";

				MainClass m2 = new MainClass();
				m2.Description = "description2";
				m2.Title = "title2";
				s.Save(m);
				s.Save(m2);
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tran = s.BeginTransaction())
			{
				s.Delete("from MainClass");
				tran.Commit();
			}


		}
		[Test]
		public void CriteriaCanCacheSingleResultSingleProperty()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using (ISession session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof (MainClass));
				criteria.Add(Restrictions.Eq("Title", "title1"));
				criteria.SetProjection(Projections.Property("Description"));
				criteria.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				string result = (string)criteria.UniqueResult();
				Assert.AreEqual("description1", result);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				string newResult = (string)criteria.UniqueResult();
				Assert.AreEqual("description1", newResult);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}

		[Test]
		public void CriteriaCanCacheSingleResulMultipleProperty()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using (ISession session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(MainClass));
				criteria.Add(Restrictions.Eq("Title", "title1"));
				criteria.SetProjection(Projections.ProjectionList()
				                       	.Add(Projections.Property("Description"))
				                       	.Add(Projections.Property("Title")));
				criteria.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				object[] result = (object[])criteria.UniqueResult();
				Assert.AreEqual("description1", result[0]);
				Assert.AreEqual("title1", result[1]);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				object[] newResult = (object[])criteria.UniqueResult();
				Assert.AreEqual("description1", newResult[0]);
				Assert.AreEqual("title1", newResult[1]);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}



		[Test]
		public void CriteriaCanCacheEntity()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using(ISession session=OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof (MainClass));
				criteria.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				IList results = criteria.List();
				Assert.AreEqual(2,results.Count);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				results = criteria.List();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}


		[Test]
		public void CriteriaCanCacheTransformedResult()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using (ISession session = OpenSession())
			{
				ICriteria criteria = session.CreateCriteria(typeof(MainClass))
					.SetProjection(
					Projections.ProjectionList()
						.Add(Projections.Property("Title"))
						.Add(Projections.Property("Description")))
					.SetResultTransformer(new TupleToPropertyResultTransformer(typeof(MainClassWithoutId),"Title","Description"))
					.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				IList<MainClassWithoutId> results=criteria.List<MainClassWithoutId>();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				results = criteria.List<MainClassWithoutId>();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(2,results.Count);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}

		[Test]
		public void HqlCanCacheEntity()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using (ISession session = OpenSession())
			{
				IQuery query = session.CreateQuery("from MainClass")
										.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				IList results=query.List();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				results=query.List();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}

		[Test]
		public void HqlCanCacheTransformedResult()
		{
			IStatistics stats = sessions.Statistics;
			stats.Clear();
			bool isStatsEnabled = stats.IsStatisticsEnabled;
			stats.IsStatisticsEnabled = true;
			using (ISession session = OpenSession())
			{
				IQuery query = session.CreateQuery("select m.Title,m.Description from MainClass m")
					.SetResultTransformer(new TupleToPropertyResultTransformer(typeof (MainClassWithoutId), "Title", "Description"))
					.SetCacheable(true);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				IList results=query.List();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
				results=query.List();
				Assert.AreEqual(2, results.Count);
				Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			}
			stats.IsStatisticsEnabled = isStatsEnabled;
		}
	}
}