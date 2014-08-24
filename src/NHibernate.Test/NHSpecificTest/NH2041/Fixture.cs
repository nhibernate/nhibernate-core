using System.Linq;
using NUnit.Framework;
using NHibernate.Cfg;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2041
{
	public class Fixture
	{
		[Test]
		public void WhenJoinTableContainComponentsThenColumnsShouldBeInJoinedTable()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2041.Mappings.hbm.xml", GetType().Assembly);
			var mappings = cfg.CreateMappings(Dialect.Dialect.GetDialect(cfg.Properties));
			var table = mappings.GetTable(null, null, "Locations");
			table.Should().Not.Be.Null();
			table.ColumnIterator.Select(c => c.Name).Should().Have.SameValuesAs("myclassId", "latitudecol", "longitudecol");
		}
	}
}