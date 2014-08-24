using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2003
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ShouldCreateNotNullIdColumn()
		{
			StringBuilder script = new StringBuilder();

			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(MappingsAssembly + "." + (string)Mappings[0]))
				cfg.AddInputStream(stream);
			new SchemaExport(cfg).Execute(s => script.AppendLine(s), false, false);

			string wholeScript = script.ToString();
			Assert.That(wholeScript.ToLower(), Is.StringContaining("not null"));
		}
	}
}
