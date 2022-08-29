using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Multi;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class QueryBatchFixture : TestCase
	{
		// This fixture aggregates most of the tests from MultiCriteriaFixture, MultipleMixedQueriesFixture and
		// MultipleQueriesFixture, rewritten for using QueryBatch instead of obsoleted MultiCriteria/MultiQuery.

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "SecondLevelCacheTest.Item.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override void OnSetUp()
		{
			Sfi.Statistics.Clear();
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

		#region Criteria

		[Test]
		public void CanExecuteMultipleCriteriaQueriesInSingleRoundTrip_InTransaction()
		{
			using (var s = OpenSession())
			{
				var item = new Item
				{
					Id = 1,
					Name = "foo"
				};
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			using (var transaction = s.BeginTransaction())
			{
				var getItems = s.CreateCriteria(typeof(Item));
				var countItems = s.CreateCriteria(typeof(Item))
								  .SetProjection(Projections.RowCount());

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<int>(countItems);
				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));
				Assert.That(fromDb.Name, Is.EqualTo("foo"));

				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(1));

				transaction.Commit();
			}
		}

		[Test]
		public void CanExecuteMultipleCriteriaQueriesInSingleRoundTrip()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateCriteria(typeof(Item));
				var countItems = s.CreateCriteria(typeof(Item))
								  .SetProjection(Projections.RowCount());

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<int>(countItems);
				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));

				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParametersAndCriteria()
		{
			Sfi.QueryCache.Clear();

			CreateItems();

			Sfi.Statistics.Clear();

			DoMultiCriteriaAndAssert();

			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
		}

		[Test]
		public void CanGetMultiCriteriaFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMultiCriteriaAndAssert();
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Cache puts");

			Sfi.Statistics.Clear();
			DoMultiCriteriaAndAssert();
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Prepared statements");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Cache misses");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2), "Cache hits");
		}

		[Test]
		public void CanUpdateStatisticsWhenGetMultiCriteriaFromSecondLevelCache()
		{
			CreateItems();

			DoMultiCriteriaAndAssert();
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(2));
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));

			DoMultiCriteriaAndAssert();
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2));
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(2));
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
		}

		[Test]
		public void TwoMultiCriteriaWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
								.Add(Restrictions.Gt("id", 50));
				var queries = s.CreateQueryBatch()
							   .Add<Item>(CriteriaTransformer.Clone(criteria).SetFirstResult(10).SetCacheable(true))
							   .Add<int>(
								   CriteriaTransformer
									   .Clone(criteria).SetProjection(Projections.RowCount()).SetCacheable(true));
				var items = queries.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(99));
			}

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
								.Add(Restrictions.Gt("id", 50));
				var queries = s.CreateQueryBatch()
							   .Add<Item>(CriteriaTransformer.Clone(criteria).SetFirstResult(20).SetCacheable(true))
							   .Add<int>(
								   CriteriaTransformer
									   .Clone(criteria).SetProjection(Projections.RowCount()).SetCacheable(true));
				var items = queries.GetResult<Item>(0);
				Assert.That(
					items.Count,
					Is.EqualTo(79),
					"Should have gotten different result here, because the paging is different");
				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(99));
			}
		}

		[Test]
		public void CanUseWithParameterizedCriteriaAndLimit()
		{
			CreateItems();

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
								.Add(Restrictions.Gt("id", 50));

				var queries = s.CreateQueryBatch()
							   .Add<Item>(
								   CriteriaTransformer.Clone(criteria)
													  .SetFirstResult(10))
							   .Add<int>(
								   CriteriaTransformer.Clone(criteria)
													  .SetProjection(Projections.RowCount()));
				var items = queries.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(99));
			}
		}

		[Test]
		public void CanUseCriteriaWithParameterList()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
								.Add(
									Restrictions.In(
										"id",
										new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));
				var queries = s.CreateQueryBatch()
							   .Add<Item>(CriteriaTransformer.Clone(criteria))
							   .Add<int>(
								   CriteriaTransformer.Clone(criteria)
													  .SetProjection(Projections.RowCount()));

				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));

				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanAddCriteriaWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateQueryBatch();

				var firstCriteria = session.CreateCriteria(typeof(Item))
										   .Add(Restrictions.Lt("id", 50));

				var secondCriteria = session.CreateCriteria(typeof(Item));

				multiCriteria.Add<Item>("firstCriteria", firstCriteria);
				multiCriteria.Add<Item>("secondCriteria", secondCriteria);

				var secondResult = multiCriteria.GetResult<Item>("secondCriteria");
				var firstResult = multiCriteria.GetResult<Item>("firstCriteria");

				Assert.That(secondResult.Count, Is.GreaterThan(firstResult.Count));
			}
		}

		[Test]
		public void CanAddDetachedCriteriaWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateQueryBatch();

				var firstCriteria = DetachedCriteria.For(typeof(Item))
													.Add(Restrictions.Lt("id", 50));

				var secondCriteria = DetachedCriteria.For(typeof(Item));

				multiCriteria.Add<Item>("firstCriteria", firstCriteria);
				multiCriteria.Add<Item>("secondCriteria", secondCriteria);

				var secondResult = multiCriteria.GetResult<Item>("secondCriteria");
				var firstResult = multiCriteria.GetResult<Item>("firstCriteria");

				Assert.That(secondResult.Count, Is.GreaterThan(firstResult.Count));
			}
		}

		[Test]
		public void CannotAddCriteriaWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateQueryBatch();

				var firstCriteria = session.CreateCriteria(typeof(Item))
										   .Add(Restrictions.Lt("id", 50));

				var secondCriteria = session.CreateCriteria(typeof(Item));

				multiCriteria.Add<Item>("commonKey", firstCriteria);
				Assert.That(
					() => multiCriteria.Add<Item>("commonKey", secondCriteria),
					Throws.ArgumentException);
			}
		}

		[Test]
		public void ExecutingCriteriaThroughTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var criteria = session.CreateCriteria(typeof(Item))
									  .SetResultTransformer(transformer);
				var multiCriteria = session.CreateQueryBatch()
										   .Add<object[]>(criteria);
				multiCriteria.GetResult<object[]>(0);

				Assert.That(transformer.WasTransformTupleCalled, Is.True, "Transform Tuple was not called");
				Assert.That(transformer.WasTransformListCalled, Is.True, "Transform List was not called");
			}
		}

		[Test]
		public void UsingManyParametersAndQueries_DoesNotCauseParameterNameCollisions()
		{
			//GH-1357
			using (var s = OpenSession())
			{
				var item = new Item { Id = 15 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var multi = s.CreateQueryBatch();

				for (var i = 0; i < 12; i++)
				{
					var criteria = s.CreateCriteria(typeof(Item));
					for (var j = 0; j < 12; j++)
					{
						criteria = criteria.Add(Restrictions.Gt("id", j));
					}

					// Parameter combining was used for cacheable queries, with previous implementation (multi-criteria)
					// Query batch does not do that, but still keeping the test.
					criteria.SetCacheable(true);

					multi.Add<Item>(criteria);
				}

				for (var i = 0; i < 12; i++)
				{
					Assert.That(multi.GetResult<Item>(i).Count, Is.EqualTo(1));
				}
			}
		}

		//NH-2428 - Session.MultiCriteria and FlushMode.Auto inside transaction (GH865)
		[Test]
		public void MultiCriteriaAutoFlush()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.FlushMode = FlushMode.Auto;
				var p1 = new Item
				{
					Name = "Person name",
					Id = 15
				};
				s.Save(p1);
				s.Flush();

				s.Delete(p1);
				var multi = s.CreateQueryBatch();
				multi.Add<int>(s.QueryOver<Item>().ToRowCountQuery());
				var count = multi.GetResult<int>(0).Single();
				tx.Commit();

				Assert.That(count, Is.EqualTo(0), "Session wasn't auto flushed.");
			}
		}

		private void DoMultiCriteriaAndAssert()
		{
			using (var s = OpenSession())
			{
				var criteria = s.CreateCriteria(typeof(Item))
								.Add(Restrictions.Gt("id", 50));
				var queries = s.CreateQueryBatch()
							   .Add<Item>(CriteriaTransformer.Clone(criteria).SetFirstResult(10).SetCacheable(true))
							   .Add<int>(
								   CriteriaTransformer
									   .Clone(criteria).SetProjection(Projections.RowCount()).SetCacheable(true));
				var items = queries.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = queries.GetResult<int>(1).Single();
				Assert.That(count, Is.EqualTo(99));
			}
		}

		#endregion

		#region HQL

		[Test]
		public void CanGetMultiHqlFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMultiHqlAndAssert();

			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Cache puts");

			Sfi.Statistics.Clear();
			DoMultiHqlAndAssert();
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Prepared statements");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Cache misses");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2), "Cache hits");
		}

		[Test]
		public void TwoMultiHqlWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateQuery("from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetFirstResult(10)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateQuery("select count(*) from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetCacheable(true));
				var items = multiQuery.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}

			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateQuery("from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetFirstResult(20)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateQuery("select count(*) from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetCacheable(true));
				var items = multiQuery.GetResult<Item>(0);
				Assert.That(
					items.Count,
					Is.EqualTo(79),
					"Should have gotten different result here, because the paging is different");
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParametersAndHql()
		{
			Sfi.QueryCache.Clear();

			CreateItems();

			Sfi.Statistics.Clear();

			DoMultiHqlAndAssert();

			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
		}

		[Test]
		public void CanUseHqlWithParameterizedQueriesAndLimit()
		{
			CreateItems();

			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item i where i.Id > :id")
								.SetInt32("id", 50)
								.SetFirstResult(10);
				var countItems = s.CreateQuery("select count(*) from Item i where i.Id > :id")
								  .SetInt32("id", 50);

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<long>(countItems);
				var items = queries.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		[Test]
		public void CanUseSetParameterListWithHql()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var queries = s.CreateQueryBatch()
							   .Add<Item>(
								   s.CreateQuery("from Item i where i.id in (:items)")
									.SetParameterList(
										"items",
										new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }))
							   .Add<long>(
								   s.CreateQuery("select count(*) from Item i where i.id in (:items)")
									.SetParameterList(
										"items",
										new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));

				var items = queries.GetResult<Item>(0);
				Assert.That(items.First().Id, Is.EqualTo(1));

				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(1L));
			}
		}

		[Test]
		public void CanExecuteMultiplyHqlInSingleRoundTrip()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateQuery("from Item");
				var countItems = s.CreateQuery("select count(*) from Item");

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<long>(countItems);
				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));

				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(1L));
			}
		}

		[Test]
		public void CanAddHqlWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiQuery = session.CreateQueryBatch();

				var firstQuery = session.CreateQuery("from Item i where i.Id < :id")
										.SetInt32("id", 50);

				var secondQuery = session.CreateQuery("from Item");

				multiQuery.Add<Item>("first", firstQuery).Add<Item>("second", secondQuery);

				var secondResult = multiQuery.GetResult<Item>("second");
				var firstResult = multiQuery.GetResult<Item>("first");

				Assert.That(secondResult.Count, Is.GreaterThan(firstResult.Count));
			}
		}

		[Test]
		public void CannotAddHqlWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateQueryBatch();

				var firstQuery = session.CreateQuery("from Item i where i.Id < :id")
										.SetInt32("id", 50);

				var secondQuery = session.CreateQuery("from Item");

				multiCriteria.Add<Item>("commonKey", firstQuery);

				Assert.That(
					() => multiCriteria.Add<Item>("commonKey", secondQuery),
					Throws.ArgumentException);
			}
		}

		[Test]
		public void ExecutingHqlThroughMultiQueryTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var criteria = session.CreateQuery("from Item")
									  .SetResultTransformer(transformer);
				session.CreateQueryBatch()
					   .Add<object[]>(criteria)
					   .GetResult<object[]>(0);

				Assert.That(transformer.WasTransformTupleCalled, Is.True, "Transform Tuple was not called");
				Assert.That(transformer.WasTransformListCalled, Is.True, "Transform List was not called");
			}
		}

		private void DoMultiHqlAndAssert()
		{
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateQuery("from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetFirstResult(10)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateQuery("select count(*) from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetCacheable(true));
				var items = multiQuery.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		#endregion

		#region Mixed

		[Test]
		public void NH_1085_WillGiveReasonableErrorIfBadParameterName()
		{
			using (var s = Sfi.OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateSQLQuery("select * from ITEM where Id in (:ids)")
									   .AddEntity(typeof(Item)))
								  .Add<Item>(s.CreateQuery("from Item i where i.Id in (:ids2)"));
				var e = Assert.Throws<QueryException>(multiQuery.Execute);
				Assert.That(
					e.Message,
					Is.EqualTo(
						"Not all named parameters have been set: ['ids'] [select * from ITEM where Id in (:ids)]"));
			}
		}

		[Test]
		public void CanGetMultiQueryFromSecondLevelCache()
		{
			CreateItems();
			//set the query in the cache
			DoMultiQueryAndAssert();

			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Cache puts");

			Sfi.Statistics.Clear();
			DoMultiQueryAndAssert();
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(0), "Prepared statements");
			Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "Cache misses");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2), "Cache hits");
		}

		[Test]
		public void CanSpecifyParameterOnMultiQueryWhenItIsNotUsedInAllQueries()
		{
			using (var s = OpenSession())
			{
				s.CreateQueryBatch()
				 .Add<Item>(s.CreateQuery("from Item"))
				 .Add<Item>(
					 s.CreateSQLQuery("select * from ITEM where Id = :id or Id = :id2")
					  .AddEntity(typeof(Item))
					  .SetParameter("id", 5)
					  .SetInt32("id2", 5))
				 .Add<Item>(
					 s.CreateQuery("from Item i where i.Id = :id2")
					  .SetInt32("id2", 5))
				 .Execute();
			}
		}

		[Test]
		public void TwoMultiQueriesWithDifferentPagingGetDifferentResultsWhenUsingCachedQueries()
		{
			CreateItems();
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateQuery("from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetFirstResult(10)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateSQLQuery("select count(*) as itemCount from ITEM where Id > ?")
									   .AddScalar("itemCount", NHibernateUtil.Int64)
									   .SetInt32(0, 50)
									   .SetCacheable(true));

				var items = multiQuery.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}

			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateQuery("from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetFirstResult(20)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateSQLQuery("select count(*) as itemCount from ITEM where Id > ?")
									   .AddScalar("itemCount", NHibernateUtil.Int64)
									   .SetInt32(0, 50)
									   .SetCacheable(true));
				var items = multiQuery.GetResult<Item>(0);
				Assert.That(
					items.Count,
					Is.EqualTo(79),
					"Should have gotten different result here, because the paging is different");
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		[Test]
		public void CanUseSecondLevelCacheWithPositionalParameters()
		{
			Sfi.QueryCache.Clear();

			CreateItems();

			Sfi.Statistics.Clear();

			DoMultiQueryAndAssert();

			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2));
		}

		[Test]
		public void CanUseWithParameterizedQueriesAndLimit()
		{
			CreateItems();

			using (var s = OpenSession())
			{
				var getItems = s.CreateSQLQuery("select * from ITEM where Id > :id")
								.AddEntity(typeof(Item))
								.SetFirstResult(10)
								.SetInt32("id", 50);
				var countItems = s.CreateQuery("select count(*) from Item i where i.Id > :id")
								  .SetInt32("id", 50);

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<long>(countItems);
				var items = queries.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		[Test]
		public void CanUseSetParameterList()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var queries = s.CreateQueryBatch()
							   .Add<Item>(
								   s.CreateSQLQuery("select * from ITEM where Id in (:items)")
									.AddEntity(typeof(Item))
									.SetParameterList(
										"items",
										new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }))
							   .Add<long>(s.CreateQuery("select count(*) from Item i where i.id in (:items)")
									 .SetParameterList(
										 "items",
										 new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }));

				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));

				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(1L));
			}
		}

		[Test]
		public void CanExecuteMultiplyQueriesInSingleRoundTrip()
		{
			using (var s = OpenSession())
			{
				var item = new Item { Id = 1 };
				s.Save(item);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var getItems = s.CreateSQLQuery("select * from ITEM").AddEntity(typeof(Item));
				var countItems = s.CreateQuery("select count(*) from Item");

				var queries = s.CreateQueryBatch()
							   .Add<Item>(getItems)
							   .Add<long>(countItems);
				var items = queries.GetResult<Item>(0);
				var fromDb = items.First();
				Assert.That(fromDb.Id, Is.EqualTo(1));

				var count = queries.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(1L));
			}
		}

		[Test]
		public void CanAddIQueryWithKeyAndRetrieveResultsWithKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiQuery = session.CreateQueryBatch();

				var firstQuery = session.CreateSQLQuery("select * from ITEM where Id < :id")
										.AddEntity(typeof(Item))
										.SetInt32("id", 50);

				var secondQuery = session.CreateQuery("from Item");

				multiQuery.Add<Item>("first", firstQuery).Add<Item>("second", secondQuery);

				var secondResult = multiQuery.GetResult<Item>("second");
				var firstResult = multiQuery.GetResult<Item>("first");

				Assert.That(secondResult.Count, Is.GreaterThan(firstResult.Count));
			}
		}

		[Test]
		public void ExecutingQueryThroughMultiQueryTransformsResults()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var transformer = new ResultTransformerStub();
				var query = session.CreateSQLQuery("select * from ITEM")
								   .AddEntity(typeof(Item))
								   .SetResultTransformer(transformer);
				session.CreateQueryBatch()
					   .Add<object[]>(query)
					   .GetResult<object[]>(0);

				Assert.That(transformer.WasTransformTupleCalled, Is.True, "Transform Tuple was not called");
				Assert.That(transformer.WasTransformListCalled, Is.True, "Transform List was not called");
			}
		}

		[Test]
		public void CannotAddMixedQueryWithKeyThatAlreadyExists()
		{
			using (var session = OpenSession())
			{
				var queries = session.CreateQueryBatch();

				var criteria = DetachedCriteria.For(typeof(Item))
											   .Add(Restrictions.Lt("id", 50));

				var query = session.Query<Item>();

				queries.Add<Item>("commonKey", criteria);

				Assert.That(
					() => queries.Add("commonKey", query),
					Throws.ArgumentException);
			}
		}

		[Test]
		public void CannotRetrieveResultWithUnknownKey()
		{
			CreateItems();

			using (var session = OpenSession())
			{
				var multiCriteria = session.CreateQueryBatch();

				var firstCriteria = session.CreateCriteria(typeof(Item))
										   .Add(Restrictions.Lt("id", 50));

				multiCriteria.Add<Item>("firstCriteria", firstCriteria);

				Assert.That(
					() => multiCriteria.GetResult<Item>("unknownKey"),
					Throws.InstanceOf<KeyNotFoundException>());
			}
		}

		private void DoMultiQueryAndAssert()
		{
			using (var s = OpenSession())
			{
				var multiQuery = s.CreateQueryBatch()
								  .Add<Item>(
									  s.CreateSQLQuery("select * from ITEM where Id > ?")
									   .AddEntity(typeof(Item))
									   .SetInt32(0, 50)
									   .SetFirstResult(10)
									   .SetCacheable(true))
								  .Add<long>(
									  s.CreateQuery("select count(*) from Item i where i.Id > ?")
									   .SetInt32(0, 50)
									   .SetCacheable(true));
				var items = multiQuery.GetResult<Item>(0);
				Assert.That(items.Count, Is.EqualTo(89));
				var count = multiQuery.GetResult<long>(1).Single();
				Assert.That(count, Is.EqualTo(99L));
			}
		}

		#endregion

		private void CreateItems()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				for (var i = 0; i < 150; i++)
				{
					var item = new Item { Id = i };
					s.Save(item);
				}

				t.Commit();
			}
		}
	}
}
