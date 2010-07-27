using System.Text;
using NHibernate.Cfg;
using NHibernate.Dialect;
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
	}
}