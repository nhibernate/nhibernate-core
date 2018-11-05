using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	/// <summary>
	/// GH948
	/// </summary>
	[TestFixture]
	public class EntityProjectionsTest : TestCaseMappingByCode
	{
		private const string customEntityName = "CustomEntityName";
		private EntityWithCompositeId _entityWithCompositeId;
		private EntityCustomEntityName _entityWithCustomEntityName;

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

			mapper.Class<EntityCustomEntityName>(
				rc =>
				{
					rc.EntityName(customEntityName);

					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
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
					SameTypeChild = new EntityComplex()
					{
						Name = "ComplexEntityChild"
					}
				};

				_entityWithCustomEntityName = new EntityCustomEntityName()
				{
					Name = "EntityCustomEntityName"
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
				session.Save(customEntityName, _entityWithCustomEntityName);

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void RootEntityProjectionFullyInitializedAndWithUnfetchedLazyPropertiesByDefault()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				Sfi.Statistics.Clear();
				EntityComplex entityRoot = session
					.QueryOver<EntityComplex>()
					.Where(ec => ec.LazyProp != null)
					.Select(Projections.RootEntity())
					.Take(1).SingleOrDefault();

				Assert.That(NHibernateUtil.IsInitialized(entityRoot), Is.True, "Object must be initialized by default");
				Assert.That(session.IsReadOnly(entityRoot), Is.False, "Object must not be readonly by default");
				Assert.That(NHibernateUtil.IsPropertyInitialized(entityRoot, nameof(entityRoot.LazyProp)), Is.False, "Lazy properties should not be initialized by default.");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void RootEntityProjectionLazy()
		{
			using (var session = OpenSession())
			{
				EntityComplex entityRoot = session
					.QueryOver<EntityComplex>()
					.Select(Projections.RootEntity().SetLazy(true))
					.Take(1).SingleOrDefault();


				Assert.That(NHibernateUtil.IsInitialized(entityRoot), Is.False, "Object must be lazy loaded.");
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
		public void EntityProjectionAsSelectExpression()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				child1 = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.Select(ec => child1.AsEntity())
					.Take(1).SingleOrDefault<EntitySimpleChild>();

				Assert.That(child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.True, "Object must be initialized");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void EntityProjectionAsSelectExpressionForArgumentAlias()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				var complex = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.Select(ec => ec.AsEntity())
					.Take(1).SingleOrDefault();

				Assert.That(complex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(complex), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(complex.Child1), Is.False, "Object must be lazy");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void EntityProjectionLockMode()
		{
			if (Dialect is Oracle8iDialect)
				Assert.Ignore("Oracle is not supported due to #1352 bug (NH-3902)");

			var upgradeHint = Dialect.ForUpdateString;
			if(string.IsNullOrEmpty(upgradeHint))
				upgradeHint = this.Dialect.AppendLockHint(LockMode.Upgrade, string.Empty);
			if (string.IsNullOrEmpty(upgradeHint))
			{
				Assert.Ignore($"Upgrade hint is not supported by dialect {Dialect.GetType().Name}");
			}

			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild child1 = null;
				child1 = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.Lock(() => child1).Upgrade
					.Select(Projections.Entity(() => child1))
					.Take(1).SingleOrDefault<EntitySimpleChild>();
				
				Assert.That(child1, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.True, "Object must be initialized");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
				Assert.That(sqlLog.Appender.GetEvents()[0].RenderedMessage, Does.Contain(upgradeHint));
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

				Assert.That(NHibernateUtil.IsInitialized(root), Is.False, "root must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(sameTypeChild), Is.False, "sameTypeChild must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.False, "child1 must be lazy loaded.");
				Assert.That(NHibernateUtil.IsInitialized(child2), Is.False, "child2 must be lazy loaded.");

				//make sure objects are populated from different aliases for the same types
				Assert.That(root.Id, Is.Not.EqualTo(sameTypeChild.Id), "Different objects are expected for root and sameTypeChild.");
				Assert.That(child1.Id, Is.Not.EqualTo(child2.Id), "Different objects are expected for child1 and child2.");

			}
		}

		[Test]
		public void EntityProjectionWithLazyPropertiesFetched()
		{
			using (var session = OpenSession())
			{
				EntityComplex entityRoot = session
					.QueryOver<EntityComplex>()
					.Where(ec => ec.LazyProp != null)
					.Select(Projections.RootEntity().SetFetchLazyProperties(true))
					.Take(1).SingleOrDefault();

				Assert.That(entityRoot, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityRoot), Is.True, "Object must be initialized");
				Assert.That(NHibernateUtil.IsPropertyInitialized(entityRoot, nameof(entityRoot.LazyProp)), Is.True, "Lazy property must be initialized");
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

				Assert.That(NHibernateUtil.IsInitialized(root), Is.True, "root must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(child1), Is.True, "child1 must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(child2), Is.True, "child2 must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(sameAsRootChild), Is.True, "sameAsRootChild must be initialized");
				Assert.That(nullListElem, Is.Null, "nullListElem must be null");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		class MultipleEntitiesResult
		{
			public EntityComplex Root { get; set; }
			public EntitySimpleChild Child1 { get; set; }
			public EntitySimpleChild Child2 { get; set; }
			public EntityComplex SameAsRootChild { get; set; }
			public EntitySimpleChild NullListElem { get; set; }
			public string Name { get; set; }
		}

		[Test]
		public void MultipleEntitiesProjectionsToResultTransformer()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				MultipleEntitiesResult r = null;

				EntitySimpleChild child1 = null;
				EntitySimpleChild child2 = null;
				EntityComplex sameAsRootChild = null;
				EntitySimpleChild nullListElem = null;

				r = session
					.QueryOver<EntityComplex>()
					.JoinAlias(ep => ep.Child1, () => child1)
					.JoinAlias(ep => ep.Child2, () => child2)
					.JoinAlias(ep => ep.SameTypeChild, () => sameAsRootChild)
					.JoinAlias(ep => ep.ChildrenList, () => nullListElem, JoinType.LeftOuterJoin)
					.Select(
						Projections.RootEntity().WithAlias(nameof(r.Root)),
						Projections.Entity(() => child1),
						Projections.Property(() => child2.Name).As(nameof(r.Name)),
						Projections.Entity(() => child2),
						Projections.Property(() => child1.Id),
						Projections.Entity(() => sameAsRootChild),
						Projections.Entity(() => nullListElem)
					)
					.TransformUsing(Transformers.AliasToBean<MultipleEntitiesResult>())
					.Take(1)
					.SingleOrDefault<MultipleEntitiesResult>();

				Assert.That(NHibernateUtil.IsInitialized(r.Root), Is.True, "Root must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(r.Child1), Is.True, "Child1 must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(r.Child2), Is.True, "Child2 must be initialized");
				Assert.That(NHibernateUtil.IsInitialized(r.SameAsRootChild), Is.True, "SameAsRootChild must be initialized");
				Assert.That(r.NullListElem, Is.Null, "NullListElem must be null");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}


		[Test]
		public void ReadOnlyProjection()
		{
			using (var session = OpenSession())
			{
				EntityComplex entityRoot = session
					.QueryOver<EntityComplex>()
					.Select(Projections.RootEntity())
					.ReadOnly()
					.Take(1).SingleOrDefault();

				Assert.That(session.IsReadOnly(entityRoot), Is.True, "Object must be loaded readonly.");
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

		[Test]
		public void EntityProjectionForCustomEntityName()
		{
			using (var session = OpenSession())
			{
				var entity = session
							.QueryOver<EntityCustomEntityName>(customEntityName)
							.Select(Projections.RootEntity())
							.Take(1).SingleOrDefault();

				Assert.That(entity, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entity), Is.True, "Object must be initialized.");
				Assert.That(entity.Id, Is.EqualTo(_entityWithCustomEntityName.Id));
				Assert.That(entity.Name, Is.EqualTo(_entityWithCustomEntityName.Name));
			}
		}
	}
}
