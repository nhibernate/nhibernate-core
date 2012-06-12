using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3160
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class MappingWithUniqueTests : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m => m.Unique(true));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void TestThatScriptGenerationForDialectWithoutUniqueSucceeds()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
		    configuration.AddMapping(GetMappings());
			
            var script = configuration.GenerateSchemaCreationScript(new DialectNotSupportingUniqueKeyword());

            Assert.That(script.Length > 0);
		}
	}
}