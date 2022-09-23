using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlCommand;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	/// <summary>
	/// Tests for explicit entity joins (not associated entities)
	/// </summary>
	[TestFixture]
	public class EntityJoinCriteriaTest : TestCaseMappingByCode
	{
		private const string customEntityName = "CustomEntityName";
		private EntityWithCompositeId _entityWithCompositeId;
		private EntityWithNoAssociation _noAssociation;
		private EntityCustomEntityName _entityWithCustomEntityName;

		//check JoinEntityAlias - JoinAlias analog for entity join
		[Test]
		public void CanJoinNotAssociatedEntity()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
						.JoinEntityAlias(() => entityComplex, Restrictions.Where(() => root.Complex1Id == entityComplex.Id)).Take(1)
						.SingleOrDefault();
				entityComplex = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		//GH1680
		[Test]
		public void CanRowCountWithEntityJoin()
		{
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = null;
				EntityWithNoAssociation root = null;
				var query = session.QueryOver(() => root)
						.JoinEntityAlias(() => entityComplex, Restrictions.Where(() => root.Complex1Id == entityComplex.Id));

				var rowCountQuery = query.ToRowCountQuery();
				int rowCount = 0;
				Assert.DoesNotThrow(() => rowCount = rowCountQuery.SingleOrDefault<int>(), "row count query should not throw exception");
				Assert.That(rowCount, Is.GreaterThan(0), "row count should be > 0");
			}
		}

		//check JoinEntityAlias - JoinAlias analog for entity join
		[Test]
		public void CanJoinNotAssociatedEntity_Expression()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
						.JoinEntityAlias(() => entityComplex, () => root.Complex1Id == entityComplex.Id).Take(1)
						.SingleOrDefault();
				entityComplex = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		//check JoinEntityQueryOver - JoinQueryOver analog for entity join
		[Test]
		public void CanJoinEntityQueryOver()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejQueryOver = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							.JoinEntityQueryOver(() => ejQueryOver, Restrictions.Where(() => root.Complex1Id == ejQueryOver.Id))
							.Take(1)
							.SingleOrDefault();
				ejQueryOver = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(ejQueryOver), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		//check JoinEntityQueryOver - JoinQueryOver analog for entity join
		[Test]
		public void CanJoinEntityQueryOver_Expression()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejQueryOver = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							.JoinEntityQueryOver(() => ejQueryOver, () => root.Complex1Id == ejQueryOver.Id)
							.Take(1)
							.SingleOrDefault();
				ejQueryOver = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(ejQueryOver), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		//can join not associated entity and join associated entities for it via JoinQueryOver
		[Test]
		public void CanQueryOverForAssociationInNotAssociatedEntity()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejComplex = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							.JoinEntityQueryOver(() => ejComplex, () => root.Complex1Id == ejComplex.Id)
							.JoinQueryOver(ej => ej.Child1)
							.Take(1)
							.SingleOrDefault();

				ejComplex = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(ejComplex), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(ejComplex.Child1), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(ejComplex.Child2), Is.False);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void SimpleProjectionForEntityJoin()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejComplex = null;
				EntityWithNoAssociation root = null;
				var name = session.QueryOver(() => root)
							.JoinEntityQueryOver(() => ejComplex, () => root.Complex1Id == ejComplex.Id)
							.Select((e) => ejComplex.Name)
							.Take(1)
							.SingleOrDefault<string>();
				
				Assert.That(name, Is.Not.Empty);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void EntityProjectionForEntityJoin()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntitySimpleChild ejChild1 = null;

				EntityComplex root = null;
				EntityComplex st = null;
				var r = session
						.QueryOver(() => root)
						.JoinAlias(c => c.SameTypeChild, () => st)
						.JoinEntityAlias(() => ejChild1, () => ejChild1.Id == root.Child1.Id)
						.Select(
							Projections.RootEntity(),
							Projections.Entity(() => st),
							Projections.Entity(() => ejChild1)
						)
						.SingleOrDefault<object[]>();
				var rootObj = (EntityComplex) r[0];
				var mappedObj =  (EntityComplex) r[1];
				var entityJoinObj =  (EntitySimpleChild) r[2];

				Assert.That(rootObj, Is.Not.Null);
				Assert.That(mappedObj, Is.Not.Null);
				Assert.That(entityJoinObj, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(rootObj), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(mappedObj), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(entityJoinObj), Is.True);
			}
		}

		//just check that it can be executed without error
		[Test]
		public void MixOfJoinsForAssociatedAndNotAssociatedEntities()
		{
#pragma warning disable CS8073 //The result of the expression is always 'false'
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex root = null;
				EntityComplex ejLevel1 = null;
				EntitySimpleChild customChildForEjLevel1 = null;
				EntityComplex entityComplexForEjLevel1 = null;
				EntitySimpleChild ejLevel2OnEntityComplexForEjLevel1 = null;
				var obj = session
						.QueryOver(() => root)
						.JoinEntityAlias(() => ejLevel1, Restrictions.Where(() => ejLevel1.Id == root.SameTypeChild.Id && root.Id != null), JoinType.LeftOuterJoin)
						.JoinAlias(() => ejLevel1.Child1, () => customChildForEjLevel1, JoinType.InnerJoin)
						.JoinAlias(() => ejLevel1.SameTypeChild, () => entityComplexForEjLevel1, JoinType.LeftOuterJoin)
						.JoinEntityAlias(() => ejLevel2OnEntityComplexForEjLevel1, () => entityComplexForEjLevel1.Id == ejLevel2OnEntityComplexForEjLevel1.Id)
						.Where(() => customChildForEjLevel1.Id != null && ejLevel2OnEntityComplexForEjLevel1.Id != null)
						.Take(1)
						.SingleOrDefault<object>();
			}
#pragma warning restore CS8073 //The result of the expression is always 'false'
		}

		[Test]
		public void EntityJoinForCompositeKey()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityWithCompositeId ejComposite = null;
				EntityWithNoAssociation root = null;
				root = session
								.QueryOver(() => root)
								.JoinEntityAlias(() => ejComposite, () => root.Composite1Key1 == ejComposite.Key.Id1 && root.Composite1Key2 == ejComposite.Key.Id2)
								.Take(1).SingleOrDefault();
				var composite = session.Load<EntityWithCompositeId>(_entityWithCompositeId.Key);

				Assert.That(NHibernateUtil.IsInitialized(composite), Is.True, "Object must be initialized");
				Assert.That(composite, Is.EqualTo(_entityWithCompositeId).Using((EntityWithCompositeId x, EntityWithCompositeId y) => (Equals(x.Key, y.Key) && Equals(x.Name, y.Name)) ? 0 : 1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void NullLeftEntityJoin()
		{
#pragma warning disable CS8073 //The result of the expression is always 'false'
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejLeftNull = null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							//add some non existent join condition
							.JoinEntityAlias(() => ejLeftNull, () => ejLeftNull.Id == null, JoinType.LeftOuterJoin)
							.Take(1)
							.SingleOrDefault();

				Assert.That(root, Is.Not.Null, "root should not be null (looks like left join didn't work)");
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
#pragma warning restore CS8073 //The result of the expression is always 'false'
		}	
		
		[Test]
		public void NullLeftEntityJoinWithEntityProjection()
		{
#pragma warning disable CS8073 //The result of the expression is always 'false'
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ejLeftNull = null;
				EntityWithNoAssociation root = null;
				var objs = session.QueryOver(() => root)
							//add some non existent join condition
							.JoinEntityAlias(() => ejLeftNull, () => ejLeftNull.Id == null, JoinType.LeftOuterJoin)
							.Select((e) => root.AsEntity(), e => ejLeftNull.AsEntity())
							.Take(1)
							.SingleOrDefault<object[]>();
				root = (EntityWithNoAssociation) objs[0];
				ejLeftNull = (EntityComplex) objs[1];

				Assert.That(root, Is.Not.Null, "root should not be null (looks like left join didn't work)");
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(ejLeftNull, Is.Null, "Entity join should be null");
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
#pragma warning restore CS8073 //The result of the expression is always 'false'
		}

		[Test]
		public void EntityJoinForCustomEntityName()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityCustomEntityName ejCustomEntity= null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							.JoinEntityAlias(() => ejCustomEntity, Restrictions.Where(() => ejCustomEntity.Id == root.CustomEntityNameId), JoinType.InnerJoin, customEntityName)
							.Take(1)
							.SingleOrDefault();

				ejCustomEntity = (EntityCustomEntityName) session.Load(customEntityName, root.CustomEntityNameId);

				Assert.That(NHibernateUtil.IsInitialized(ejCustomEntity), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void EntityJoinForCustomEntityName_Expression()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityCustomEntityName ejCustomEntity= null;
				EntityWithNoAssociation root = null;
				root = session.QueryOver(() => root)
							.JoinEntityAlias(() => ejCustomEntity, () => ejCustomEntity.Id == root.CustomEntityNameId, JoinType.InnerJoin, customEntityName)
							.Take(1)
							.SingleOrDefault();

				ejCustomEntity = (EntityCustomEntityName) session.Load(customEntityName, root.CustomEntityNameId);

				Assert.That(NHibernateUtil.IsInitialized(ejCustomEntity), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public void EntityJoinFoSubquery_JoinEntityAlias()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ej = null;
				EntityWithNoAssociation root = null;

				EntityComplex ejSub = null;
				EntityWithNoAssociation rootSub = null;

				var subquery = QueryOver.Of(() => rootSub)
										.JoinEntityAlias(() => ejSub, () => rootSub.Complex1Id == ejSub.Id)
										.Where(r => ejSub.Name == ej.Name)
										.Select(x => ejSub.Id);

				root = session.QueryOver(() => root)
							.JoinEntityAlias(() => ej, () => root.Complex1Id == ej.Id)
							.WithSubquery.WhereExists(subquery)
							.SingleOrDefault();
				ej = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(ej), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}
		
		[Test]
		public void EntityJoinFoSubquery_JoinQueryOver()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ej = null;
				EntityWithNoAssociation root = null;

				EntityComplex ejSub = null;
				EntityWithNoAssociation rootSub = null;

				var subquery = QueryOver.Of(() => rootSub)
										.JoinEntityQueryOver(() => ejSub, () => rootSub.Complex1Id == ejSub.Id)
										.Where(x => x.Name == ej.Name)
										.Select(x => ejSub.Id);

				root = session.QueryOver(() => root)
							.JoinEntityAlias(() => ej, () => root.Complex1Id == ej.Id)
							.WithSubquery.WhereExists(subquery)
							.SingleOrDefault();
				ej = session.Load<EntityComplex>(root.Complex1Id);

				Assert.That(NHibernateUtil.IsInitialized(ej), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
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

			mapper.Class<EntityWithNoAssociation>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));

					rc.Property(e => e.Complex1Id);
					rc.Property(e => e.Complex2Id);
					rc.Property(e => e.Simple1Id);
					rc.Property(e => e.Simple2Id);
					rc.Property(e => e.Composite1Key1);
					rc.Property(e => e.Composite1Key2);
					rc.Property(e => e.CustomEntityNameId);
				});

			mapper.Class<EntityCustomEntityName>(
				rc =>
				{
					rc.EntityName(customEntityName);

					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
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

				_entityWithCustomEntityName = new EntityCustomEntityName()
				{
					Name = "EntityCustomEntityName"
				};

				session.Save(customEntityName, _entityWithCustomEntityName);

				_noAssociation = new EntityWithNoAssociation()
				{
					Complex1Id = parent.Id,
					Complex2Id = parent.SameTypeChild.Id,
					Composite1Key1 = _entityWithCompositeId.Key.Id1,
					Composite1Key2 = _entityWithCompositeId.Key.Id2,
					Simple1Id = child1.Id,
					Simple2Id = child2.Id,
					CustomEntityNameId = _entityWithCustomEntityName.Id
				};

				session.Save(_noAssociation);

				session.Flush();
				transaction.Commit();
			}
		}

		#endregion Test Setup
	}

	public class EntityWithNoAssociation
	{
		public virtual Guid Id { get; set; }
		public virtual Guid Complex1Id { get; set; }
		public virtual Guid Complex2Id { get; set; }
		public virtual Guid Simple1Id { get; set; }
		public virtual Guid Simple2Id { get; set; }
		public virtual int Composite1Key1 { get; set; }
		public virtual int Composite1Key2 { get; set; }
		public virtual Guid CustomEntityNameId { get; set; }
		public virtual string Name { get; set; }
	}
}
