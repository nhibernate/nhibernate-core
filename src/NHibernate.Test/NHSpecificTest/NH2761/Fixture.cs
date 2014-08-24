using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2761
{
	public class Fixture
	{
		[Test]
		public void WhenMultipleConfigurationUseSameMappingsThenConstraintsHasSameNames()
		{
			var mappings1 = GetMappings();
			var mappings2 = GetMappings();

			var fkNamesForMappings1 = mappings1.IterateTables.SelectMany(t => t.ForeignKeyIterator).Select(fk => fk.Name).ToArray();
			var fkNamesForMappings2 = mappings2.IterateTables.SelectMany(t => t.ForeignKeyIterator).Select(fk => fk.Name).ToArray();

			fkNamesForMappings1.Should().Have.SameValuesAs(fkNamesForMappings2);
		}

		private Mappings GetMappings()
		{
			var configuration = GetConfiguration();
			return configuration.CreateMappings(Dialect.Dialect.GetDialect(configuration.Properties));
		}

		private Configuration GetConfiguration()
		{
			// for each call will use a differente instance of configuration
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			Assembly thisAssembly = GetType().Assembly;
			configuration.AddResource("NHibernate.Test.NHSpecificTest.NH2761.A.hbm.xml", thisAssembly);
			configuration.AddResource("NHibernate.Test.NHSpecificTest.NH2761.B.hbm.xml", thisAssembly);
			configuration.AddResource("NHibernate.Test.NHSpecificTest.NH2761.C.hbm.xml", thisAssembly);
			return configuration;
		}
	}
}