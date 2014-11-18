using NHibernate.Engine.Query.Sql;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2138
{
	[TestFixture]
	public class Fixture 
	{
		[Test]
		public void AfterAddAppingShouldHaveAResultsetWithEntityName()
		{
			var cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH2138.Mappings.hbm.xml", GetType().Assembly);
			Assert.That(() => cfg.BuildMappings(), Throws.Nothing);
			var sqlQuery = cfg.NamedSQLQueries["AllCoders"];
			var rootReturn = (NativeSQLQueryRootReturn)sqlQuery.QueryReturns[0];
			Assert.That(rootReturn.ReturnEntityName, Is.EqualTo("Coder"));
		}
	}
}