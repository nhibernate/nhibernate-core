using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Mapping;
using NHibernate.Test.Tools.hbm2ddl.SchemaMetadataUpdaterTest;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using SharpTestsEx;

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
			script.ToString().Should().Contain("[Order]").And.Contain("[Select]").And.Contain("[From]").And.Contain("[And]");
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
			cm.Table.Satisfy(t=> t.IsQuoted);
			culs.First(c => "From".Equals(c.Name)).Satisfy(c=> c.IsQuoted);
			culs.First(c => "And".Equals(c.Name)).Satisfy(c => c.IsQuoted);
			culs.First(c => "Select".Equals(c.Name)).Satisfy(c => c.IsQuoted);
			culs.First(c => "Column".Equals(c.Name)).Satisfy(c => c.IsQuoted);
		}
	}
}