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
