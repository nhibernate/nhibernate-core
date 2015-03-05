using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2003
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ShouldCreateNotNullIdColumn()
		{
			StringBuilder script = new StringBuilder();

			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();

			string ns = GetType().Namespace;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ns + ".Mappings.hbm.xml"))
				cfg.AddInputStream(stream);

			new SchemaExport(cfg).Execute(s => script.AppendLine(s), false, false);

			string wholeScript = script.ToString();
			Assert.That(wholeScript, Is.StringContaining("not null").IgnoreCase);
		}
	}
}
