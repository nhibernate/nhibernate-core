using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1355
{
	//http://jira.nhibernate.org/browse/NH-1355
	//http://jira.nhibernate.org/browse/NH-1377
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void Bug()
		{
			Configuration cfg = new Configuration();
			Assembly domain = typeof(Category).Assembly;
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1355.Category.hbm.xml", domain);
			
			try
			{
				cfg.BuildSessionFactory();
			}
			catch (MappingException)
			{
				Assert.Fail("Should not throw exception");
			}
		}
	}
}