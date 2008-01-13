using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Expressions;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;
using NHibernate.Driver;

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
		public void CanExecuteMultiplyQueriesInSingleRoundTrip_InTransaction()
		{
			using (ISession s = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				item.Name = "foo";
				s.Save(item);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				ITransaction transaction = s.BeginTransaction();
				ICriteria getItems = s.CreateCriteria(typeof(Item));
				ICriteria countItems = s.CreateCriteria(typeof(Item))
					.SetProjection(Projections.RowCount());

				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(getItems)
					.Add(countItems);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);
				Assert.AreEqual("foo", fromDb.Name);

				IList counts = (IList)results[1];
				int count = (int)counts[0];
				Assert.AreEqual(1, count);

				transaction.Commit();
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
				ICriteria getItems = s.CreateCriteria(typeof(Item));
				ICriteria countItems = s.CreateCriteria(typeof(Item))
					.SetProjection(Projections.RowCount());

				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(getItems)
					.Add(countItems);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				int count = (int)counts[0];
				Assert.AreEqual(1, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
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
			secondQueryResults[0] = 2;

			using (ISession s = sessions.OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.Gt("id", 50));
				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Assert.AreEqual(2, items.Count);
				int count = (int)((IList)results[1])[0];
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
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.Gt("id", 50));
				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				int count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.Gt("id", 50));
				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(20))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Assert.AreEqual(79, items.Count, "Should have gotten different result here, because the paging is different");
				int count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
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
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.Gt("id", 50));

				IList results = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria)
						.SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria)
						.SetProjection(Projections.RowCount()))
					.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				int count = (int)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			RemoveAllItems();
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
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.In("id", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
				IList results = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria))
					.Add(CriteriaTransformer.Clone(criteria)
						.SetProjection(Projections.RowCount()))
					.List();

				IList items = (IList)results[0];
				Item fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				IList counts = (IList)results[1];
				int count = (int)counts[0];
				Assert.AreEqual(1L, count);
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}


		private void DoMutiQueryAndAssert()
		{
			using (ISession s = OpenSession())
			{
				ICriteria criteria = s.CreateCriteria(typeof(Item))
					.Add(Expressions.Expression.Gt("id", 50));
				IMultiCriteria multiCriteria = s.CreateMultiCriteria()
					.Add(CriteriaTransformer.Clone(criteria).SetFirstResult(10))
					.Add(CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCount()));
				multiCriteria.SetCacheable(true);
				IList results = multiCriteria.List();
				IList items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				int count = (int)((IList)results[1])[0];
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


		private void RemoveAllItems()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Item");
				s.Flush();
			}
		}
	}
}