using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3992
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void Test_MappedSubclass_AllMapsAtTopLevel()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(BaseEntityMapping));
			mapper.AddMapping(typeof(MappedEntitySubclassMapping));

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			// Check that the subclass was mapped
			var baseMapping = mapping.RootClasses.SingleOrDefault(x => x.Name == "BaseEntity");
			Assert.IsNotNull(baseMapping, "Mapping did not return mapping for BaseEntity");
			var targetMapping = mapping.SubClasses.SingleOrDefault(x => x.Name == "MappedEntity");
			Assert.IsNotNull(targetMapping, "Mapping did not return mapping for subclass MappedEntity");
			// Check that all three fields are mapped
			var baseProperty = baseMapping.Properties.SingleOrDefault(p => p.Name == "BaseField");
			Assert.IsNotNull(baseProperty, "Base class mapping did not map base class property");
			var extendedProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "ExtendedField");
			Assert.IsNotNull(extendedProperty, "Sub class mapping did not map extended (class not mapped) class property");
			var topLevelProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "TopLevelField");
			Assert.IsNotNull(topLevelProperty, "Sub class mapping did not map base class property");

		}

		[Test]
		public void Test_MappedJoinedSubclass_AllMapsAtTopLevel()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(BaseEntityMapping));
			mapper.AddMapping(typeof(MappedEntityJoinedSubclassMapping));

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			// Check that the subclass was mapped
			var baseMapping = mapping.RootClasses.SingleOrDefault(x => x.Name == "BaseEntity");
			Assert.IsNotNull(baseMapping, "Mapping did not return mapping for BaseEntity");
			var targetMapping = mapping.JoinedSubclasses.SingleOrDefault(x => x.Name == "MappedEntity");
			Assert.IsNotNull(targetMapping, "Mapping did not return mapping for subclass MappedEntity");
			// Check that all three fields are mapped
			var baseProperty = baseMapping.Properties.SingleOrDefault(p => p.Name == "BaseField");
			Assert.IsNotNull(baseProperty, "Base class mapping did not map base class property");
			var extendedProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "ExtendedField");
			Assert.IsNotNull(extendedProperty, "Sub class mapping did not map extended (class not mapped) class property");
			var topLevelProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "TopLevelField");
			Assert.IsNotNull(topLevelProperty, "Sub class mapping did not map base class property");

		}

		[Test]
		public void Test_MappedUnionSubclass_AllMapsAtTopLevel()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(BaseEntityMapping));
			mapper.AddMapping(typeof(MappedEntityUnionSubclassMapping));

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			// Check that the subclass was mapped
			var baseMapping = mapping.RootClasses.SingleOrDefault(x => x.Name == "BaseEntity");
			Assert.IsNotNull(baseMapping, "Mapping did not return mapping for BaseEntity");
			var targetMapping = mapping.UnionSubclasses.SingleOrDefault(x => x.Name == "MappedEntity");
			Assert.IsNotNull(targetMapping, "Mapping did not return mapping for subclass MappedEntity");
			// Check that all three fields are mapped
			var baseProperty = baseMapping.Properties.SingleOrDefault(p => p.Name == "BaseField");
			Assert.IsNotNull(baseProperty, "Base class mapping did not map base class property");
			var extendedProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "ExtendedField");
			Assert.IsNotNull(extendedProperty, "Sub class mapping did not map extended (class not mapped) class property");
			var topLevelProperty = targetMapping.Properties.SingleOrDefault(p => p.Name == "TopLevelField");
			Assert.IsNotNull(topLevelProperty, "Sub class mapping did not map base class property");

		}
	}
}