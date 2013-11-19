using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class MultiCriteriaFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			this.sessions.Statistics.Clear();
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanExecuteMultiplyQueriesInSingleRoundTrip_InTransaction()
		{
			using (var s = OpenSession())
			{
				var item = new Item();
				item.Id = 1;
				item.Name = "foo";
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var transaction = s.BeginTransaction();
				var getItems = s.CreateCriteria(typeof(Item));
				var countItems = s.CreateCriteria(typeof(Item))
					.SetProjection(Projections.RowCount());

				var multiCriteria = s.CreateMultiCriteria()
					.Add(getItems)
					.Add(countItems);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				var fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);
				Assert.AreEqual("foo", fromDb.Name);

				var counts = (IList)results[1];
				var count = (int)counts[0];
				Assert.AreEqual(1, count);

				transaction.Commit();
			}
		}

		[Test]
		public void CanExecuteMultiplyQueriesInSingleRoundTrip()
		{
			using (var s = OpenSession())
			{
				var item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateCriteria(typeof(Item));
				var countItems = s.CreateCriteria(typeof(Item))
					.SetProjection(Projections.RowCount());

				var multiCriteria = s.CreateMultiCriteria()
					.Add(getItems)
					.Add(countItems);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				var fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				var counts = (IList)results[1];
				var count = (int)counts[0];
				Assert.AreEqual(1, count);
			}
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParameters()
		{
			var cacheHashtable = MultipleQueriesFixture.GetHashTableUsedAsQueryCache(sessions);
			cacheHashtable.Clear();

			CreateItems();

			DoMutiQueryAndAssert();

			Assert.AreEqual(1, cacheHashtable.Count);
		}

		[Test]
		public void CanGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMutiQueryAndAssert();

			var cacheHashtable = MultipleQueriesFixture.GetHashTableUsedAsQueryCache(sessions);
			var cachedListEntry = (IList)new ArrayList(cacheHashtable.Values)[0];
			var cachedQuery = (IList)cachedListEntry[1];

			var firstQueryResults = (IList)cachedQuery[0];
			firstQueryResults.Clear();
			firstQueryResults.Add(3);
			firstQueryResults.Add(4);

			var secondQueryResults = (IList)cachedQuery[1];
			secondQueryResults[0] = 2;

			using (var s = sessions.OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.Gt("id", 50));
				var multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				Assert.AreEqual(2, items.Count);
				var count = (int)((IList)results[1])[0];
				Assert.AreEqual(2L, count);
			}
		}

		[Test]
		public void CanUpdateStatisticsWhenGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();

			DoMutiQueryAndAssert();
			Assert.AreEqual(0, sessions.Statistics.QueryCacheHitCount);
			Assert.AreEqual(1, sessions.Statistics.QueryCacheMissCount);
			Assert.AreEqual(1, sessions.Statistics.QueryCachePutCount);

			DoMutiQueryAndAssert();
			Assert.AreEqual(1, sessions.Statistics.QueryCacheHitCount);
			Assert.AreEqual(1, sessions.Statistics.QueryCacheMissCount);
			Assert.AreEqual(1, sessions.Statistics.QueryCachePutCount);
		}

		[Test]
		public void TwoMultiQueriesWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.Gt("id", 50));
				var multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				var count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.Gt("id", 50));
				var multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(20))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				Assert.AreEqual(79, items.Count, "Should have gotten different result here, because the paging is different");
				var count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		[Test]
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			CreateItems();

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.Gt("id", 50));

				var results = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria)
						.SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria)
						.SetProjection(Projections.RowCount()))
					.List();
				var items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				var count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		[Test]
		public void CanUseSetParameterList()
		{
			using (var s = OpenSession())
			{
				var item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.In("id", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
				var results = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria))
					.Add(CriteriaTransformer.Clone(criteria)
						.SetProjection(Projections.RowCount()))
					.List();

				var items = (IList)results[0];
				var fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				var counts = (IList)results[1];
				var count = (int)counts[0];
				Assert.AreEqual(1L, count);
			}
		}

		[Test]
		public void CanAddCriteriaWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = session.CreateCriteria(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				var secondCriteria = session.CreateCriteria(typeof(Item));

				multiCriteria.Add("firstCriteria", firstCriteria);
				multiCriteria.Add("secondCriteria", secondCriteria);

				var secondResult = (IList)multiCriteria.GetResult("secondCriteria");
				var firstResult = (IList)multiCriteria.GetResult("firstCriteria");

				Assert.Greater(secondResult.Count, firstResult.Count);
			}
		}

		[Test]
		public void CanAddDetachedCriteriaWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = DetachedCriteria.For(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				var secondCriteria = DetachedCriteria.For(typeof(Item));

				multiCriteria.Add("firstCriteria", firstCriteria);
				multiCriteria.Add("secondCriteria", secondCriteria);

				var secondResult = (IList)multiCriteria.GetResult("secondCriteria");
				var firstResult = (IList)multiCriteria.GetResult("firstCriteria");

				Assert.Greater(secondResult.Count, firstResult.Count);
			}
		}

		[Test]
		public void CanNotAddCriteriaWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = session.CreateCriteria(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				var secondCriteria = session.CreateCriteria(typeof(Item));

				multiCriteria.Add("firstCriteria", firstCriteria);

				try
				{
					multiCriteria.Add("firstCriteria", secondCriteria);
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		[Test]
		public void CanNotAddDetachedCriteriaWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = DetachedCriteria.For(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				var secondCriteria = DetachedCriteria.For(typeof(Item));

				multiCriteria.Add("firstCriteria", firstCriteria);

				try
				{
					multiCriteria.Add("firstCriteria", secondCriteria);
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		[Test]
		public void CanNotRetrieveCriteriaResultWithUnknownKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = session.CreateCriteria(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				multiCriteria.Add("firstCriteria", firstCriteria);

				try
				{
					multiCriteria.GetResult("unknownKey");
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		[Test]
		public void CanNotRetrieveDetachedCriteriaResultWithUnknownKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateMultiCriteria();

				var firstCriteria = DetachedCriteria.For(typeof(Item))
					.Add(Restrictions.Lt("id", 50));

				multiCriteria.Add("firstCriteria", firstCriteria);

				try
				{
					multiCriteria.GetResult("unknownKey");
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception)
				{
					Assert.Fail("This should've thrown an InvalidOperationException");
				}
			}
		}

		private void DoMutiQueryAndAssert()
		{
			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
					.Add(Restrictions.Gt("id", 50));
				var multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				var results = multiCriteria.List();
				var items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				var count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		private void CreateItems()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				for (var i = 0; i < 150; i++)
				{
					var item = new Item();
					item.Id = i;
					s.Save(item);
				}
				t.Commit();
			}
		}

		[Test]
		public void CanGetResultInAGenericList()
		{
			using (var s = OpenSession())
			{
				var getItems = s.CreateCriteria(typeof(Item));
				var countItems = s.CreateCriteria(typeof(Item))
					.SetProjection(Projections.RowCount());

				var multiCriteria = s.CreateMultiCriteria()
					.Add(getItems) // we expect a non-generic result from this (ArrayList)
					.Add<int>(countItems); // we expect a generic result from this (List<int>)
				var results = multiCriteria.List();

				Assert.That(results[0], Is.InstanceOf<List<object>>());
				Assert.That(results[1], Is.InstanceOf<List<int>>());
			}
		}
	}
}
