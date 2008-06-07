using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH251
{
	[TestFixture]
	public class CustomAccessFixture
	{
		[Test]
		public void ConfigurationIsOK()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH251.CustomAccessDO.hbm.xml",
			                Assembly.GetExecutingAssembly());

			ISessionFactoryImplementor factory = (ISessionFactoryImplementor)cfg.BuildSessionFactory();
			cfg.GenerateSchemaCreationScript(factory.Dialect);
		}
	}
}