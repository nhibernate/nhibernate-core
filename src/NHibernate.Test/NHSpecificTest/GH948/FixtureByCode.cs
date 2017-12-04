﻿using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH948
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private EntityWithCompositeId _entityWithCompositeId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityComplex>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));

				rc.Version(ep => ep.Version, vm => { });

				rc.Property(x => x.Name);

				rc.Property(ep => ep.LazyProp, m => m.Lazy(true));

				rc.ManyToOne(ep => ep.Child1, m => m.Column("Child1Id"));
				rc.ManyToOne(ep => ep.Child2, m => m.Column("Child2Id"));
				rc.ManyToOne(ep => ep.SameTypeChild, m => m.Column("SameTypeChildId"));

				rc.Bag(ep => ep.ChildrenList, m =>
				{
					m.Cascade(Cascade.All);
					m.Inverse(true);
				}, a => a.OneToMany());
				
			});

			mapper.Class<EntitySimpleChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithCompositeId>(
				rc =>
				{
					rc.ComponentAsId(
						e => e.Key,
						ekm =>
						{
							ekm.Property(ek => ek.Id1);
							ekm.Property(ek => ek.Id2);
						});

					rc.Property(e => e.Name);
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
			CreateObjects();
		}

		[Test]
		public void RootEntityProjectionFullyInitializedAndWithUnfetchedLazyPropertiesByDefault()
		{
			using(var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				Sfi.Statistics.Clear();
				EntityComplex entitypRoot = session
					.QueryOver<EntityComplex>()
					.Where(ec => ec.LazyProp != null)
					.Select(Projections.RootEntity())
					.Take(1).SingleOrDefault();
				
				Assert.That(NHibernateUtil.IsInitialized(entitypRoot),Is.True, "Object must be initialized by default");
				Assert.That(session.IsReadOnly(entitypRoot),Is.False, "Object must not be readonly by default");
				Assert.That(NHibernateUtil.IsPropertyInitialized(entitypRoot, nameof(entitypRoot.LazyProp)), Is.False, "Lazy properties should not be initialized by default.");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void RootEntityProjectionLazy()
		{
			using (var session = OpenSession())
			{
				EntityComplex entitypRoot = session
					.QueryOver<EntityComplex>()
					.Select(Projections.RootEntity().SetLazy(true))
					.Take(1).SingleOrDefault();
				
				
				Assert.That(NHibernateUtil.IsInitialized(entitypRoot),Is.False, "Object must be lazy loaded.");
			}
		}

		[Test]
		public void AliasedEntityProjection()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				child1 = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.Select(Projections.Entity(() => child1))
					.Take(1).SingleOrDefault<EntitySimpleChild>();

				Assert.That(child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.True, "Object must be initialized");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void MultipleLazyEntityProjections()
		{
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				EntityComplex root = null;
				EntityComplex sameTypeChild = null;
				EntitySimpleChild child2 = null;

				var result = session
					.QueryOver(() => root)
					.JoinAlias(ep => ep.SameTypeChild, () => sameTypeChild)
					.JoinAlias(ep => ep.Child1, () => child1)
					.JoinAlias(ep => ep.Child2, () => child2)
					.Select(
						Projections.RootEntity().SetLazy(true),
						Projections.Entity(() => child1).SetLazy(true),
						Projections.Entity(() => sameTypeChild).SetLazy(true),
						Projections.Entity(() => child2).SetLazy(true)
					)
					.Take(1).SingleOrDefault<object[]>();

				root = (EntityComplex) result[0];
				child1 = (EntitySimpleChild) result[1];
				sameTypeChild = (EntityComplex) result[2];
				child2 = (EntitySimpleChild) result[3];

				Assert.That(NHibernateUtil.IsInitialized(root), Is.False, "Object must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(sameTypeChild), Is.False, "Object must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.False, "Object must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(child2), Is.False, "Object must be lazy loaded.");
				
				//make sure objects are populated from different aliases for the same types
				Assert.That(root.Id , Is.Not.EqualTo(sameTypeChild.Id), "Different objects are expected.");
				Assert.That(child1.Id , Is.Not.EqualTo(child2.Id), "Different objects are expected.");

			}
		}

		[Test]
		public void EntityProjectionWithLazyPropertiesFetched()
		{
			using (var session = OpenSession())
			{
				EntityComplex entitypRoot = session
					.QueryOver<EntityComplex>()
					.Where(ec => ec.LazyProp != null)
					.Select(Projections.RootEntity().SetAllPropertyFetch(true))
					.Take(1).SingleOrDefault();

				Assert.That(entitypRoot, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entitypRoot), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsPropertyInitialized(entitypRoot, nameof(entitypRoot.LazyProp)), Is.True, "Lazy property must be initialized");
			}
		}

		[Test]
		public void NullEntityProjection()
		{
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				child1 = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1, JoinType.LeftOuterJoin)
					.Where(() => child1.Id == null)
					.Select(Projections.Entity(() => child1))
					.Take(1).SingleOrDefault<EntitySimpleChild>();

				Assert.That(child1, Is.Null);
			}
		}

		[Test]
		public void MultipleEntitiesProjections()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				EntitySimpleChild child1 = null;
				EntitySimpleChild child2 = null;
				EntityComplex sameAsRootChild = null;
				EntitySimpleChild nullListElem = null;
				var objects = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.JoinAlias(ep => ep.Child2, () => child2)
					.JoinAlias(ep => ep.SameTypeChild, () => sameAsRootChild)
					.JoinAlias(ep => ep.ChildrenList, () => nullListElem, JoinType.LeftOuterJoin)
					.Select(
						Projections.RootEntity(),
						Projections.Entity(() => child1),
						Projections.Entity(() => child2),
						Projections.Entity(() => sameAsRootChild),
						Projections.Entity(() => nullListElem)
					)
					.Take(1).SingleOrDefault<object[]>();

				root = (EntityComplex) objects[0];
				child1 = (EntitySimpleChild) objects[1];
				child2 = (EntitySimpleChild) objects[2];
				sameAsRootChild = (EntityComplex) objects[3];
				nullListElem = (EntitySimpleChild) objects[4];

				Assert.That(NHibernateUtil.IsInitialized(root), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(child2), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(sameAsRootChild), Is.True, "Object must be initialized");
				Assert.That(nullListElem, Is.Null, "Object must be initialized");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void ReadOnlyProjection()
		{
			using (var session = OpenSession())
			{
				EntityComplex entitypRoot = session
					.QueryOver<EntityComplex>()
					.Select(Projections.RootEntity().SetReadonly(true))
					.Take(1).SingleOrDefault();

				Assert.That(session.IsReadOnly(entitypRoot), Is.True, "Object must be loaded readonly.");
			}
		}

		[Test]
		public void EntityProjectionForCompositeKeyInitialized()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var composite = session
					.QueryOver<EntityWithCompositeId>()
					.Select(Projections.RootEntity())
					.Take(1).SingleOrDefault();


				Assert.That(composite, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(composite), Is.True, "Object must be initialized");
				Assert.That(composite, Is.EqualTo(_entityWithCompositeId).Using((EntityWithCompositeId x, EntityWithCompositeId y) => (Equals(x.Key, y.Key) && Equals(x.Name, y.Name)) ? 0 : 1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
			
		}

		[Test]
		public void EntityProjectionForCompositeKeyLazy()
		{
			using (var session = OpenSession())
			{
				var composite = session
					.QueryOver<EntityWithCompositeId>()
					.Select(Projections.RootEntity().SetLazy(true))
					.Take(1).SingleOrDefault();


				Assert.That(composite, Is.Not.Null);
				Assert.That(composite, Is.EqualTo(_entityWithCompositeId).Using((EntityWithCompositeId x, EntityWithCompositeId y) => (Equals(x.Key, y.Key)) ? 0 : 1));
				Assert.That(NHibernateUtil.IsInitialized(composite), Is.False, "Object must be lazy loaded.");
			}
		}

		private void CreateObjects()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var child1 = new EntitySimpleChild
				{
					Name = "Child1"
				};
				var child2 = new EntitySimpleChild
				{
					Name = "Child1"
				};


				var parent = new EntityComplex
				{
					Name = "ComplexEnityParent",
					Child1 = child1,
					Child2 = child2,
					LazyProp = "SomeBigValue",
					SameTypeChild = new EntityComplex() { Name = "ComplexEntityChild" }
				};

				_entityWithCompositeId = new EntityWithCompositeId
				{
					Key = new CompositeKey
					{
						Id1 = 1,
						Id2 = 2
					},
					Name = "Composite"
				};

				session.Save(child1);
				session.Save(child2);
				session.Save(parent.SameTypeChild);
				session.Save(parent);
				session.Save(_entityWithCompositeId);

				session.Flush();
				transaction.Commit();
			}
		}

	}
}
