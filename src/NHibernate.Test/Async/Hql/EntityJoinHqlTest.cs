﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using System.Text.RegularExpressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.Hql.EntityJoinHqlTestEntities;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Hql
{
	using System.Threading.Tasks;
	/// <summary>
	/// Tests for explicit entity joins (not associated entities)
	/// </summary>
	[TestFixture]
	public class EntityJoinHqlTestAsync : TestCaseMappingByCode
	{
		private const string _customEntityName = "CustomEntityName.Test";
		private EntityWithCompositeId _entityWithCompositeId;
		private EntityWithNoAssociation _noAssociation;
		private EntityCustomEntityName _entityWithCustomEntityName;

		[Test]
		public async Task CanJoinNotAssociatedEntityAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						"left join EntityComplex ex with root.Complex1Id = ex.Id")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task CanJoinNotAssociatedEntityFullNameAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						$"left join {typeof(EntityComplex).FullName} ex with root.Complex1Id = ex.Id")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task CanJoinNotAssociatedInterfaceFullNameAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						$"left join {typeof(IEntityComplex).FullName} ex with root.Complex1Id = ex.Id")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task CanJoinNotAssociatedEntity_OnKeywordAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						"left join EntityComplex ex on root.Complex1Id = ex.Id")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinForCompositeKeyAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				await (session.CreateQuery(
					"select root, ejComposite from EntityWithNoAssociation root " +
					"inner join EntityWithCompositeId ejComposite " +
					"	with (root.Composite1Key1 = ejComposite.Key.Id1 and root.Composite1Key2 = ejComposite.Key.Id2)")
					.SetMaxResults(1).ListAsync());

				var composite = await (session.LoadAsync<EntityWithCompositeId>(_entityWithCompositeId.Key));

				Assert.That(composite, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(composite), Is.True, "Object must be initialized");
				Assert.That(composite, Is.EqualTo(_entityWithCompositeId).Using((EntityWithCompositeId x, EntityWithCompositeId y) => (Equals(x.Key, y.Key) && Equals(x.Name, y.Name)) ? 0 : 1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task NullLeftEntityJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var objs =await (session.CreateQuery(
					"select ejLeftNull, root " +
					"from EntityWithNoAssociation root " +
					"left join EntityComplex ejLeftNull with ejLeftNull.Id is null")
					.SetMaxResults(1).UniqueResultAsync<object[]>());
				EntityComplex ejLeftNull = (EntityComplex)objs[0];
				EntityWithNoAssociation root = (EntityWithNoAssociation) objs[1];

				Assert.That(root, Is.Not.Null, "root should not be null (looks like left join didn't work)");
				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(ejLeftNull, Is.Null);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinForCustomEntityNameAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityCustomEntityName ejCustomEntity = await (session.CreateQuery(
					$"select ejCustomEntity from EntityWithNoAssociation root inner join {_customEntityName} ejCustomEntity with ejCustomEntity.Id = root.CustomEntityNameId")
					.SetMaxResults(1).UniqueResultAsync<EntityCustomEntityName>());

				Assert.That(ejCustomEntity, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(ejCustomEntity), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinFoSubqueryAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex ej = null;
				EntityWithNoAssociation root = null;

				var subquery = "from EntityWithNoAssociation rootSub " +
					"inner join EntityComplex ejSub with rootSub.Complex1Id = ejSub.Id " +
					"where ejSub.Name = ej.Name";
				var objs = await (session.CreateQuery(
					"select root, ej from EntityWithNoAssociation root " +
					"inner join EntityComplex ej with root.Complex1Id = ej.Id " +
					$"where exists ({subquery})")
					.UniqueResultAsync<object[]>());
				root = (EntityWithNoAssociation) objs[0];
				ej = (EntityComplex)objs[1];

				Assert.That(NHibernateUtil.IsInitialized(root), Is.True);
				Assert.That(ej, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(ej), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinWithNullableOneToOneEntityComparisonInWithClausShouldAddJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex "
							+ "left join OneToOneEntity st with st = ex.OneToOne "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "OneToOneEntity").Count, Is.EqualTo(2));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task NullableOneToOneWhereEntityIsNotNullAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex "
							+ "where ex.OneToOne is not null "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "OneToOneEntity").Count, Is.EqualTo(1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task NullableOneToOneWhereIdIsNotNullAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex "
							+ "where ex.OneToOne.Id is not null "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "OneToOneEntity").Count, Is.EqualTo(1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task NullablePropRefWhereIdEntityNotNullShouldAddJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex "
							+ "where ex.PropRef is not null "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "PropRefEntity").Count, Is.EqualTo(1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test(Description = "GH-2688")]
		public async Task NullableManyToOneDeleteQueryAsync()
		{
			using (var session = OpenSession())
			{
				await (session
					.CreateQuery(
						"delete "
						+ "from NullableOwner ex "
						+ "where ex.ManyToOne.id = :id"
					).SetParameter("id", Guid.Empty)
					.ExecuteUpdateAsync());
			}
		}

		[Test]
		public async Task NullableOneToOneFetchQueryIsNotAffectedAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex left join fetch ex.OneToOne o "
							+ "where o is null "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "OneToOneEntity").Count, Is.EqualTo(1));
			}
		}
		
		[Test]
		public async Task NullableOneToOneFetchQueryIsNotAffected2Async()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var entity =
					await (session
						.CreateQuery(
							"select ex "
							+ "from NullableOwner ex left join fetch ex.OneToOne o "
							+ "where o.Id is null "
						).SetMaxResults(1)
						.UniqueResultAsync<NullableOwner>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "OneToOneEntity").Count, Is.EqualTo(1));
			}
		}

		[Test(Description = "GH-2772")]
		public async Task NullableEntityProjectionAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var nullableOwner1 = new NullableOwner {Name = "1", ManyToOne = await (session.LoadAsync<OneToOneEntity>(Guid.NewGuid()))};
				var nullableOwner2 = new NullableOwner {Name = "2"};
				await (session.SaveAsync(nullableOwner1));
				await (session.SaveAsync(nullableOwner2));

				var fullList = await (session.Query<NullableOwner>().Select(x => new {x.Name, ManyToOneId = (Guid?) x.ManyToOne.Id}).ToListAsync());
				var withValidManyToOneList = await (session.Query<NullableOwner>().Where(x => x.ManyToOne != null).Select(x => new {x.Name, ManyToOneId = (Guid?) x.ManyToOne.Id}).ToListAsync());
				var withValidManyToOneList2 = await (session.CreateQuery("from NullableOwner ex where not ex.ManyToOne is null").ListAsync<NullableOwner>());
				var withNullManyToOneList = await (session.Query<NullableOwner>().Where(x => x.ManyToOne == null).ToListAsync());
				var withNullManyToOneJoinedList =
					await ((from x in session.Query<NullableOwner>()
							from x2 in session.Query<NullableOwner>()
							where x == x2 && x.ManyToOne == null && x.OneToOne.Name == null
							select x2).ToListAsync());
				Assert.That(fullList.Count, Is.EqualTo(2));
				Assert.That(withValidManyToOneList.Count, Is.EqualTo(0));
				Assert.That(withValidManyToOneList2.Count, Is.EqualTo(0));
				Assert.That(withNullManyToOneList.Count, Is.EqualTo(2));
				Assert.That(withNullManyToOneJoinedList.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public async Task EntityJoinWithEntityComparisonInWithClausShouldNotAddJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex =
					await (session
						.CreateQuery(
							"select ex "
							+ "from EntityComplex ex "
							+ "inner join EntityComplex st with st = ex.SameTypeChild "
						).SetMaxResults(1)
						.UniqueResultAsync<EntityComplex>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "EntityComplex").Count, Is.EqualTo(2));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinWithEntityAssociationComparisonShouldAddJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex =
					await (session
						.CreateQuery(
							"select ex "
							+ "from EntityComplex ex "
							+ "inner join EntityComplex st with st = ex.SameTypeChild.SameTypeChild "
						).SetMaxResults(1)
						.UniqueResultAsync<EntityComplex>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "EntityComplex").Count, Is.EqualTo(3));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinWithEntityAssociationComparison2ShouldAddJoinAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex =
					await (session
						.CreateQuery(
							"select ex "
							+ "from EntityComplex ex "
							+ "inner join EntityComplex st with st.Version = ex.SameTypeChild.Version "
						).SetMaxResults(1)
						.UniqueResultAsync<EntityComplex>());

				Assert.That(Regex.Matches(sqlLog.GetWholeLog(), "EntityComplex").Count, Is.EqualTo(3));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task WithClauseOnOtherAssociationAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityComplex ex join fetch ex.SameTypeChild stc " +
						"join ex.SameTypeChild2 stc2 with stc.Version != stc2.Version ")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Null);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinNoTablesInWithClauseAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						"left join EntityComplex ex with 1 = 2")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Null);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task EntityJoinWithFetchesAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				EntityComplex entityComplex = 
				await (session
					.CreateQuery("select ex " +
						"from EntityWithNoAssociation root " +
						"left join EntityComplex ex with root.Complex1Id = ex.Id " +
						"inner join fetch ex.SameTypeChild st")
						.SetMaxResults(1)
					.UniqueResultAsync<EntityComplex>());

				Assert.That(entityComplex, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex), Is.True);
				Assert.That(entityComplex.SameTypeChild, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(entityComplex.SameTypeChild), Is.True);
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
			}
		}

		[Test]
		public async Task WithImpliedJoinOnAssociationAsync()
		{
			using (var session = OpenSession())
			{
				var result = await (session.CreateQuery(
					"SELECT s " +
					"FROM EntityComplex s left join s.SameTypeChild q on q.SameTypeChild.SameTypeChild.Name = s.Name"
				).ListAsync());
			}
		}
		
		[Test]
		public async Task WithImpliedEntityJoinAsync()
		{
			using (var session = OpenSession())
			{
				var result = await (session.CreateQuery(
					"SELECT s " +
					"FROM EntityComplex s left join EntityComplex q on q.SameTypeChild.SameTypeChild.Name = s.Name"
				).ListAsync());
			}
		}

		[Test]
		public async Task CrossJoinAndWhereClauseAsync()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var session = OpenSession())
			{
				var result = await (session.CreateQuery(
					"SELECT s " +
					"FROM EntityComplex s cross join EntityComplex q " +
					"where s.SameTypeChild.Id = q.SameTypeChild.Id"
				).ListAsync());

				Assert.That(result, Has.Count.EqualTo(1));
				Assert.That(sqlLog.Appender.GetEvents().Length, Is.EqualTo(1), "Only one SQL select is expected");
				if (Dialect.SupportsCrossJoin)
				{
					Assert.That(sqlLog.GetWholeLog(), Does.Contain("cross join"), "A cross join is expected in the SQL select");
				}
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

					rc.ManyToOne(ep => ep.SameTypeChild, m => m.Column("SameTypeChildId"));

					rc.ManyToOne(ep => ep.SameTypeChild2, m => m.Column("SameTypeChild2Id"));
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
					rc.EntityName(_customEntityName);

					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(e => e.Name);
				});

			mapper.Class<OneToOneEntity>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(e => e.Name);
				});

			mapper.Class<PropRefEntity>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(e => e.Name);
					rc.Property(e => e.PropertyRef, m => m.Column("EntityPropertyRef"));
				});

			mapper.Class<NullableOwner>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(e => e.Name);
					rc.OneToOne(e => e.OneToOne, m => m.Constrained(false));
					rc.ManyToOne(
						e => e.PropRef,
						m =>
						{
							m.Column("OwnerPropertyRef");
							m.PropertyRef(nameof(PropRefEntity.PropertyRef));
							m.ForeignKey("none");
							m.NotFound(NotFoundMode.Ignore);
						});
					rc.ManyToOne(e => e.ManyToOne, m => m.NotFound(NotFoundMode.Ignore));
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
				var parent = new EntityComplex
				{
					Name = "ComplexEnityParent",
					LazyProp = "SomeBigValue",
					SameTypeChild = new EntityComplex()
					{
						Name = "ComplexEntityChild"
					},
					SameTypeChild2 = new EntityComplex()
					{
						Name = "ComplexEntityChild2"
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

				session.Save(parent.SameTypeChild);
				session.Save(parent.SameTypeChild2);
				session.Save(parent);
				session.Save(_entityWithCompositeId);

				_entityWithCustomEntityName = new EntityCustomEntityName()
				{
					Name = "EntityCustomEntityName"
				};

				session.Save(_customEntityName, _entityWithCustomEntityName);

				_noAssociation = new EntityWithNoAssociation()
				{
					Complex1Id = parent.Id,
					Complex2Id = parent.SameTypeChild.Id,
					Composite1Key1 = _entityWithCompositeId.Key.Id1,
					Composite1Key2 = _entityWithCompositeId.Key.Id2,
					CustomEntityNameId = _entityWithCustomEntityName.Id
				};
				session.Save(_noAssociation);

				session.Flush();
				transaction.Commit();
			}
		}

		#endregion Test Setup
	}
}
