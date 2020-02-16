using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Linq;
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
		public void SelectModeJoinOnly()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				root = session.QueryOver(() => root)
							//Child1 is required solely for filtering, no need to be fetched, so skip it from select statement
							.JoinQueryOver(r => r.Child1, JoinType.InnerJoin)
							.Fetch(SelectMode.JoinOnly, child1 => child1)
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
		public void SelectModeDetachedQueryOver()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				root = QueryOver.Of(() => root)
								.Where(x => x.Id == _parentEntityComplexId)
								.Fetch(SelectMode.Fetch, r => r.Child1)
								.GetExecutableQueryOver(session)
								.SingleOrDefault();

				Assert.That(root, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(root.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root.Child1), Is.True, "Joined ManyToOne Child1 should not be fetched.");
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
								.Fetch(SelectMode.Fetch, ec => ec.Child1)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								//now we can fetch inner joined collection
								.Fetch(SelectMode.Fetch, childrenList => childrenList)
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
								.Fetch(SelectMode.Fetch, ec => ec.Child1)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								//now we can fetch inner joined collection
								.Fetch(SelectMode.Fetch, childrenList => childrenList)
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
		public void SelectModeUndefined()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				//SelectMode.Default is no op - fetching is controlled by default behavior:
				//SameTypeChild won't be loaded, and ChildrenList collection won't be fetched due to InnerJoin
				var list = session.QueryOver<EntityComplex>()
								.Fetch(SelectMode.Undefined, ec => ec.SameTypeChild)
								.JoinQueryOver(ec => ec.ChildrenList, JoinType.InnerJoin)
								.Fetch(SelectMode.Undefined, childrenList => childrenList)
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
								.Fetch(SelectMode.FetchLazyProperties, ec => ec)
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
					.Fetch(SelectMode.ChildFetch, ec => ec)
					.Fetch(SelectMode.Fetch, ec => ec.ChildrenList)
					.Where(r => r.Id == _parentEntityComplexId)
					.Future();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.ChildFetch, ec => ec)
					.Fetch(SelectMode.Fetch, ec => ec.ChildrenListEmpty)
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
				root = session
						.QueryOver(() => root)
						.Where(r => r.Id == _parentEntityComplexId)
						.SingleOrDefault();

				session
					.QueryOver(() => root)
					//Only ID is added to SELECT statement for root so it's index scan only
					.Fetch(SelectMode.ChildFetch, ec => ec)
					.Fetch(SelectMode.Fetch, ec => ec.ChildrenList)
					.Where(r => r.Id == _parentEntityComplexId)
					.List();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.ChildFetch, ec => ec)
					.Fetch(SelectMode.Fetch, ec => ec.ChildrenListEmpty)
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
		public void SelectModeJoinOnlyEntityJoin()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex parentJoin = null;
				EntitySimpleChild rootChild = null;
				rootChild = session.QueryOver(() => rootChild)
							.JoinEntityQueryOver(() => parentJoin, Restrictions.Where(() => rootChild.ParentId == parentJoin.Id))
							.Fetch(SelectMode.JoinOnly, a => a)
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
							.Fetch(SelectMode.FetchLazyProperties, ec => ec)
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
							.Fetch(SelectMode.ChildFetch, r => r)
							.JoinQueryOver(ec => ec.ChildrenList)
							.Fetch(SelectMode.Fetch, simpleChild => simpleChild)
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
		public void SelectModeChildFetchLoadsNotLoaded_NotProxifiedObject()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityEager root = null;
				root = session.QueryOver(() => root)
							.Fetch(SelectMode.ChildFetch, r => r)
							.JoinQueryOver(ec => ec.ChildrenList)
							.Fetch(SelectMode.Fetch, simpleChild => simpleChild)
							.Take(1)
							.SingleOrDefault();

				Assert.That(root, Is.Not.Null, "root is loaded");
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True, "root should be initialized");
				Assert.That(sqlLog.Appender.GetEvents(), Has.Length.EqualTo(2), "Two SQL selects are expected (query + loading not proxified entity)");
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
					.Fetch(SelectMode.ChildFetch, () => root)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList)
					.Future();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.ChildFetch, () => root, () => root.ChildrenList)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList[0].Children)
					.Future();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.JoinOnly, () => root.ChildrenList)
					.Fetch(SelectMode.ChildFetch,() => root, () => root.ChildrenList[0].Children)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList[0].Children[0].Children)
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
					.Fetch(SelectMode.ChildFetch, () => root)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList)
					.List();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.ChildFetch, () => root, () => root.ChildrenList)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList[0].Children)
					.List();

				session
					.QueryOver(() => root)
					.Fetch(SelectMode.JoinOnly, () => root.ChildrenList)
					.Fetch(SelectMode.ChildFetch,() => root, () => root.ChildrenList[0].Children)
					.Fetch(SelectMode.Fetch, () => root.ChildrenList[0].Children[0].Children)
					.List();

				root = list.First(r => r.Id == _parentEntityComplexId);

				Assert.That(root?.ChildrenList, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children));
				Assert.That(NHibernateUtil.IsInitialized(root?.ChildrenList[0].Children[0].Children));
			}
		}

		[Test]
		public void JoinOnlyRootEntityIsNotSupported()
		{
			using (var session = OpenSession())
			{
				var query = session.QueryOver<EntityComplex>()
						.Fetch(SelectMode.JoinOnly, ec => ec)
						.Fetch(SelectMode.Fetch, ec => ec.Child1)
						.Take(1);

				Assert.Throws<NotSupportedException>(() => query.SingleOrDefault());
			}
		}

		[Test]
		public void SkipRootEntityIsNotSupported()
		{
			using (var session = OpenSession())
			{
				var query = session.QueryOver<EntityComplex>()
						.Fetch(SelectMode.Skip, ec => ec)
						.Fetch(SelectMode.Fetch, ec => ec.Child1)
						.Take(1);

				Assert.Throws<NotSupportedException>(() => query.SingleOrDefault());
			}
		}

		[Test]
		public void OrderedInnerJoinFetch()
		{
			using (var session = OpenSession())
			{
				var list = session.QueryOver<EntityComplex>()
					.Where(ec => ec.Id == _parentEntityComplexId)
					.JoinQueryOver(c => c.ChildrenList).Fetch(SelectMode.Fetch, child => child)
					.TransformUsing(Transformers.DistinctRootEntity)
					.List();

				var childList = list[0].ChildrenList;
				Assert.That(list[0].ChildrenList.Count, Is.GreaterThan(1));
				Assert.That(list[0].ChildrenList, Is.EqualTo(list[0].ChildrenList.OrderByDescending(c => c.OrderIdx)), "wrong order");
			}
		}

		[Test, Obsolete]
		public void FetchModeEagerForLazy()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityComplex>()
									.Fetch(ec => ec.Child1).Eager
									.Fetch(ec => ec.ChildrenList).Eager
									.Where(ec => ec.Child1 != null)
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(parent?.Child1), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.True);
			}
		}

		[Test, Obsolete]
		public void FetchModeLazyForLazy()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityComplex>()
									.Fetch(ec => ec.Child1).Lazy
									.Fetch(ec => ec.ChildrenList).Lazy
									.Where(ec => ec.Child1 != null)
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(parent?.Child1), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.False);
			}
		}

		[Test, Obsolete]
		public void FetchModeDefaultForLazy()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityComplex>()
									.Fetch(ec => ec.Child1).Default
									.Fetch(ec => ec.ChildrenList).Default
									.Where(ec => ec.Child1 != null)
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.Child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(parent?.Child1), Is.False);
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.False);
			}
		}

		[Test, Obsolete]
		public void FetchModeLazyForEager()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityEager>().List().FirstOrDefault();
				Assert.That(parent?.ChildrenList, Is.Not.Null, "Failed test set up. Object must not be null.");
				Assert.That(parent?.ChildrenList, Is.Not.Null, "Failed test set up. ChildrenList must be eager loaded with parent.");
			}

			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityEager>()
									.Fetch(ec => ec.ChildrenList).Lazy
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.ChildrenList, Is.Not.Null, "collection should not be null");
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.False, "Eager collection should not be initialized.");
			}
		}

		[Test, Obsolete]
		public void FetchModeDefaultForEager()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityEager>()
									.Fetch(ec => ec.ChildrenList).Default
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.ChildrenList, Is.Not.Null, "collection should not be null");
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.True, "eager collection should be initialized");
			}
		}

		[Test, Obsolete]
		public void FetchModeEagerForEager()
		{
			using (var session = OpenSession())
			{
				var parent = session.QueryOver<EntityEager>()
									.Fetch(ec => ec.ChildrenList).Eager
									.TransformUsing(Transformers.DistinctRootEntity)
									.List()
									.FirstOrDefault();

				Assert.That(parent?.ChildrenList, Is.Not.Null, "collection should not be null");
				Assert.That(NHibernateUtil.IsInitialized(parent?.ChildrenList), Is.True, "eager collection should be initialized");
			}
		}

		private void SkipFutureTestIfNotSupported()
		{
			if (Sfi.ConnectionProvider.Driver.SupportsMultipleQueries == false)
				Assert.Ignore("Driver {0} does not support multi-queries", Sfi.ConnectionProvider.Driver.GetType().FullName);
		}

		#region Test Setup

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityEager>(
				rc =>
				{
					rc.Lazy(false);
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Version(ep => ep.Version, vm => { });
					rc.Property(x => x.Name);
					MapList(rc, p => p.ChildrenList, CollectionFetchMode.Join);
				});

			MapSimpleChild<EntityEagerChild>(
				mapper,
				rc => { rc.Lazy(false); });

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
					MapList(rc, ep => ep.ChildrenList, mapper: m => m.OrderBy("OrderIdx desc"));
					MapList(rc, ep => ep.ChildrenListEmpty);
				});

			MapSimpleChild(
				mapper,
				default(EntitySimpleChild),
				c => c.Children,
				rc =>
				{
					rc.Property(sc => sc.LazyProp, mp => mp.Lazy(true));
					rc.Property(sc => sc.OrderIdx);
				});
			MapSimpleChild(mapper, default(Level2Child), c => c.Children);
			MapSimpleChild<Level3Child>(mapper);

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		private static void MapSimpleChild<TChild>(ModelMapper mapper, Action<IClassMapper<TChild>> action = null) where TChild : BaseChild
		{
			MapSimpleChild<TChild, object>(mapper, default(TChild), null, action);
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
					action?.Invoke(rc);
				});
		}

		private static void MapList<TParent, TElement>(IClassMapper<TParent> rc, Expression<Func<TParent, IEnumerable<TElement>>> expression, CollectionFetchMode fetchMode =  null, Action<IBagPropertiesMapper<TParent, TElement>> mapper = null) where TParent : class
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
					if (fetchMode != null)
					{
						m.Fetch(fetchMode);
					}
					mapper?.Invoke(m);
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
				session.Query<EntityEagerChild>().Delete();
				session.Query<EntityEager>().Delete();

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
					},
					OrderIdx = 100
				};

				var child2 = new EntitySimpleChild
				{
					Name = "Child2",
					LazyProp = "LazyFromSimpleChild2",
				};

				var child3 = new EntitySimpleChild
				{
					Name = "Child3",
					OrderIdx = 0
				};
				var child4 = new EntitySimpleChild
				{
					Name = "Child4",
					OrderIdx = 50
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
					ChildrenList = new List<EntitySimpleChild> {child3, child1, child4 },
					ChildrenListEmpty = new List<EntityComplex> { },
				};
				session.Save(new EntityEager()
				{
					Name = "Eager",
					ChildrenList = new List<EntityEagerChild>
					{ new EntityEagerChild(){Name ="EagerChild"}}
				});

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
