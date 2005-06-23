using System.Reflection;

using NHibernate;
using NHibernate.Cfg;
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
			cfg.AddResource( "NHibernate.Test.NHSpecificTest.NH251.CustomAccessDO.hbm.xml",
				Assembly.GetExecutingAssembly() );

			ISessionFactory factory = cfg.BuildSessionFactory();
			cfg.GenerateSchemaCreationScript( factory.Dialect );
		}
	}
}