using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaExportTests
{
	[TestFixture]
	public class WithColumnTagFixture
	{
		[Test]
		public void ShouldCreateSchemaWithDefaultClause()
		{
			var script = new StringBuilder();
			const string mapping = "NHibernate.Test.Tools.hbm2ddl.SchemaExportTests.WithColumnTag.hbm.xml";

			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(mapping))
				cfg.AddInputStream(stream);
			new SchemaExport(cfg).Execute(s => script.AppendLine(s), false, false);

			string wholeScript = script.ToString();
			Assert.That(wholeScript, Is.StringContaining("default SYSTEM_USER"));
			Assert.That(wholeScript, Is.StringContaining("default 77"));
		}
	}
}