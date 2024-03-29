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
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.Associations.OneToOneFixtureEntities;
using NHibernate.Util;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.Associations
{
	using System.Threading.Tasks;
	[TestFixture]
	public class OneToOneFixtureAsync : TestCaseMappingByCode
	{
		[Test]
		public async Task OneToOneCompositeQueryByEntityParamAsync()
		{
			using (var session = OpenSession())
			{
				var e = await (session.Query<EntityWithCompositeId>().FirstOrDefaultAsync());
				var loadedEntity = await (session.Query<Parent>().Where(p => p.OneToOneComp == e).FirstOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		//NH-3778 (GH-1368)
		[Test]
		public async Task OneToOneCompositeQueryOverByEntityParamAsync()
		{
			using (var session = OpenSession())
			{
				var e = await (session.Query<EntityWithCompositeId>().FirstOrDefaultAsync());
				var loadedEntity = await (session.QueryOver<Parent>().Where(p => p.OneToOneComp == e).SingleOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
	
		[Test]
		public async Task OneToOneCompositeQueryByKeyAsync()
		{
			using (var session = OpenSession())
			{
				var e = await (session.Query<EntityWithCompositeId>().FirstOrDefaultAsync());
				var loadedEntity = await (session.Query<Parent>().Where(p => p.OneToOneComp.Key == e.Key).FirstOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public async Task OneToOneCompositeQueryOverByKeyAsync()
		{
			using (var session = OpenSession())
			{
				var e = await (session.Query<EntityWithCompositeId>().FirstOrDefaultAsync());
				var loadedEntity = await (session.QueryOver<Parent>().Where(p => p.OneToOneComp.Key == e.Key).SingleOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}

		//NH-3469 (GH-1309)
		[Test]
		public async Task OneToOneCompositeQueryByNotNullAsync()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = await (session.Query<Parent>().Where(p => p.OneToOneComp != null).FirstOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public async Task OneToOneCompositeQueryOverByNotNullAsync()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = await (session.QueryOver<Parent>().Where(p => p.OneToOneComp != null).SingleOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public async Task OneToOneCompositeQueryCompareWithJoinAsync()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = await (session.CreateQuery("select e from Parent p, EntityWithCompositeId e where p.OneToOneComp = e").UniqueResultAsync<EntityWithCompositeId>());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Explicit("Expression in Restrictions.Where can't recognize direct alias comparison.")]
		[Test]
		public async Task OneToOneCompositeQueryOverCompareWithJoinAsync()
		{
			using(new SqlLogSpy())			
			using (var session = OpenSession())
			{
				Parent parent = null;
				EntityWithCompositeId oneToOne = null;

				var loadedEntity = await (session.QueryOver<Parent>(() => parent)
										.JoinEntityAlias(() => oneToOne, () => parent.OneToOneComp == oneToOne)
										.SingleOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}
		
		[Test]
		public async Task OneToOneCompositeQueryOverCompareWithJoinByIdAsync()
		{
			using (var session = OpenSession())
			{
				var e = await (session.Query<EntityWithCompositeId>().FirstOrDefaultAsync());
				Parent parent = null;
				EntityWithCompositeId oneToOne = null;

				var loadedEntity = await (session.QueryOver<Parent>(() => parent)
										.JoinEntityAlias(() => oneToOne, () => parent.OneToOneComp.Key == oneToOne.Key)
										.SingleOrDefaultAsync());

				Assert.That(loadedEntity, Is.Not.Null);
			}
		}

		//GH-2064
		[Test]
		public async Task OneToOneCompositeQuerySelectProjectionAsync()
		{
			using(var logSpy = new LogSpy(typeof(ReflectHelper)))
			using (var session = OpenSession())
			{
				var loadedProjection = await (session.Query<Parent>().Select(x => new {x.OneToOneComp, x.Key}).FirstOrDefaultAsync());

				Assert.That(loadedProjection.OneToOneComp, Is.Not.Null);
				// GH-2855 Error is logged
				Assert.That(logSpy.GetWholeLog(), Is.Empty);
			}
		}

		//NH-3178 (GH-1125)
		[Test]
		public async Task OneToOneQueryOverSelectProjectionAsync()
		{
			using (var session = OpenSession())
			{
				var loadedEntity = await (session.QueryOver<Parent>()
										.Select(x => x.OneToOneComp)
										.SingleOrDefaultAsync<EntityWithCompositeId>());

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
}
