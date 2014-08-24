using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3105
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CanMapComponentAsIdWhenComponentIsDeclaredInBaseClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.ComponentAsId(x => x.ComponentId, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			Assert.That(componentId, Is.Not.Null);

			var key = componentId.Items[0] as HbmKeyProperty;
			Assert.That(key.Columns.Single().name, Is.EqualTo("IdColumn"));
		}


		[Test]
		public void CanMapIdWhenIdIsDeclaredInBaseClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Column("IdColumn"));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			Assert.That(componentId, Is.Null);

			var id = entity.Id;
			Assert.That(id, Is.Not.Null);
		}

		[Test]
		public void CanMapComponentAsIdWhenComponentIsDeclaredInClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityBase>(rc =>
			{
				rc.ComponentAsId(x => x.ComponentId, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			Assert.That(componentId, Is.Not.Null);

			var key = componentId.Items[0] as HbmKeyProperty;
			Assert.That(key.Columns.Single().name, Is.EqualTo("IdColumn"));
		}
	}
}