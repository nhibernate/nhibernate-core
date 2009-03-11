using System.IO;
using System.Reflection;
using System.Text;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace NHibernate.Test.Tools.hbm2ddl.SchemaExportTests
{
	[TestFixture]
	public class NonClusteredPrimaryKeyFixture
	{
		[Test]
		public void ShouldCreateSchemaWithNonClusterdPrimaryKeyForMsSql2005Dialect()
		{
			var script = new StringBuilder();
			const string mapping = "NHibernate.Test.Tools.hbm2ddl.SchemaExportTests.WithColumnTag.hbm.xml";

			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();

			if (cfg.Properties[Environment.Dialect] != typeof(MsSql2005Dialect).FullName)
			{
				Assert.Ignore("this test only applies for MsSql2005Dialect");
			}

			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(mapping))
				cfg.AddInputStream(stream);
			new SchemaExport(cfg).Execute(s => script.AppendLine(s), false, false, false);

			string wholeScript = script.ToString();
			Assert.That(wholeScript, Text.Contains("primary key nonclustered (id)"));
		}
	}
}