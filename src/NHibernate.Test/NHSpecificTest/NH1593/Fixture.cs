using System.Text;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1593
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void SchemaUpdateAddsIndexesThatWerentPresentYet()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1593.TestIndex.hbm.xml", GetType().Assembly);
			var su = new SchemaUpdate(cfg);
			var sb = new StringBuilder(500);
			su.Execute(x => sb.AppendLine(x), false);
			Assert.That(sb.ToString(), Is.StringContaining("create index test_index_name on TestIndex (Name)"));
		}
	}
}