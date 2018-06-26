using System.Linq;
using NUnit.Framework;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH2041
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void WhenJoinTableContainComponentsThenColumnsShouldBeInJoinedTable()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2041.Mappings.hbm.xml", GetType().Assembly);
			var mappings = cfg.CreateMappings();
			var table = mappings.GetTable(null, null, "Locations");
			Assert.That(table, Is.Not.Null);
			Assert.That(table.ColumnIterator.Select(c => c.Name), Is.EquivalentTo(new [] {"myclassId", "latitudecol", "longitudecol"}));
		}
	}
}
