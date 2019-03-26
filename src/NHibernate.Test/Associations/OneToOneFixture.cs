using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.Associations.OneToOneFixtureEntities;
using NUnit.Framework;

namespace NHibernate.Test.Associations
{
	[TestFixture]
	public class OneToOneFixture : TestCaseMappingByCode
	{
		[Test]
		public void OneToOneCompositeQueryByEntityParam()
		{
			using (var session = OpenSession())
			{
				var e = session.Query<EntityWithCompositeId>().FirstOrDefault();
				var loadedEntity = session.Query<Parent>().Where(p => p.OneToOneComp == e).FirstOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQueryOverByEntityParam()
		{
			using (var session = OpenSession())
			{
				var e = session.Query<EntityWithCompositeId>().FirstOrDefault();
				var loadedEntity = session.QueryOver<Parent>().Where(p => p.OneToOneComp == e).SingleOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
	
		[Test]
		public void OneToOneCompositeQueryByKey()
		{
			using (var session = OpenSession())
			{
				var e = session.Query<EntityWithCompositeId>().FirstOrDefault();
				var loadedEntity = session.Query<Parent>().Where(p => p.OneToOneComp.Key == e.Key).FirstOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQueryOverByKey()
		{
			using (var session = OpenSession())
			{
				var e = session.Query<EntityWithCompositeId>().FirstOrDefault();
				var loadedEntity = session.QueryOver<Parent>().Where(p => p.OneToOneComp.Key == e.Key).SingleOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}

		[Test]
		public void OneToOneCompositeQueryByNotNull()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = session.Query<Parent>().Where(p => p.OneToOneComp != null).FirstOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQueryOverByNotNull()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = session.QueryOver<Parent>().Where(p => p.OneToOneComp != null).SingleOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQueryCompareWithJoin()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = session.CreateQuery("select e from Parent p, EntityWithCompositeId e where p.OneToOneComp = e").UniqueResult<EntityWithCompositeId>();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Explicit("Expression in Restrictions.Where can't recognize direct alias comparison.")]
		[Test]
		public void OneToOneCompositeQueryOverCompareWithJoin()
		{
			using(new SqlLogSpy())			
			using (var session = OpenSession())
			{
				Parent parent = null;
				EntityWithCompositeId oneToOne = null;

				var loadedEntity = session.QueryOver<Parent>(() => parent)
										.JoinEntityAlias(() => oneToOne, () => parent.OneToOneComp == oneToOne)
										.SingleOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQueryOverCompareWithJoinById()
		{
			using (var session = OpenSession())
			{
				var e = session.Query<EntityWithCompositeId>().FirstOrDefault();
				Parent parent = null;
				EntityWithCompositeId oneToOne = null;

				var loadedEntity = session.QueryOver<Parent>(() => parent)
										.JoinEntityAlias(() => oneToOne, () => parent.OneToOneComp.Key == oneToOne.Key)
										.SingleOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneCompositeQuerySelectProjection()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = session.Query<Parent>().Select(x => new {x.OneToOneComp, x.Key}).FirstOrDefault();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public void OneToOneQueryOverSelectProjection()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = session.QueryOver<Parent>()
										.Select(x => x.OneToOneComp)
										.SingleOrDefault<EntityWithCompositeId>();

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		#region Test Setup

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

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

			mapper.Class<Parent>(
				rc =>
				{
					rc.ComponentAsId(
						e => e.Key,
						ekm =>
						{
							ekm.Property(ek => ek.Id1);
							ekm.Property(ek => ek.Id2);
						});

					rc.OneToOne(e => e.OneToOneComp, m => { });
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
				var key = new CompositeKey
				{
					Id1 = 4,
					Id2 = 3,
				};
				var oneToOneParent = new Parent()
				{
					OneToOneComp = new EntityWithCompositeId
					{
						Key = key,
						Name = "Composite2"
					},
					Key = key
				};

				session.Save(oneToOneParent.OneToOneComp);
				session.Save(oneToOneParent);

				session.Flush();
				transaction.Commit();
			}
		}

		#endregion Test Setup
	}

	namespace OneToOneFixtureEntities
	{
		public class CompositeKey
		{
			public int Id1 { get; set; }
			public int Id2 { get; set; }

			public override bool Equals(object obj)
			{
				var key = obj as CompositeKey;
				return key != null
						&& Id1 == key.Id1
						&& Id2 == key.Id2;
			}

			public override int GetHashCode()
			{
				var hashCode = -1596524975;
				hashCode = hashCode * -1521134295 + Id1.GetHashCode();
				hashCode = hashCode * -1521134295 + Id2.GetHashCode();
				return hashCode;
			}
		}

		public class EntityWithCompositeId
		{
			public virtual CompositeKey Key { get; set; }
			public virtual string Name { get; set; }
		}

		public class OneToOneEntity
		{
			public virtual Guid Id { get; set; }
			public virtual string Name { get; set; }
		}

		public class Parent
		{
			public virtual CompositeKey Key { get; set; }
			public virtual EntityWithCompositeId OneToOneComp { get; set; }
		}
	}
}
