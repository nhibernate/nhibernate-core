using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Multi;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	[TestFixture]
	public class QueryBatchFixture : TestCaseMappingByCode
	{
		private Guid _parentId;
		private Guid _eagerId;

		protected override string CacheConcurrencyStrategy => "nonstrict-read-write";

		[Test]
		public void CanCombineCriteriaAndHqlInFuture()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var future1 = session.QueryOver<EntityComplex>()
						.Where(x => x.Version >= 0)
						.TransformUsing(new ListTransformerToInt()).Future<int>();

				var future2 = session.Query<EntityComplex>().Where(ec => ec.Version > 2).ToFuture();
				var future3 = session.Query<EntitySimpleChild>().Select(sc => sc.Name).ToFuture();

				var future4 = session
						.Query<EntitySimpleChild>()
						.ToFutureValue(sc => sc.FirstOrDefault());

				Assert.That(future1.GetEnumerable().Count(), Is.GreaterThan(0), "Empty results are not expected");
				Assert.That(future2.GetEnumerable().Count(), Is.EqualTo(0), "This query should not return results");
				Assert.That(future3.GetEnumerable().Count(), Is.GreaterThan(1), "Empty results are not expected");
				Assert.That(future4.Value, Is.Not.Null, "Loaded entity should not be null");

				if (SupportsMultipleQueries)
					Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanCombineCriteriaAndHqlInBatch()
		{
			using (var session = OpenSession())
			{
				var batch = session
					.CreateQueryBatch()

					.Add<int>(
						session
							.QueryOver<EntityComplex>()
							.Where(x => x.Version >= 0)
							.TransformUsing(new ListTransformerToInt()))

					.Add("queryOver", session.QueryOver<EntityComplex>().Where(x => x.Version >= 1))

					.Add(session.Query<EntityComplex>().Where(ec => ec.Version > 2))

					.Add<EntitySimpleChild>("sql",
						session.CreateSQLQuery(
									$"select * from {nameof(EntitySimpleChild)}")
								.AddEntity(typeof(EntitySimpleChild)));

				using (var sqlLog = new SqlLogSpy())
				{
					batch.GetResult<int>(0);
					batch.GetResult<EntityComplex>("queryOver");
					batch.GetResult<EntityComplex>(2);
					batch.GetResult<EntitySimpleChild>("sql");
					if (SupportsMultipleQueries)
						Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1));
				}
			}
		}

		[Test]
		public void CanCombineCriteriaAndHqlInBatchAsFuture()
		{
			using (var session = OpenSession())
			{
				var batch = session
					.CreateQueryBatch();

				var future1 = batch.AddAsFuture<int>(
					session
						.QueryOver<EntityComplex>()
						.Where(x => x.Version >= 0)
						.TransformUsing(new ListTransformerToInt()));

				var future2 = batch.AddAsFutureValue<Guid>(session.QueryOver<EntityComplex>().Where(x => x.Version >= 1).Select(x => x.Id));

				var future3 = batch.AddAsFuture(session.Query<EntityComplex>().Where(ec => ec.Version > 2));
				var future4 = batch.AddAsFutureValue(session.Query<EntityComplex>().Where(ec => ec.Version > 2), ec => ec.FirstOrDefault());

				var future5 = batch.AddAsFuture<EntitySimpleChild>(
					session.CreateSQLQuery(
								$"select * from {nameof(EntitySimpleChild)}")
							.AddEntity(typeof(EntitySimpleChild)));

				using (var sqlLog = new SqlLogSpy())
				{
					var future1List = future1.GetEnumerable().ToList();
					var future2Value = future2.Value;
					var future3List = future3.GetEnumerable().ToList();
					var future4Value = future4.Value;
					var future5List = future5.GetEnumerable().ToList();

					if (SupportsMultipleQueries)
						Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1));
				}
			}
		}

		[Test]
		public void CanFetchCollectionInBatch()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var batch = session.CreateQueryBatch();

				var q1 = session.QueryOver<EntityComplex>()
								.Where(x => x.Version >= 0);

				batch.Add(q1);
				batch.Add(session.Query<EntityComplex>().Fetch(c => c.ChildrenList));
				batch.Execute();

				var parent = session.Load<EntityComplex>(_parentId);
				Assert.That(NHibernateUtil.IsInitialized(parent), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(parent.ChildrenList), Is.True);
				if (SupportsMultipleQueries)
					Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1));
			}
		}

		[Test]
		public void AfterLoadCallback()
		{
			using (var session = OpenSession())
			{
				var batch = session.CreateQueryBatch();
				IList<EntityComplex> results = null;
				int count = 0;
				batch.Add(session.Query<EntityComplex>().WithOptions(o => o.SetCacheable(true)), r => results = r);
				batch.Add(session.Query<EntityComplex>().WithOptions(o => o.SetCacheable(true)), ec => ec.Count(), r => count = r);
				batch.Execute();

				Assert.That(results, Is.Not.Null);
				Assert.That(count, Is.GreaterThan(0));
			}

			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var batch = session.CreateQueryBatch();
				IList<EntityComplex> results = null;
				int count = 0;
				batch.Add(session.Query<EntityComplex>().WithOptions(o => o.SetCacheable(true)), r => results = r);
				batch.Add(session.Query<EntityComplex>().WithOptions(o => o.SetCacheable(true)), ec => ec.Count(), r => count = r);

				batch.Execute();

				Assert.That(results, Is.Not.Null);
				Assert.That(count, Is.GreaterThan(0));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(0), "Query is expected to be retrieved from cache");
			}
		}

		//NH-3350 (Duplicate records using Future())
		[Test]
		public void SameCollectionFetches()
		{
			using (var session = OpenSession())
			{
				var entiyComplex = session.QueryOver<EntityComplex>().Where(c => c.Id == _parentId).FutureValue();

				session.QueryOver<EntityComplex>()
						.Fetch(SelectMode.Fetch, ec => ec.ChildrenList)
						.Where(c => c.Id == _parentId).Future();

				session.QueryOver<EntityComplex>()
						.Fetch(SelectMode.Fetch, ec => ec.ChildrenList)
						.Where(c => c.Id == _parentId).Future();

				var parent = entiyComplex.Value;
				Assert.That(NHibernateUtil.IsInitialized(parent), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(parent.ChildrenList), Is.True);
				Assert.That(parent.ChildrenList.Count, Is.EqualTo(2));
			}
		}

		//NH-3864 - Cacheable Multicriteria/Future'd query with aliased join throw exception 
		[Test]
		public void CacheableCriteriaWithAliasedJoinFuture()
		{
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				var ecFuture = session.QueryOver<EntityComplex>()
									.JoinAlias(c => c.Child1, () => child1)
									.Where(c => c.Id == _parentId)
									.Cacheable()
									.FutureValue();
				EntityComplex value = null;
				Assert.DoesNotThrow(() => value = ecFuture.Value);
				Assert.That(value, Is.Not.Null);
			}

			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				var ecFuture = session.QueryOver<EntityComplex>()
									.JoinAlias(c => c.Child1, () => child1)
									.Where(c => c.Id == _parentId)
									.Cacheable()
									.FutureValue();
				EntityComplex value = null;
				Assert.DoesNotThrow(() => value = ecFuture.Value);
				Assert.That(value, Is.Not.Null);

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(0), "Query is expected to be retrieved from cache");
			}
		}

		//NH-3334 - 'collection is not associated with any session' upon refreshing objects from QueryOver<>().Future<>()
		[KnownBug("NH-3334")]
		[Test]
		public void RefreshFutureWithEagerCollections()
		{
			using (var session = OpenSession())
			{
				var ecFutureList = session.QueryOver<EntityEager>().Future();

				foreach(var ec in ecFutureList.GetEnumerable())
				{
					//trouble causes ec.ChildrenListEager with eager select mapping
					Assert.DoesNotThrow(() => session.Refresh(ec), "session.Refresh should not throw exception");
				}
			}
		}

		//Related to NH-3334. Eager mappings are not fetched by Future
		[KnownBug("NH-3334")]
		[Test]
		public void FutureForEagerMappedCollection()
		{
			//Note: This behavior might be considered as feature but it's not documented.
			//Quirk: if this query is also cached - results will be still eager loaded when values retrieved from cache
			using (var session = OpenSession())
			{
				var futureValue = session.QueryOver<EntityEager>().Where(e => e.Id == _eagerId).FutureValue();

				Assert.That(futureValue.Value, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(futureValue.Value), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(futureValue.Value.ChildrenListEager), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(futureValue.Value.ChildrenListSubselect), Is.True);
			}
		}

		[Test]
		public void AutoDiscoverWorksWithFuture()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var future =
					s
						.CreateSQLQuery("select count(*) as childCount from EntitySimpleChild where Name like :pattern")
						.AddScalar("childCount", NHibernateUtil.Int64)
						.SetString("pattern", "Chi%")
						.SetCacheable(true)
						.FutureValue<long>();

				Assert.That(future.Value, Is.EqualTo(2L), "From DB");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var future =
					s
						.CreateSQLQuery("select count(*) as childCount from EntitySimpleChild where Name like :pattern")
						.AddScalar("childCount", NHibernateUtil.Int64)
						.SetString("pattern", "Chi%")
						.SetCacheable(true)
						.FutureValue<long>();

				Assert.That(future.Value, Is.EqualTo(2L), "From cache");
				t.Commit();
			}
		}

		[Test]
		public void AutoFlushCacheInvalidationWorksWithFuture()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var futureResults =
					s
						.CreateQuery("from EntitySimpleChild")
						.SetCacheable(true)
						.Future<EntitySimpleChild>()
						.GetEnumerable()
						.ToList();

				Assert.That(futureResults, Has.Count.EqualTo(2), "First call");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var deleted = s.Query<EntitySimpleChild>().First();
				// We need to get rid of a referencing entity for the delete.
				deleted.Parent.Child1 = null;
				deleted.Parent.Child2 = null;
				s.Delete(deleted);

				var future =
					s
						.CreateQuery("from EntitySimpleChild")
						.SetCacheable(true)
						.Future<EntitySimpleChild>();

				Assert.That(future.GetEnumerable().ToList(), Has.Count.EqualTo(1), "After delete");
				t.Commit();
			}
		}

		[Test]
		public void UsingHqlToFutureWithCacheAndTransformerDoesntThrow()
		{
			// Adapted from #383
			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				//store values in cache
				session
					.CreateQuery("from EntitySimpleChild")
					.SetResultTransformer(Transformers.DistinctRootEntity)
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Normal)
					.Future<EntitySimpleChild>()
					.GetEnumerable();
				t.Commit();
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				//get values from cache
				var results =
					session
						.CreateQuery("from EntitySimpleChild")
						.SetResultTransformer(Transformers.DistinctRootEntity)
						.SetCacheable(true)
						.SetCacheMode(CacheMode.Normal)
						.Future<EntitySimpleChild>()
						.GetEnumerable()
						.ToList();

				Assert.That(results.Count, Is.EqualTo(2));
				t.Commit();
			}
		}

		[Test]
		public void ReadOnlyWorksWithFuture()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var futureSimples =
					s
						.CreateQuery("from EntitySimpleChild")
						.SetReadOnly(true)
						.Future<EntitySimpleChild>();
				var futureSubselect =
					s
						.CreateQuery("from EntitySubselectChild")
						.Future<EntitySubselectChild>();

				var simples = futureSimples.GetEnumerable().ToList();
				Assert.That(simples, Has.Count.GreaterThan(0));
				foreach (var entity in simples)
				{
					Assert.That(s.IsReadOnly(entity), Is.True, entity.Name);
				}

				var subselect = futureSubselect.GetEnumerable().ToList();
				Assert.That(subselect, Has.Count.GreaterThan(0));
				foreach (var entity in subselect)
				{
					Assert.That(s.IsReadOnly(entity), Is.False, entity.Name);
				}

				t.Commit();
			}
		}

		[Test]
		public void CacheModeWorksWithFuture()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s
					.CreateQuery("from EntitySimpleChild")
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Get)
					.Future<EntitySimpleChild>();
				s
					.CreateQuery("from EntityComplex")
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Put)
					.Future<EntityComplex>();
				s
					.CreateQuery("from EntitySubselectChild")
					.SetCacheable(true)
					.Future<EntitySubselectChild>()
					.GetEnumerable();
				Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(2), "Future put");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Sfi.Statistics.Clear();
				s
					.CreateQuery("from EntitySimpleChild")
					.SetCacheable(true)
					.List<EntitySimpleChild>();
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "EntitySimpleChild query hit");

				Sfi.Statistics.Clear();
				s
					.CreateQuery("from EntityComplex")
					.SetCacheable(true)
					.List<EntityComplex>();
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "EntityComplex query hit");

				Sfi.Statistics.Clear();
				s
					.CreateQuery("from EntitySubselectChild")
					.SetCacheable(true)
					.List<EntitySubselectChild>();
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "EntitySubselectChild query hit");

				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				Sfi.Statistics.Clear();
				s
					.CreateQuery("from EntitySimpleChild")
					.SetCacheable(true)
					.SetCacheMode(CacheMode.Get)
					.Future<EntitySimpleChild>();
				s
					.CreateQuery("from EntitySubselectChild")
					.SetCacheable(true)
					.Future<EntitySubselectChild>()
					.GetEnumerable();
				Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(0), "Second future put");
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(2), "Second future hit");

				t.Commit();
			}
		}

		//GH-2173
		[Test]
		public void CanFetchNonLazyEntitiesInSubsequentQuery()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(
					new EntityEager
					{
						Name = "EagerManyToOneAssociation",
						EagerEntity = new EntityEagerChild {Name = "association"}
					});
				t.Commit();
			}

			using (var s = OpenSession())
			{
				Sfi.Statistics.Clear();
				//EntityEager.EagerEntity is lazy initialized instead of being loaded by the second query 
				s.QueryOver<EntityEager>().Fetch(SelectMode.Skip, x => x.EagerEntity).Future();
				s.QueryOver<EntityEager>().Fetch(SelectMode.Fetch, x => x.EagerEntity).Future().GetEnumerable();

				if(SupportsMultipleQueries)
					Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			}
		}

		#region Test Setup

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityComplex>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));

					rc.Version(ep => ep.Version, vm => { });

					rc.Property(x => x.Name);

					rc.Property(ep => ep.LazyProp, m => m.Lazy(true));

					rc.ManyToOne(ep => ep.Child1, m => m.Column("Child1Id"));
					rc.ManyToOne(ep => ep.Child2, m => m.Column("Child2Id"));
					rc.ManyToOne(ep => ep.SameTypeChild, m => m.Column("SameTypeChildId"));

					rc.Bag(
						ep => ep.ChildrenList,
						m =>
						{
							m.Cascade(Mapping.ByCode.Cascade.All);
							m.Inverse(true);
						},
						a => a.OneToMany());
				});

			mapper.Class<EntitySimpleChild>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.ManyToOne(x => x.Parent);
					rc.Property(x => x.Name);
				});
			mapper.Class<EntityEager>(
				rc =>
				{
					rc.Lazy(false);

					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);

					rc.ManyToOne(x => x.EagerEntity, m =>
					{
						m.Cascade(Mapping.ByCode.Cascade.Persist);
					});
					rc.Bag(ep => ep.ChildrenListSubselect,
							m =>
							{
								m.Cascade(Mapping.ByCode.Cascade.All);
								m.Inverse(true);
								m.Fetch(CollectionFetchMode.Subselect);
								m.Lazy(CollectionLazy.NoLazy);
							},
							a => a.OneToMany());

					rc.Bag(ep => ep.ChildrenListEager,
							m =>
							{
								m.Lazy(CollectionLazy.NoLazy);
							},
							a => a.OneToMany());
				});
			mapper.Class<EntityEagerChild>(
				rc =>
				{
					rc.Lazy(false);

					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
				});
			mapper.Class<EntitySubselectChild>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.ManyToOne(c => c.Parent);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
			Sfi.Statistics.IsStatisticsEnabled = false;
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var child1 = new EntitySimpleChild
				{
					Name = "Child1",
				};
				var child2 = new EntitySimpleChild
				{
					Name = "Child2"
				};
				var complex = new EntityComplex
				{
					Name = "ComplexEnityParent",
					Child1 = child1,
					Child2 = child2,
					LazyProp = "SomeBigValue",
					SameTypeChild = new EntityComplex()
					{
						Name = "ComplexEntityChild"
					},
				};
				child1.Parent = child2.Parent = complex;

				var eager = new EntityEager()
				{
					Name = "eager1",
				};

				var eager2 = new EntityEager()
				{
					Name = "eager2",
				};
				eager.ChildrenListSubselect = new List<EntitySubselectChild>()
					{
						new EntitySubselectChild()
						{
							Name = "subselect1",
							Parent = eager,
						},
						new EntitySubselectChild()
						{
							Name = "subselect2",
							Parent = eager,
						},
					};

				session.Save(child1);
				session.Save(child2);
				session.Save(complex.SameTypeChild);
				session.Save(complex);
				session.Save(eager);
				session.Save(eager2);

				session.Flush();
				transaction.Commit();

				_parentId = complex.Id;
				_eagerId = eager.Id;
			}
		}

		public class ListTransformerToInt : IResultTransformer
		{
			public object TransformTuple(object[] tuple, string[] aliases)
			{
				return tuple.Length == 1 ? tuple[0] : tuple;
			}

			public IList TransformList(IList collection)
			{
				return new List<int>()
				{
					1,
					2,
					3,
					4,
				};
			}
		}

		private bool SupportsMultipleQueries => Sfi.ConnectionProvider.Driver.SupportsMultipleQueries;

		#endregion Test Setup
	}
}
