using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2931
{
	[TestFixture]
	public class MappingByCodeTest
	{
		[Test]
		public void CompiledMappings_ShouldNotDependOnAddedOrdering_AddedBy_AddMapping()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<AMapping>();
			mapper.AddMapping<BMapping>();
			mapper.AddMapping<CMapping>();
			mapper.AddMapping<DMapping>();
			mapper.AddMapping<EMapping>();
			mapper.AddMapping<FMapping>();
			mapper.AddMapping<GMapping>();
			var config = TestConfigurationHelper.GetDefaultConfiguration();
			Assert.DoesNotThrow(() => config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities()));
		}

		[Test]
		public void CompiledMappings_ShouldNotDependOnAddedOrdering_AddedBy_AddMappings()
		{
			var mappings =
				typeof(MappingByCodeTest)
					.Assembly
					.GetExportedTypes()
					//only add our test entities/mappings
					.Where(t => t.Namespace == typeof(MappingByCodeTest).Namespace && t.Name.EndsWith("Mapping"));
			var mapper = new ModelMapper();
			mapper.AddMappings(mappings);
			var config = TestConfigurationHelper.GetDefaultConfiguration();
			Assert.DoesNotThrow(() => config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities()));
		}
	}
}
