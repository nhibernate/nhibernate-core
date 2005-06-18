using System.Reflection;

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

			cfg.BuildSessionFactory();
			cfg.GenerateSchemaCreationScript( new Dialect.MsSql2000Dialect() );
		}
	}
}