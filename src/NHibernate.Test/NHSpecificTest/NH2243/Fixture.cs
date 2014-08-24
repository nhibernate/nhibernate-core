using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2243
{
	public class Fixture
	{
			[Test]
			public void ShouldCreateSchemaWithDefaultClause()
			{
				var script = new StringBuilder();
				const string mapping = "NHibernate.Test.NHSpecificTest.NH2243.Mappings.hbm.xml";

				Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
				using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(mapping))
					cfg.AddInputStream(stream);
				new SchemaExport(cfg).Execute(s => script.AppendLine(s), false, false);

				script.ToString().Should().Contain("MyNameForFK");
			}
	}
}