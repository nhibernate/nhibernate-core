using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Engine;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	[TestFixture, Obsolete]
	public class MultipleQueriesFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
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
		public void NH_1085_WillIgnoreParametersIfDoesNotAppearInQuery()
		{
			using (var s = Sfi.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add("from Item i where i.Id in (:ids)")
					.Add("from Item i where i.Id in (:ids2)")
				.SetParameterList("ids", new[] { 50 })
				.SetParameterList("ids2", new[] { 50 });
				multiQuery.List();
			}
		}

		[Test]
		public void NH_1085_WillGiveReasonableErrorIfBadParameterName()
		{
			using (var s = Sfi.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add("from Item i where i.Id in (:ids)")
					.Add("from Item i where i.Id in (:ids2)");
				var e = Assert.Throws<QueryException>(() => multiQuery.List());
				Assert.That(e.Message, Is.EqualTo("Not all named parameters have been set: ['ids'] [from Item i where i.Id in (:ids)]"));
			}
		}

		[Test]
		public void CanGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();
			// Set the query in the cache.
			DoMultiQueryAndAssert();

			var cacheHashtable = GetHashTableUsedAsQueryCache(Sfi);
			var cachedListEntry = (IList) new ArrayList(cacheHashtable.Values)[0];
			// The first element is a timestamp, then only we have the cached data.
			var cachedQuery = (IList) cachedListEntry[1] ?? throw new InvalidOperationException("Cached data is null");

			var firstQueryResults = (IList) cachedQuery[0];
			firstQueryResults.Clear();
			firstQueryResults.Add(3);
			firstQueryResults.Add(4);

			var secondQueryResults = (IList) cachedQuery[1];
			secondQueryResults[0] = 2L;

			using (var s = Sfi.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				var results = multiQuery.List();
				var items = (IList) results[0];
				Assert.AreEqual(2, items.Count);
				var count = (long) ((IList) results[1])[0];
				Assert.AreEqual(2L, count);
			}
		}

		[Test]
		public void CanSpecifyParameterOnMultiQueryWhenItIsNotUsedInAllQueries()
		{
			using (var s = OpenSession())
			{
				s.CreateMultiQuery()
					.Add("from Item")
					.Add("from Item i where i.Id = :id")
					.SetParameter("id", 5)
					.List();
			}
		}

		[Test]
		public void CanSpecifyParameterOnMultiQueryWhenItIsNotUsedInAllQueries_MoreThanOneParameter()
		{
			using (var s = OpenSession())
			{
				s.CreateMultiQuery()
					.Add("from Item")
					.Add("from Item i where i.Id = :id or i.Id = :id2")
					.Add("from Item i where i.Id = :id2")
					.SetParameter("id", 5)
					.SetInt32("id2", 5)
					.List();
			}
		}

		[Test]
		public void TwoMultiQueriesWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				var results = multiQuery.List();
				var items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				var count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}

			using (var s = OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							 .SetInt32(0, 50)
							 .SetFirstResult(20))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				var results = multiQuery.List();
				var items = (IList)results[0];
				Assert.AreEqual(79, items.Count,
								"Should have gotten different result here, because the paging is different");
				var count = (long)((IList)results[1])[0];
				Assert.AreEqual(99L, count);
			}
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParameters()
		{
			var cacheHashtable = GetHashTableUsedAsQueryCache(Sfi);
			cacheHashtable.Clear();

			CreateItems();

			DoMultiQueryAndAssert();

			Assert.AreEqual(1, cacheHashtable.Count);
		}

		private void DoMultiQueryAndAssert()
		{
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateQuery("from Item i where i.Id > ?")
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				var results = multiQuery.List();
				var items = (IList) results[0];
				Assert.AreEqual(89, items.Count);
				var count = (long) ((IList) results[1])[0];
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

		/// <summary>
		/// Get the inner Hashtable from the IQueryCache.Cache
		/// </summary>
		/// <returns></returns>
		public static Hashtable GetHashTableUsedAsQueryCache(ISessionFactoryImplementor factory)
		{
			Hashtable hashTable = null;
			var cache = (HashtableCache)factory.GetQueryCache(null).Cache;
			var fieldInfo = typeof(HashtableCache).GetField("hashtable", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fieldInfo != null)
				hashTable = (Hashtable)fieldInfo.GetValue(cache);
			return hashTable;
		}

		[Test]
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			CreateItems();

			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item i where i.Id > :id")
					.SetFirstResult(10);
				var countItems = s.CreateQuery("select count(*) from Item i where i.Id > :id");

				var results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.SetInt32("id", 50)
					.List();
				var items = (IList)results[0];
				Assert.AreEqual(89, items.Count);
				var count = (long)((IList)results[1])[0];
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
				var results = s.CreateMultiQuery()
					.Add("from Item i where i.id in (:items)")
					.Add("select count(*) from Item i where i.id in (:items)")
					.SetParameterList("items", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 })
					.List();

				var items = (IList)results[0];
				var fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				var counts = (IList)results[1];
				var count = (long)counts[0];
				Assert.AreEqual(1L, count);
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
				var getItems = s.CreateQuery("from Item");
				var countItems = s.CreateQuery("select count(*) from Item");

				var results = s.CreateMultiQuery()
					.Add(getItems)
					.Add(countItems)
					.List();
				var items = (IList)results[0];
				var fromDb = (Item)items[0];
				Assert.AreEqual(1, fromDb.Id);

				var counts = (IList)results[1];
				var count = (long)counts[0];
				Assert.AreEqual(1L, count);
			}
		}

		[Test]
		public void CanAddIQueryWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiQuery = session.CreateMultiQuery();

				var firstQuery = session.CreateQuery("from Item i where i.Id < :id")
					.SetInt32("id", 50);

				var secondQuery = session.CreateQuery("from Item");

				multiQuery.Add("first", firstQuery).Add("second", secondQuery);

				var secondResult = (IList)multiQuery.GetResult("second");
				var firstResult = (IList)multiQuery.GetResult("first");

				Assert.Greater(secondResult.Count, firstResult.Count);
			}
		}

		[Test]
		public void CanNotAddCriteriaWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiQuery = session.CreateMultiQuery();

				var firstQuery = session.CreateQuery("from Item i where i.Id < :id")
					.SetInt32("id", 50);

				try
				{
					var secondQuery = session.CreateQuery("from Item");
					multiQuery.Add("first", firstQuery).Add("second", secondQuery);
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
			using (var session = OpenSession())
			{
				var multiQuery = session.CreateMultiQuery();

				multiQuery.Add("firstCriteria", session.CreateQuery("from Item"));

				try
				{
					var firstResult = (IList)multiQuery.GetResult("unknownKey");
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
		public void ExecutingCriteriaThroughMultiQueryTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var criteria = session.CreateQuery("from Item")
					.SetResultTransformer(transformer);
				session.CreateMultiQuery()
					.Add(criteria)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}
		}

		[Test]
		public void ExecutingCriteriaThroughMultiQueryTransformsResults_When_setting_on_multi_query_directly()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var query = session.CreateQuery("from Item");
				session.CreateMultiQuery()
					.Add(query)
					.SetResultTransformer(transformer)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}
		}

		[Test]
		public void ExecutingCriteriaThroughMultiCriteriaTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var criteria = session.CreateCriteria(typeof(Item))
					.SetResultTransformer(transformer);
				var multiCriteria = session.CreateMultiCriteria()
					.Add(criteria);
				multiCriteria.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled,"Transform List was not called");
			}
		}

		[Test]
		public void CanGetResultsInAGenericList()
		{
			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item");
				var countItems = s.CreateQuery("select count(*) from Item");

				var results = s.CreateMultiQuery()
					.Add(getItems)
					.Add<long>(countItems)
					.List();

				Assert.That(results[0], Is.InstanceOf<List<object>>());
				Assert.That(results[1], Is.InstanceOf<List<long>>());
			}
		}

		[Test]
		public void CanGetResultsInAGenericListClass()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var item1 = new Item { Id = 1,  Name = "test item"};
				var item2 = new Item { Id = 2,  Name = "test child", Parent = item1 };
				s.Save(item1);
				s.Save(item2);

				tx.Commit();
				s.Clear();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item");
				var parents = s.CreateQuery("select Parent from Item");

				var results = s.CreateMultiQuery()
					.Add(getItems)
					.Add<Item>(parents)
					.List();

				Assert.That(results[0], Is.InstanceOf<List<object>>());
				Assert.That(results[1], Is.InstanceOf<List<Item>>());
			}
		}
	}
}
