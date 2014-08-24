using NHibernate.Engine.Query.Sql;
using NUnit.Framework;
using SharpTestsEx;

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
			cfg.Executing(c => c.BuildMappings()).NotThrows();
			var sqlQuery = cfg.NamedSQLQueries["AllCoders"];
			var rootReturn = (NativeSQLQueryRootReturn)sqlQuery.QueryReturns[0];
			rootReturn.ReturnEntityName.Should().Be("Coder");
		}
	}
}