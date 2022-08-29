using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2043
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		private Guid _entityWithClassProxy2Id;
		private Guid _entityWithInterfaceProxy2Id;
		private Guid _entityWithClassLookupId;
		private Guid _entityWithInterfaceLookupId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityWithClassProxyDefinition>(rc =>
			{
				rc.Table("ProxyDefinition");
				rc.Proxy(typeof(EntityWithClassProxyDefinition));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithInterfaceProxyDefinition>(rc =>
			{
				rc.Table("IProxyDefinition");
				rc.Proxy(typeof(IEntityProxy));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithClassLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x => x.Class(typeof(EntityWithClassProxyDefinition)));
			});

			mapper.Class<EntityWithInterfaceLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x => x.Class(typeof(EntityWithInterfaceProxyDefinition)));
			});

			mapper.Class<EntityAssigned>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
			});

			mapper.Class<EntityWithAssignedBag>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
					rc.Property(x => x.Name);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Inverse(true);
							m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						},
						cm => { cm.OneToMany(); });
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityCP1 = new EntityWithClassProxyDefinition
				{
					Id = Guid.NewGuid(),
					Name = "Name 1"
				};

				var entityCP2 = new EntityWithClassProxyDefinition
				{
					Id = Guid.NewGuid(),
					Name = "Name 2"
				};
				_entityWithClassProxy2Id = entityCP2.Id;

				var entityIP1 = new EntityWithInterfaceProxyDefinition
				{
					Id = Guid.NewGuid(),
					Name = "Name 1"
				};

				var entityIP2 = new EntityWithInterfaceProxyDefinition
				{
					Id = Guid.NewGuid(),
					Name = "Name 2"
				};
				_entityWithInterfaceProxy2Id = entityIP2.Id;

				session.Save(entityCP1);
				session.Save(entityCP2);
				session.Save(entityIP1);
				session.Save(entityIP2);

				var entityCL = new EntityWithClassLookup
				{
					Id = Guid.NewGuid(),
					Name = "Name 1",
					EntityLookup = (EntityWithClassProxyDefinition) session.Load(typeof(EntityWithClassProxyDefinition), entityCP1.Id)
				};
				_entityWithClassLookupId = entityCL.Id;

				var entityIL = new EntityWithInterfaceLookup
				{
					Id = Guid.NewGuid(),
					Name = "Name 1",
					EntityLookup = (IEntityProxy) session.Load(typeof(EntityWithInterfaceProxyDefinition), entityIP1.Id)
				};
				_entityWithInterfaceLookupId = entityIL.Id;

				session.Save(entityCL);
				session.Save(entityIL);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void UpdateEntityWithClassLookup()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityToUpdate = session.Query<EntityWithClassLookup>()
											.First(e => e.Id == _entityWithClassLookupId);

				entityToUpdate.EntityLookup = (EntityWithClassProxyDefinition) session.Load(typeof(EntityWithClassProxyDefinition), _entityWithClassProxy2Id);

				session.Update(entityToUpdate);
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void UpdateEntityWithInterfaceLookup()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entityToUpdate = session.Query<EntityWithInterfaceLookup>()
											.First(e => e.Id == _entityWithInterfaceLookupId);

				entityToUpdate.EntityLookup = (IEntityProxy) session.Load(typeof(EntityWithInterfaceProxyDefinition), _entityWithInterfaceProxy2Id);

				session.Update(entityToUpdate);
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void TransientProxySave()
		{
			var id = 10;

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() { Id = id, Name = "a" };

				session.Save(e);
				session.Flush();
				t.Commit();
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var e = GetTransientProxy(session, id);
				session.Save(e);
				session.Flush();

				t.Commit();
			}

			using (var session = OpenSession())
			{
				var entity = session.Get<EntityAssigned>(id);
				Assert.That(entity, Is.Not.Null, "Transient proxy wasn't saved");
			}
		}

		[Test]
		public void TransientProxyBagCascadeSave()
		{
			var id = 10;

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() { Id = id, Name = "a" };
				session.Save(e);
				session.Flush();
				t.Commit();
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var child = GetTransientProxy(session, id);
				var parent = new EntityWithAssignedBag()
				{
					Id = 1,
					Name = "p",
					Children =
					{
						child
					}
				};
				child.Parent = parent;

				session.Save(parent);
				session.Flush();

				t.Commit();
			}

			using (var session = OpenSession())
			{
				var entity = session.Get<EntityAssigned>(id);
				Assert.That(entity, Is.Not.Null, "Transient proxy wasn't saved");
			}
		}

		[Test]
		public void TransientProxyDetectionFromUserCode()
		{
			var id = 10;

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var e = new EntityAssigned() { Id = id, Name = "a" };
				session.Save(e);
				session.Flush();
				t.Commit();
			}

			using (var session = OpenSession())
			using (var t = session.BeginTransaction())
			{
				var child = GetTransientProxy(session, id);
				Assert.That(ForeignKeys.IsTransientSlow(typeof(EntityAssigned).FullName, child, session.GetSessionImplementation()), Is.True);
				t.Commit();
			}
		}

		private static EntityAssigned GetTransientProxy(ISession session, int id)
		{
			EntityAssigned e;
			e = session.Load<EntityAssigned>(id);
			e.Name = "b";
			session.Delete(e);
			session.Flush();
			Assert.That(e.IsProxy(), Is.True, "Failed test set up");
			return e;
		}
	}
}
