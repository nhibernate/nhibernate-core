using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.IlogsProxyTest
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		private Guid _entityWithClassProxy1Id;
		private Guid _entityWithClassProxy2Id;
		private Guid _entityWithInterfaceProxy1Id;
		private Guid _entityWithInterfaceProxy2Id;
		private Guid _entityWithClassLookupId;
		private Guid _entityWithInterfaceLookupId;
		
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityWithClassProxyDefinition>(rc =>
			{
				rc.Proxy(typeof(EntityWithClassProxyDefinition));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithInterfaceProxyDefinition>(rc =>
			{
				rc.Proxy(typeof(IEntityProxy));

				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityWithClassLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x  => x.Class(typeof(EntityWithClassProxyDefinition)));
			});

			mapper.Class<EntityWithInterfaceLookup>(rc =>
			{
				rc.Id(x => x.Id);
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.EntityLookup, x => x.Class(typeof(EntityWithInterfaceProxyDefinition)));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}


		protected override void OnSetUp()
		{
			using(var session = OpenSession())
			{
				var entityCP1 = new EntityWithClassProxyDefinition
								{
									Id = Guid.NewGuid(),
									Name = "Name 1"
								};
				_entityWithClassProxy1Id = entityCP1.Id;

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
				_entityWithInterfaceProxy1Id = entityIP1.Id;

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
									EntityLookup = (EntityWithClassProxyDefinition)session.Load(typeof(EntityWithClassProxyDefinition), entityCP1.Id)
								};
				_entityWithClassLookupId = entityCL.Id;

				var entityIL = new EntityWithInterfaceLookup
								{
									Id = Guid.NewGuid(),
									Name = "Name 1",
									EntityLookup = (IEntityProxy)session.Load(typeof(EntityWithInterfaceProxyDefinition), entityIP1.Id)
								};
				_entityWithInterfaceLookupId = entityIL.Id;

				session.Save(entityCL);
				session.Save(entityIL);

				session.Flush();
			}
		}


		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Delete("from System.Object");

					session.Flush();
					transaction.Commit();
				}
			}
		}


		[Test]
		public void UpdateEntityWithClassLookup()
		{
			using (var session = OpenSession())
			{
				using(var transaction = session.BeginTransaction())
				{
					var entityToUpdate = session.Query<EntityWithClassLookup>()
												.First(e => e.Id == _entityWithClassLookupId);

					entityToUpdate.EntityLookup = (EntityWithClassProxyDefinition)session.Load(typeof(EntityWithClassProxyDefinition), _entityWithClassProxy2Id);

					session.Update(entityToUpdate);
					session.Flush();
					transaction.Rollback();
				}
			}
		}


		[Test]
		public void UpdateEntityWithInterfaceLookup()
		{
			using(var session = OpenSession())
			{
				using(var transaction = session.BeginTransaction())
				{
					var entityToUpdate = session.Query<EntityWithInterfaceLookup>()
												.First(e => e.Id == _entityWithInterfaceLookupId);

					entityToUpdate.EntityLookup = (IEntityProxy)session.Load(typeof(EntityWithInterfaceProxyDefinition), _entityWithInterfaceProxy2Id);

					session.Update(entityToUpdate);
					session.Flush();
					transaction.Rollback();
				}
			}
		}
	}
}
