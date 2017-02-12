using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3153
{
	[TestFixture]
	public class Fixture
	{
		// Regardless of whether the schema is specified in the <class> or
		// the <nhibernate-mapping> element, we should get it into the generator
		// properties, and it should be _consistently_ unquoted to make sure
		// that generator from different classes will match up to the same table.

		[Test]
		public void ShouldGetIdentifierSchemaFromClassElement()
		{
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			conf.AddResource("NHibernate.Test.NHSpecificTest.NH3153.SchemaInClass.hbm.xml", typeof(A).Assembly);

			var mappings = conf.CreateMappings(Dialect.Dialect.GetDialect());

			var pc = mappings.GetClass(typeof(A).FullName);
			Assert.That(((SimpleValue)pc.Identifier).IdentifierGeneratorProperties["schema"], Is.EqualTo("Test"));
		}


		[Test]
		public void ShouldGetIdentifierSchemaFromMappingElement()
		{
			var conf = TestConfigurationHelper.GetDefaultConfiguration();
			conf.AddResource("NHibernate.Test.NHSpecificTest.NH3153.SchemaInMapping.hbm.xml", typeof(A).Assembly);

			var mappings = conf.CreateMappings(Dialect.Dialect.GetDialect());

			var pc = mappings.GetClass(typeof(A).FullName);
			Assert.That(((SimpleValue)pc.Identifier).IdentifierGeneratorProperties["schema"], Is.EqualTo("Test"));
		}
	}
}
