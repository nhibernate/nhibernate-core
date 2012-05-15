using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;


namespace NHibernate.Test.NHSpecificTest.NH3148
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CanMapComponentAsIdWhenComnponentIsDeclaredInBaseClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.ComponentAsId(x => x.ComponentId, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			componentId.Should().Not.Be.Null();

			var key = componentId.Items[0] as HbmKeyProperty;
			key.Columns.Single().name.Should().Match("IdColumn");
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
			componentId.Should().Be.Null();

			var id = entity.Id;
			id.Should().Not.Be.Null();
		}

		[Test]
		public void CanMapComponentAsIdWhenComnponentIsDeclaredInClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityBase>(rc =>
			{
				rc.ComponentAsId(x => x.ComponentId, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			componentId.Should().Not.Be.Null();

			var key = componentId.Items[0] as HbmKeyProperty;
			key.Columns.Single().name.Should().Match("IdColumn");
		}
	}
}