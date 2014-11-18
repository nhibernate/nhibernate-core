using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaExportTests
{
	public class AutoQuoteFixture
	{
		[Test]
		public void WhenCalledExplicitlyThenTakeInAccountHbm2DdlKeyWordsSetting()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);
			if(!(dialect is MsSql2000Dialect))
			{
				Assert.Ignore(GetType() + " does not apply to " + dialect);
			}

			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);

			var script = new StringBuilder();
			new SchemaExport(configuration).Execute(s=> script.AppendLine(s), false, false);
			Assert.That(script.ToString(), Is.StringContaining("[Order]").And.Contains("[Select]").And.Contains("[From]").And.Contains("[And]"));
		}

		[Test]
		public void WhenUpdateCalledExplicitlyThenTakeInAccountHbm2DdlKeyWordsSetting()
		{
			var configuration = TestConfigurationHelper.GetDefaultConfiguration();
			var dialect = NHibernate.Dialect.Dialect.GetDialect(configuration.Properties);
			if (!(dialect is MsSql2000Dialect))
			{
				Assert.Ignore(GetType() + " does not apply to " + dialect);
			}

			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			configuration.AddResource("NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest.HeavyEntity.hbm.xml",
																GetType().Assembly);

			var script = new StringBuilder();
			new Tool.hbm2ddl.SchemaUpdate(configuration).Execute(s => script.AppendLine(s), false);

			// With SchemaUpdate the auto-quote method should be called and the conf. should hold quoted stuff
			var cm = configuration.GetClassMapping(typeof(Order));
			var culs = cm.Table.ColumnIterator.ToList();
			Assert.That(cm.Table.IsQuoted, Is.True);
			Assert.That(culs.First(c => "From" == c.Name).IsQuoted, Is.True);
			Assert.That(culs.First(c => "And" == c.Name).IsQuoted, Is.True);
			Assert.That(culs.First(c => "Select" == c.Name).IsQuoted, Is.True);
			Assert.That(culs.First(c => "Column" == c.Name).IsQuoted, Is.True);
		}
	}
}