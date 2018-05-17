using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Loader;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.SelectModeTest
{
	/// <summary>
	/// Tests for select mode
	/// </summary>
	[TestFixture]
	public class SelectModeTest : TestCaseMappingByCode
	{
		private Guid _parentEntityComplexId;

		[Test]
		public void SelectModeSkip()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				root = session.QueryOver(() => root)
							//Child1 is required solely for filtering, no need to be fetched, so skip it from select statement
							.JoinQueryOver(r => r.Child1, JoinType.InnerJoin)
							.With(SelectMode.Skip, child1 => child1)
							.Take(1)
							.SingleOrDefault();

				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root.Child1), Is.False, "Joined ManyToOne Child1 should not be fetched.");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void SelectModeFetch()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var list = session.QueryOver<EntityComplex>()
								.With(SelectMode.Fetch, ec => ec.Child1)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								//now we can fetch inner joined collection
								.With(SelectMode.Fetch, childrenList => childrenList)
								.TransformUsing(Transformers.DistinctRootEntity)
								.List();

				var root = list.FirstOrDefault();
				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root.Child1), Is.True, "Entity must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(root.ChildrenList), Is.True, "ChildrenList Inner Joined collection must be initialized");

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		//Check that SelecMode survives query cloning
		[Test]
		public void SelectModeFetch_ClonedQueryOver()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var list = session.QueryOver<EntityComplex>()
								.With(SelectMode.Fetch, ec => ec.Child1)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								//now we can fetch inner joined collection
								.With(SelectMode.Fetch, childrenList => childrenList)
								.TransformUsing(Transformers.DistinctRootEntity)
								.Clone()
								.List();

				var root = list.FirstOrDefault();
				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root.Child1), Is.True, "Entity must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(root.ChildrenList), Is.True, "ChildrenList Inner Joined collection must be initialized");

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void SelectModeDefault()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				//SelectMode.Default is no op - fetching is controlled by default behavior:
				//SameTypeChild won't be loaded, and ChildrenList collection won't be fetched due to InnerJoin
				var list = session.QueryOver<EntityComplex>()
								.With(SelectMode.Default, ec => ec.SameTypeChild)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								.With(SelectMode.Default, childrenList => childrenList)
								.TransformUsing(Transformers.DistinctRootEntity)
								.List();

				//So it's a full equivalent of the following query (without SelectMode)
				session.QueryOver<EntityComplex>()
						.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
						.TransformUsing(Transformers.DistinctRootEntity);
						
				var root = list.FirstOrDefault();
				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.SameTypeChild, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root.SameTypeChild), Is.False, "Entity must be NOT initialized");
				Assert.That(NHibernateUtil.IsInitialized(root.ChildrenList), Is.False, "ChildrenList Inner Joined collection must be NOT initialized");

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void SelectModeFetchLazyProperties()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var root = session.QueryOver<EntityComplex>()
								.With(SelectMode.FetchLazyProperties, ec => ec)
								.Where(ec => ec.LazyProp != null)
								.Take(1)
								.SingleOrDefault();

				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.LazyProp, Is.Not.Null);
				Assert.That(NHibernateUtil.IsPropertyInitialized(root, nameof(root.LazyProp)), Is.True, "Lazy property must be fetched.");

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}


		[Test]
		public void SelectModeChildFetchForMultipleCollections_SingleDbRoundtrip()
		{
			SkipFutureTestIfNotSupported();

			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				var futureRoot = session
						.QueryOver(() => root)
						.Where(r => r.Id == _parentEntityComplexId)
						.FutureValue();

				session
					.QueryOver(() => root)
					//Only ID is added to SELECT statement for root so it's index scan only
					.With(SelectMode.ChildFetch, ec => ec)
					.With(SelectMode.Fetch, ec => ec.ChildrenList)
					.Where(r => r.Id == _parentEntityComplexId)
					.Future();
				
				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, ec => ec)
					.With(SelectMode.Fetch, ec => ec.ChildrenListEmpty)
					.Where(r => r.Id == _parentEntityComplexId)
					.Future();

				root = futureRoot.Value;

				Assert.That(root?.ChildrenList, Is.Not.Null);
				Assert.That(root?.ChildrenListEmpty, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList), "ChildrenList must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenListEmpty), "ChildrenListEmpty must be initialized");
				Assert.That(root?.ChildrenList, Is.Not.Empty, "ChildrenList must not be empty");
				Assert.That(root?.ChildrenListEmpty, Is.Empty, "ChildrenListEmpty must be empty");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void SelectModeChildFetchForMultipleCollections()
		{
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				root  = session
						.QueryOver(() => root)
						.Where(r => r.Id == _parentEntityComplexId)
						.SingleOrDefault();

				session
					.QueryOver(() => root)
					//Only ID is added to SELECT statement for root so it's index scan only
					.With(SelectMode.ChildFetch, ec => ec)
					.With(SelectMode.Fetch, ec => ec.ChildrenList)
					.Where(r => r.Id == _parentEntityComplexId)
					.List();
				
				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, ec => ec)
					.With(SelectMode.Fetch, ec => ec.ChildrenListEmpty)
					.Where(r => r.Id == _parentEntityComplexId)
					.List();

				Assert.That(root?.ChildrenList, Is.Not.Null);
				Assert.That(root?.ChildrenListEmpty, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList), "ChildrenList must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenListEmpty), "ChildrenListEmpty must be initialized");
				Assert.That(root?.ChildrenList, Is.Not.Empty, "ChildrenList must not be empty");
				Assert.That(root?.ChildrenListEmpty, Is.Empty, "ChildrenListEmpty must be empty");
			}
		}

		[Test]
		public void SelectModeSkipEntityJoin()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex parentJoin = null;
				EntitySimpleChild rootChild = null;
				rootChild = session.QueryOver(() => rootChild)
							.JoinEntityQueryOver(() => parentJoin, Restrictions.Where(() => rootChild.ParentId == parentJoin.Id))
							.With(SelectMode.Skip, a => a)
							.Take(1)
							.SingleOrDefault();

				parentJoin = session.Load<EntityComplex>(rootChild.ParentId);

				Assert.That(rootChild, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(rootChild), Is.True);
				Assert.That(rootChild.ParentId, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(parentJoin), Is.False, "Entity Join must not be initialized.");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}


		[Test]
		public void SelectModeFetchLazyPropertiesForEntityJoin()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex parentJoin = null;
				EntitySimpleChild rootChild = null;
				rootChild = session.QueryOver(() => rootChild)
							.JoinEntityQueryOver(() => parentJoin, Restrictions.Where(() => rootChild.ParentId == parentJoin.Id))
							.With(SelectMode.FetchLazyProperties, ec => ec)
							.Take(1)
							.SingleOrDefault();
				parentJoin = session.Load<EntityComplex>(rootChild.ParentId);

				Assert.That(rootChild, Is.Not.Null);

				Assert.That(NHibernateUtil.IsInitialized(rootChild), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(parentJoin), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(parentJoin, nameof(parentJoin.LazyProp)), Is.Not.Null.Or.Empty);
				Assert.That(parentJoin.LazyProp, Is.Not.Null.Or.Empty);

				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}


		[Test]
		public void SelectModeChildFetchLoadsNotLoadedObject()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				root = session.QueryOver(() => root)
							.With(SelectMode.ChildFetch, r => r)
							.JoinQueryOver(ec => ec.ChildrenList)
							.With(SelectMode.Fetch, simpleChild => simpleChild)
							.Take(1)
							.SingleOrDefault();

				Assert.That(root, Is.Not.Null, "root is not loaded");
				Assert.That(NHibernateUtil.IsInitialized(root), Is.False, "root should not be initialized");
				Assert.That(sqlLog.Appender.GetEvents(), Has.Length.EqualTo(1), "Only one SQL select is expected");
				// The root was not initialized but its children collection is immediately initialized... A bit weird feature.
				Assert.That(NHibernateUtil.IsInitialized(root.ChildrenList), Is.True, "root children should be initialized");
				Assert.That(root.ChildrenList, Has.Count.EqualTo(1).And.None.Null, "Unexpected children collection content");
			}
		}

		[Test]
		public void SelectModeChildFetchDeep_SingleDbRoundtrip_Aliased()
		{
			SkipFutureTestIfNotSupported();

			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				var rootFuture = session
								.QueryOver(() => root)
								.Future();

				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, () => root)
					.With(SelectMode.Fetch, () => root.ChildrenList)
					.Future();

				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, () => root, () => root.ChildrenList)
					.With(SelectMode.Fetch, () => root.ChildrenList[0].Children)
					.Future();

				session
					.QueryOver(() => root)
					.With(SelectMode.Skip, () => root.ChildrenList)
					.With(SelectMode.ChildFetch,() => root, () => root.ChildrenList[0].Children)
					.With(SelectMode.Fetch, () => root.ChildrenList[0].Children[0].Children)
					.Future();

				root = rootFuture.ToList().First(r => r.Id == _parentEntityComplexId);


				Assert.That(root?.ChildrenList, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children[0].Children));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void SelectModeChildFetchDeep_Aliased()
		{
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				var list = session
							.QueryOver(() => root)
							.List();

				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, () => root)
					.With(SelectMode.Fetch, () => root.ChildrenList)
					.List();

				session
					.QueryOver(() => root)
					.With(SelectMode.ChildFetch, () => root, () => root.ChildrenList)
					.With(SelectMode.Fetch, () => root.ChildrenList[0].Children)
					.List();

				session
					.QueryOver(() => root)
					.With(SelectMode.Skip, () => root.ChildrenList)
					.With(SelectMode.ChildFetch,() => root, () => root.ChildrenList[0].Children)
					.With(SelectMode.Fetch, () => root.ChildrenList[0].Children[0].Children)
					.List();

				root = list.First(r => r.Id == _parentEntityComplexId);


				Assert.That(root?.ChildrenList, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children[0].Children));
			}
		}

		[Test]
		public void SkipRootEntityIsNotSupported()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var query = session.QueryOver<EntityComplex>()
						.With(SelectMode.Skip, ec => ec)
						.With(SelectMode.Fetch, ec => ec.Child1)
						.Take(1);

				Assert.Throws<NotSupportedException>(() => query.SingleOrDefault());
			}
		}

		private void SkipFutureTestIfNotSupported()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (driver.SupportsMultipleQueries == false)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
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

					rc.ManyToOne(
						ep => ep.Child1,
						m =>
						{
							m.Column("Child1Id");
							m.ForeignKey("none");
						});
					rc.ManyToOne(
						ep => ep.Child2,
						m =>
						{
							m.Column("Child2Id");
							m.ForeignKey("none");
						});
					rc.ManyToOne(ep => ep.SameTypeChild, m =>
					{
						m.Column("SameTypeChildId");
						m.ForeignKey("none");
					});
					MapList(rc, ep => ep.ChildrenList);
					MapList(rc, ep => ep.ChildrenListEmpty);
				});

			MapSimpleChild(
				mapper,
				default(EntitySimpleChild),
				c => c.Children,
				rc => { rc.Property(sc => sc.LazyProp, mp => mp.Lazy(true)); });
			MapSimpleChild(mapper, default(Level2Child), c => c.Children);
			MapSimpleChild<Level3Child>(mapper);

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private static void MapSimpleChild<TChild>(ModelMapper mapper) where TChild : BaseChild
		{
			MapSimpleChild<TChild, object>(mapper, default(TChild), null);
		}

		private static void MapSimpleChild<TChild, TSubChild>(ModelMapper mapper, TChild obj, Expression<Func<TChild, IEnumerable<TSubChild>>> expression, Action<IClassMapper<TChild>> action = null) where TChild : BaseChild
		{
			mapper.Class<TChild>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.Property(c => c.ParentId);
					if (expression != null)
					{
						MapList(rc, expression);
					}
				});
		}

		private static void MapList<TParent, TElement>(IClassMapper<TParent> rc, Expression<Func<TParent, IEnumerable<TElement>>> expression) where TParent : class
		{
			rc.Bag(
				expression,
				m =>
				{
					m.Key(
						km =>
						{
							km.Column(
								ckm =>
								{
									ckm.Name("ParentId");
								});
							km.ForeignKey("none");

						});
					m.Cascade(Mapping.ByCode.Cascade.All);
				},
				a => a.OneToMany());
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction(IsolationLevel.Serializable))
			{
				session.Query<Level3Child>().Delete();
				session.Query<Level2Child>().Delete();
				session.Query<EntityComplex>().Delete();
				session.Query<EntitySimpleChild>().Delete();


				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var child1 = new EntitySimpleChild
				{
					Name = "Child1",
					LazyProp = "LazyFromSimpleChild1",
					Children = new List<Level2Child>
					{
						new Level2Child()
						{
							Name = "Level2.1",
							Children = new List<Level3Child>
							{
								new Level3Child
								{
									Name = "Level3.1.1",
								},
								new Level3Child
								{
									Name = "Level3.1.2"
								},
							}
						},
						new Level2Child
						{
							Name = "Level2.2",
							Children = new List<Level3Child>
							{
								new Level3Child
								{
									Name = "Level3.2.1"
								},
								new Level3Child
								{
									Name = "Level3.2.2"
								},
							}
						}
					}

				};
				var child2 = new EntitySimpleChild
				{
					Name = "Child2",
					LazyProp = "LazyFromSimpleChild2",
				};

				var parent = new EntityComplex
				{
					Name = "ComplexEntityParent",
					Child1 = child1,
					Child2 = child2,
					LazyProp = "SomeBigValue",
					SameTypeChild = new EntityComplex()
					{
						Name = "ComplexEntityChild"
					},
					ChildrenList = new List<EntitySimpleChild> {child1},
					ChildrenListEmpty = new List<EntityComplex> { },
				};

				session.Save(child1);
				session.Save(child2);
				session.Save(parent.SameTypeChild);
				session.Save(parent);


				session.Flush();
				transaction.Commit();
				_parentEntityComplexId = parent.Id;
			}
		}

		#endregion Test Setup
	}
}
