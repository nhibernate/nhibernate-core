using System;

using NHibernate.Cfg;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	/// <summary>
	/// Summary description for ConfigurationFixture.
	/// </summary>
	[TestFixture]
	public class ConfigurationFixture
	{
		[SetUp]
		public void SetUp() 
		{
			System.IO.File.Copy("..\\..\\hibernate.cfg.xml", "hibernate.cfg.xml", true);
		}

		[TearDown]
		public void TearDown() 
		{
			System.IO.File.Delete("hibernate.cfg.xml");
		}
		
		/// <summary>
		/// Verify that NHibernate can read the configuration from a hibernate.cfg.xml
		/// file and that the values override what is in the app.config.
		/// </summary>
		[Test]
		public void ReadCfgXmlFromDefaultFile() 
		{
			Configuration cfg = new Configuration();
			cfg.Configure();
			
			Assert.AreEqual( "true 1, false 0, yes 'Y', no 'N'", cfg.Properties[Cfg.Environment.QuerySubstitutions]);
			Assert.AreEqual( "Server=localhost;initial catalog=nhibernate;Integrated Security=SSPI", cfg.Properties[Cfg.Environment.ConnectionString]);
		}
	}
}
