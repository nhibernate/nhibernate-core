using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;


namespace NHibernate.Test.NHSpecificTest.NH3148
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Entity" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CanMapComponentAsIdWhenComnponentIsDeclaredInBaseClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.ComponentAsId(x => x.Id, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			var componentId = entity.CompositeId;
			componentId.Should().Not.Be.Null();

			var key = componentId.Items[0] as HbmKeyProperty;
			key.Columns.Single().name.Should().Match("IdColumn");
		}

		[Test]
		public void CanMapComponentAsIdWhenComnponentIsDeclaredInDerivedClass()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityBase>(rc =>
			{
				rc.ComponentAsId(x => x.Id, m => m.Property(c => c.Id, pm => pm.Column("IdColumn")));
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