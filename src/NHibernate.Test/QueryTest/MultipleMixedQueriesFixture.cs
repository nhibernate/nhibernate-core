using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Driver;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;

namespace NHibernate.Test.QueryTest
{
	public class MultipleMixedQueriesFixture : TestCase
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
			using (var s = sessions.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids)").AddEntity(typeof (Item)))
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids2)").AddEntity(typeof (Item)))
					.SetParameterList("ids", new[] {50})
					.SetParameterList("ids2", new[] {50});
				multiQuery.List();
			}
		}

		[Test]
		public void NH_1085_WillGiveReasonableErrorIfBadParameterName()
		{
			using (var s = sessions.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids)").AddEntity(typeof(Item)))
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:ids2)").AddEntity(typeof(Item)));
				var e = Assert.Throws<QueryException>(() => multiQuery.List());
				Assert.That(e.Message, Is.EqualTo("Not all named parameters have been set: ['ids'] [select * from ITEM where Id in (:ids)]"));
			}
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
			secondQueryResults[0] = 2L;

			using (var s = sessions.OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
							 .SetInt32(0, 50)
							 .SetFirstResult(10))
					.Add(s.CreateQuery("select count(*) from Item i where i.Id > ?")
							 .SetInt32(0, 50));
				multiQuery.SetCacheable(true);
				var results = multiQuery.List();
				var items = (IList)results[0];
				Assert.AreEqual(2, items.Count);
				var count = (long)((IList)results[1])[0];
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
					.Add(s.CreateSQLQuery("select * from ITEM where Id > :id").AddEntity(typeof(Item)))
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
					.Add(s.CreateSQLQuery("select * from ITEM where Id = :id or Id = :id2").AddEntity(typeof(Item)))
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
					.Add(s.CreateSQLQuery("select count(*) as count from ITEM where Id > ?").AddScalar("count", NHibernateUtil.Int64)
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
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
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
			var cacheHashtable = MultipleQueriesFixture.GetHashTableUsedAsQueryCache(sessions);
			cacheHashtable.Clear();

			CreateItems();

			DoMutiQueryAndAssert();

			Assert.AreEqual(1, cacheHashtable.Count);
		}

		private void DoMutiQueryAndAssert()
		{
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateMultiQuery()
					.Add(s.CreateSQLQuery("select * from ITEM where Id > ?").AddEntity(typeof(Item))
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
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			using (var s = OpenSession())
			{
				for (var i = 0; i < 150; i++)
				{
					var item = new Item();
					item.Id = i;
					s.Save(item);
				}
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateSQLQuery("select * from ITEM where Id > :id").AddEntity(typeof(Item)).SetFirstResult(10);
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
					.Add(s.CreateSQLQuery("select * from ITEM where Id in (:items)").AddEntity(typeof(Item)))
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
				var getItems = s.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
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

				var firstQuery = session.CreateSQLQuery("select * from ITEM where Id < :id").AddEntity(typeof(Item))
					.SetInt32("id", 50);

				var secondQuery = session.CreateQuery("from Item");

				multiQuery.Add("first", firstQuery).Add("second", secondQuery);

				var secondResult = (IList)multiQuery.GetResult("second");
				var firstResult = (IList)multiQuery.GetResult("first");

				Assert.Greater(secondResult.Count, firstResult.Count);
			}
		}

		[Test]
		public void CanNotAddQueryWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiQuery = session.CreateMultiQuery();

				var firstQuery = session.CreateSQLQuery("select * from ITEM where Id < :id").AddEntity(typeof(Item))
					.SetInt32("id", 50);

				try
				{
					IQuery secondQuery = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
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
		public void CanNotRetrieveQueryResultWithUnknownKey()
		{
			using (var session = OpenSession())
			{
				var multiQuery = session.CreateMultiQuery();

				multiQuery.Add("firstQuery", session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item)));

				try
				{
					multiQuery.GetResult("unknownKey");
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
		public void ExecutingQueryThroughMultiQueryTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var query = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item))
					.SetResultTransformer(transformer);
				session.CreateMultiQuery()
					.Add(query)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}
		}

		[Test]
		public void ExecutingQueryThroughMultiQueryTransformsResults_When_setting_on_multi_query_directly()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				IQuery query = session.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
				session.CreateMultiQuery()
					.Add(query)
					.SetResultTransformer(transformer)
					.List();

				Assert.IsTrue(transformer.WasTransformTupleCalled, "Transform Tuple was not called");
				Assert.IsTrue(transformer.WasTransformListCalled, "Transform List was not called");
			}
		}

		[Test]
		public void CanGetResultsInAGenericList()
		{
			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item");
				var countItems = s.CreateSQLQuery("select count(*) as count from ITEM").AddScalar("count", NHibernateUtil.Int64);

				var results = s.CreateMultiQuery()
					.Add(getItems)
					.Add<long>(countItems)
					.List();

				Assert.That(results[0], Is.InstanceOf<List<object>>());
				Assert.That(results[1], Is.InstanceOf<List<long>>());
			}
		}
	}
}