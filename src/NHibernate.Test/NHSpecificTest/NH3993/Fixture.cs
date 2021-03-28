using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.MappingByCode.IntegrationTests.NH2825;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3993
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		[Test]
		public void Test_MapPrivateComponentProperty_MapsCorrectly()
		{
			var mapper = new ModelMapper();

			mapper.AddMapping(typeof(BaseEntityMapper));
			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			// The component
			var componentPropertyMapping = mapping.RootClasses[0]
				.Properties.SingleOrDefault(x => x.Name == "Component") as HbmComponent;
			Assert.IsNotNull(componentPropertyMapping, "Component did not correctly map");
			// Component parent
			Assert.IsNotNull(componentPropertyMapping.Parent, "Component did not correctly map parent");
			// Component element
			var elementListMapping = componentPropertyMapping.Properties.SingleOrDefault(p => p.Name == "_elements") as HbmMap;
			Assert.IsNotNull(elementListMapping, "Component did not map element list");
			var elementMapping = elementListMapping.ElementRelationship as HbmCompositeElement;
			Assert.IsNotNull(elementMapping, "Component did not map elements");
			// Component element property
			var privateProperty = elementMapping.Properties.SingleOrDefault(p => p.Name == "_name");
			Assert.IsNotNull(privateProperty, "Component Element did not map private property");
			// Component element parent
			Assert.IsNotNull(elementMapping.Parent, "Component Element did not map parent");
			// Component element relation
			var relationproperty = elementMapping.Properties.SingleOrDefault(p => p.Name == "_description");
			Assert.IsNotNull(relationproperty, "Component Element did not map one to many relationship");
			// Component element component
			var componentMapping = elementMapping.Properties.SingleOrDefault(p => p.Name == "_component");
			Assert.IsNotNull(componentMapping, "Component Element did not private component");
		}

		[Test]
		public void Test_InvalidPrivateProperty_ThrowsException()
		{
			var mapper = new ModelMapper();

			Assert.Throws<MappingException>(() => mapper.AddMapping(typeof(InvalidPropertyMapper)));
		}

		[Test]
		public void Test_ManyToOneHasInvalidType_ThrowsException()
		{
			var mapper = new ModelMapper();

			Assert.Throws<MappingException>(() => mapper.AddMapping(typeof(InvalidRelationshipMapper)));
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
