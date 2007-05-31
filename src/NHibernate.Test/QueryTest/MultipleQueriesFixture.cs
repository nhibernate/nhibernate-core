using System.Collections;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class MultipleQueriesFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
		}

		[TestFixtureSetUp]
		public void CheckMultiQuerySupport()
		{
			base.TestFixtureSetUp();
			IDriver driver = sessions.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
			{
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
			}
		}

		[Test]
		public void CanGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMutiQueryAndAssert();

			Hashtable cacheHashtable = GetHashTableUsedAsQueryCache();
			IList cachedListEntry = (IList)new ArrayList(cacheHashtable.Values)[0];
			IList cachedQuery = (IList)cachedListEntry[1];

			IList firstQueryResults = (IList)cachedQuery[0];
			firstQueryResults.Clear();
			firstQueryResults.Add(3);
			firstQueryResults.Add(4);

			IList secondQueryResults = (IList)cachedQuery[1];
			secondQueryResults[0] = 2L;

			using (ISession s = sessions.OpenSession())
			{
				IMultiQuery MultiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							.SetInt32(0, 50)
							.SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							.SetInt32(0, 50));
				MultiQuery.SetCacheable(true);
				IList results = MultiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(2, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(2L, count);
			}

			RemoveAllItems();
		}

		[Test]
		public void TwoMultiQueriesWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (ISession s = OpenSession())
			{
				IMultiQuery MultiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							.SetInt32(0, 50)
							.SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							.SetInt32(0, 50));
				MultiQuery.SetCacheable(true);
				IList results = MultiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			using (ISession s = OpenSession())
			{
				IMultiQuery MultiQuery = s.CreateMultiQuery()
						.Add(s.CreateQuery("from Item i where i.Id > ?")
								.SetInt32(0, 50)
								.SetFirstResult(20))
						.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
								.SetInt32(0, 50));
				MultiQuery.SetCacheable(true);
				IList results = MultiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(79, items.Count, "Should have gotten different result here, because the paging is different");
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParameters()
		{
			Hashtable cacheHashtable = GetHashTableUsedAsQueryCache();
			cacheHashtable.Clear();

			CreateItems();

			DoMutiQueryAndAssert();

			Assert.AreEqual(1, cacheHashtable.Count);

			RemoveAllItems();
		}

		private void DoMutiQueryAndAssert()
		{
			using (ISession s = OpenSession())
			{
				IMultiQuery MultiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							.SetInt32(0, 50)
							.SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							.SetInt32(0, 50));
				MultiQuery.SetCacheable(true);
				IList results = MultiQuery.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		private void CreateItems()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				for (int i = 0; i < 150; i++)
				{
					Item item = new Item();
					item.Id = i;
					s.Save(item);
				}
				t.Commit();
			}
		}

		private Hashtable GetHashTableUsedAsQueryCache()
		{
			ISessionFactoryImplementor factory = (ISessionFactoryImplementor)sessions;
			//need the inner hashtable in the cache
			HashtableCache cache = (HashtableCache)
								   typeof(StandardQueryCache)
									.GetField("queryCache", BindingFlags.Instance | BindingFlags.NonPublic)
									.GetValue(factory.GetQueryCache(null));

			return (Hashtable)typeof(HashtableCache)
								.GetField("hashtable", BindingFlags.Instance | BindingFlags.NonPublic)
								.GetValue(cache);
		}

		[Test]
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			using (ISession s = OpenSession())
			{
				for (int i = 0; i < 150; i++)
				{
					Item item = new Item();
					item.Id = i;
					s.Save(item);
				}
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IQuery getItems = s.CreateQuery("from Item i where i.Id > :id")
					.SetFirstResult(10);
				IQuery countItems = s.CreateQuery("select count(*) from Item i where i.Id > :id");

				IList results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.SetInt32("id", 50)
					.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				long count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
		}

		private void RemoveAllItems()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}

		[Test]
		public void CanUseSetParameterList()
		{
			using (ISession s = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IList results = s.CreateMultiQuery()
					.Add("from Item i where i.id in (:items)")
					.Add("select count(*) from Item i where i.id in (:items)")
					.SetParameterList("items", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })
					.List();

				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				long count = (long)counts[0];
				Assert.AreEqual(1L, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}

		[Test]
		public void CanExecuteMultiplyQueriesInSingleRoundTrip()
		{
			using (ISession s = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				s.Save(item);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				IQuery getItems = s.CreateQuery("from Item");
				IQuery countItems = s.CreateQuery("select count(*) from Item");

				IList results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.List();
				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				long count = (long)counts[0];
				Assert.AreEqual(1L, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}
	}
}