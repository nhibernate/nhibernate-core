using System;
using System.Collections;

using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using NHibernate.DomainModel;

using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	/// <summary>
	/// Summary description for ConfigurationFixture.
	/// </summary>
	[TestFixture]
	public class ConfigurationFixture
	{
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

		/// <summary>
		/// Recieved sample code that Configuration could not be configured manually.  It can be configured
		/// manually just need to set all of the properties before adding classes
		/// </summary>
		[Test]
		public void ManualConfiguration() 
		{
			//log4net.Config.DOMConfigurator.ConfigureAndWatch( new FileInfo("log4net.cfg.xml") ); //use xml file instead of config
			Configuration cfg = new Configuration();
			IDictionary props = new Hashtable();

			props["hibernate.connection.provider"] = "NHibernate.Connection.DriverConnectionProvider"; 
			props["hibernate.dialect" ] = "NHibernate.Dialect.MsSql2000Dialect"; 
			props["hibernate.connection.driver_class" ] = "NHibernate.Driver.SqlClientDriver" ;
			props["hibernate.connection.connection_string"] = "Server=localhost;initial catalog=nhibernate;Integrated Security=SSPI" ;
		
			foreach( DictionaryEntry de in props ) 
			{
				cfg.SetProperty( de.Key.ToString(), de.Value.ToString() );
			}

			cfg.AddClass( typeof(Simple) );

			new SchemaExport( cfg ).Create( true, true );

			ISessionFactory factory = cfg.BuildSessionFactory();

		}

	}
}
