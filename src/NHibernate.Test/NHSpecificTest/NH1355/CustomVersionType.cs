using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1355
{
	//http://nhibernate.jira.com/browse/NH-1355
	//http://nhibernate.jira.com/browse/NH-1377
	//http://nhibernate.jira.com/browse/NH-1379
	[TestFixture]
	public class CustomVersionType
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

		[Test]
		public void BugSubTask()
		{
			Configuration cfg = new Configuration();
			Assembly domain = typeof(Category).Assembly;
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH1355.CategoryTD.hbm.xml", domain);

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