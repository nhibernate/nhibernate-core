using System;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.StaticProxyTest.InterfaceHandling
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		private Guid _id = Guid.NewGuid();

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			#region Subclass hierarchy

			mapper.Class<EntityClassProxy>(
				rc =>
				{
					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
				});

			mapper.UnionSubclass<SubEntityInterfaceProxy>(
				rc =>
				{
					rc.Proxy(typeof(ISubEntityProxy));

					rc.Property(x => x.AnotherName);
				});

			mapper.UnionSubclass<AnotherSubEntityInterfaceProxy>(
				rc =>
				{
					rc.Proxy(typeof(IAnotherSubEntityProxy));

					rc.Property(x => x.AnotherName);
				});

			mapper.Class<EntityWithSuperClassInterfaceLookup>(
				rc =>
				{
					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
					rc.ManyToOne(x => x.EntityLookup, x => x.Class(typeof(EntityClassProxy)));
				});

			#endregion Subclass hierarchy

			mapper.Class<EntitySimple>(
				rc =>
				{

					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
				});

			mapper.Class<EntityExplicitInterface>(
				rc =>
				{

					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
				});

			mapper.Class<EntityMultiInterfaces>(
				rc =>
				{

					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
				});

			mapper.Class<EntityMixExplicitImplicitInterface>(
				rc =>
				{

					rc.Id(x => x.Id);
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void ProxyForBaseSubclassCanBeCreated()
		{
			using (var session = OpenSession())
			{
				var entity = session.Load<EntityClassProxy>(_id);
			}
		}
		
		//This test is passed because of wrong proxy generation (generated proxy is not based on EntityClassProxy)
		[Test]
		public void ProxyForBaseSubclassByInterface()
		{
			using (var session = OpenSession())
			{
				var entity = (IEntity) session.Load(typeof(EntityClassProxy), _id);
				ThrowOnIdAccess(entity);
			}
		}

		[Test]
		public void ProxyForEntityImplicitInterface()
		{
			using (var session = OpenSession())
			{
				var entity = (IEntity) session.Load<EntitySimple>(_id);
				ThrowOnIdAccess(entity);
				ThrowOnNameAccess(entity);

				var multiInterface = session.Load<EntityMultiInterfaces>(_id);
				ThrowOnIdAccess(multiInterface);
				ThrowOnIdAccessSecond(multiInterface);
			}
		}

		[Test]
		public void ProxyForEntityInitializeOnExplicitInterface()
		{
			using (var session = OpenSession())
			{
				var entity = session.Load<EntityExplicitInterface>(_id);
				ThrowOnIdAccess(entity);
			}
		}

		[Test]
		public void ProxyForEntityWithMixedImplicitExplicitInterfacesAreHandledCorrectly()
		{
			using (var session = OpenSession())
			{
				var entity = session.Load<EntityMixExplicitImplicitInterface>(_id);
				ThrowOnIdAccessSecond(entity);
				ThrowOnIdAccess(entity);
			}
		}

		private void ThrowOnNameAccess(IEntity entity)
		{
			Assert.That(() => entity.Name, Throws.TypeOf<ObjectNotFoundException>(), "Non Id interface access should lead to proxy initialization");
		}

		private void ThrowOnIdAccess(IEntity entity)
		{
			Assert.That(() => entity.Id, Throws.TypeOf<ObjectNotFoundException>(), "IEntity.Id access should lead to proxy initialization");
		}

		private void ThrowOnIdAccessSecond(IEntityId entity)
		{
			Assert.That(() => entity.Id, Throws.TypeOf<ObjectNotFoundException>(), "IEntityId.Id access should lead to proxy initialization");
		}

		private void CanAccessId(IEntity entity)
		{
			Assert.That(() => entity.Id, Throws.Nothing, "Failed to access proxy IEntity.Id interface");
			Assert.That(entity.Id, Is.EqualTo(_id));
		}

		private void CanAccessSecond(IEntityId entity)
		{
			Assert.That(() => entity.Id, Throws.Nothing, "Failed to access proxy IEntityId.Id interface");
			Assert.That(entity.Id, Is.EqualTo(_id));
		}
	}
}
