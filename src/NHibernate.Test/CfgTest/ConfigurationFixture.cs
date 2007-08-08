using System;
using System.Collections;
using System.IO;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.CfgTest
{
	/// <summary>
	/// Summary description for ConfigurationFixture.
	/// </summary>
	[TestFixture]
	public class ConfigurationFixture
	{
		/// <summary>
		/// Verify that NHibernate can read the configuration from any hibernate.cfg.xml
		/// file and that the values override what is in the app.config.
		/// </summary>
		[Test]
		public void ReadCfgXmlFromDefaultFile()
		{
			Configuration cfg = new Configuration();
			cfg.Configure("TestEnbeddedConfig.cfg.xml");

			Assert.IsTrue(cfg.Properties.Contains(Environment.ShowSql));
			Assert.IsTrue(cfg.Properties.Contains(Environment.UseQueryCache));
			Assert.IsTrue(cfg.Properties.Contains(Environment.PrepareSql));
			Assert.IsTrue(cfg.Properties.Contains(Environment.Isolation));
			Assert.AreEqual("true 1, false 0, yes 1, no 0", cfg.Properties[Environment.QuerySubstitutions]);
			Assert.AreEqual("Server=localhost;initial catalog=nhibernate;User Id=;Password=",
			                cfg.Properties[Environment.ConnectionString]);
		}

		/// <summary>
		/// Verify that NHibernate can read the configuration from any hibernate.cfg.xml
		/// file and that the values ignore what is in the app.config.
		/// </summary>
		/// <remarks>
		/// This test run Explicit because clean the NHibernate.Cfg.Environment.
		/// </remarks>
		[Test, Explicit]
		public void ReadCfgXmlIgnorigAppConfig()
		{
			Configuration cfg = new Configuration();
			cfg.ConfigureIgnoringAppConfig("TestEnbeddedConfig.cfg.xml");

			Assert.IsFalse(cfg.Properties.Contains(Environment.ShowSql));
			Assert.IsFalse(cfg.Properties.Contains(Environment.UseQueryCache));
			Assert.IsFalse(cfg.Properties.Contains(Environment.PrepareSql));
			Assert.IsFalse(cfg.Properties.Contains(Environment.Isolation));
			Assert.AreEqual("true 1, false 0, yes 1, no 0", cfg.Properties[Environment.QuerySubstitutions]);
			Assert.AreEqual("Server=localhost;initial catalog=nhibernate;User Id=;Password=",
											cfg.Properties[Environment.ConnectionString]);
		}

		/// <summary>
		/// Recieved sample code that Configuration could not be configured manually.  It can be configured
		/// manually just need to set all of the properties before adding classes
		/// </summary>
		[Test, Explicit]
		public void ManualConfiguration()
		{
			//log4net.Config.DOMConfigurator.ConfigureAndWatch( new FileInfo("log4net.cfg.xml") ); //use xml file instead of config
			Configuration cfg = new Configuration();
			IDictionary props = new Hashtable();

			props[Environment.ConnectionProvider] = "NHibernate.Connection.DriverConnectionProvider";
			props[Environment.Dialect] = "NHibernate.Dialect.MsSql2000Dialect";
			props[Environment.ConnectionDriver] = "NHibernate.Driver.SqlClientDriver";
			props[Environment.ConnectionString] =
				"Server=localhost;initial catalog=nhibernate;Integrated Security=SSPI";

			foreach (DictionaryEntry de in props)
			{
				cfg.SetProperty(de.Key.ToString(), de.Value.ToString());
			}

			cfg.AddClass(typeof(Simple));

			new SchemaExport(cfg).Create(true, true);

			ISessionFactory factory = cfg.BuildSessionFactory();
		}

		/// <summary>
		/// Verify that NHibernate can read the configuration from a manifest resource in an
		/// Assembly and that the values override what is in the app.config.
		/// </summary>
		[Test]
		public void ReadCfgXmlFromAssembly()
		{
			Configuration cfg = new Configuration();
			cfg.Configure(this.GetType().Assembly, "NHibernate.Test.TestEnbeddedConfig.cfg.xml");

			Assert.AreEqual("true 1, false 0, yes 1, no 0", cfg.Properties[Environment.QuerySubstitutions]);
			Assert.AreEqual("Server=localhost;initial catalog=nhibernate;User Id=;Password=",
			                cfg.Properties[Environment.ConnectionString]);
		}

		/// <summary>
		/// Verify that NHibernate properly releases resources when an Exception occurs
		/// during the reading of config files.
		/// </summary>
		[Test]
		public void InvalidXmlInCfgFile()
		{
			XmlDocument cfgXml = new XmlDocument();
			cfgXml.Load("TestEnbeddedConfig.cfg.xml");

			// this should put us at the first <property> element
			XmlElement propElement = cfgXml.DocumentElement.GetElementsByTagName("property")[0] as XmlElement;

			// removing this will cause it not to validate
			propElement.RemoveAttribute("name");

			cfgXml.Save("hibernate.invalid.cfg.xml");

			Configuration cfg = new Configuration();
			try
			{
				cfg.Configure("hibernate.invalid.cfg.xml");
			}
			catch (HibernateException)
			{
				// just absorb it - not what we are testing
			}
			finally
			{
				// clean up the bad file - if the Configure method cleans up after
				// itself we should be able to do this without problem.  If it does
				// property release the resource then this won't be able to access
				// the file to delete.
				File.Delete("hibernate.invalid.cfg.xml");
			}
		}

		[Test]
		public void EmptyPropertyTag()
		{
			string xml =
				@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
	<session-factory name='NHibernate.Test'>
		<property name='connection.provider'></property>
	</session-factory>
</hibernate-configuration>";

			XmlDocument cfgXml = new XmlDocument();
			cfgXml.LoadXml(xml);

			Configuration cfg = new Configuration();
			XmlTextReader xtr = new XmlTextReader(xml, XmlNodeType.Document, null);
			cfg.Configure(xtr);
		}

		[Test]
		[ExpectedException(typeof(HibernateConfigException))]
		public void CacheConfigurationForUnmappedClass()
		{
			string cfgString =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
								<session-factory>
									<class-cache class='NHibernate.DomainModel.A, NHibernate.DomainModel' usage='read-write' region='xx' />
								</session-factory>
							</hibernate-configuration>";

			Configuration cfg = new Configuration();
			cfg.Configure(new XmlTextReader(cfgString, XmlNodeType.Document, null));
		}

		[Test]
		[ExpectedException(typeof(HibernateConfigException))]
		public void CacheConfigurationForUnmappedCollection()
		{
			string cfgString =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
								<session-factory>
									<mapping resource='NHibernate.DomainModel.ABC.hbm.xml' assembly='NHibernate.DomainModel' />
									<collection-cache collection='NHibernate.DomainModel.B.XX' usage='nonstrict-read-write' region='yy' />
								</session-factory>
							</hibernate-configuration>";

			Configuration cfg = new Configuration();
			cfg.Configure(new XmlTextReader(cfgString, XmlNodeType.Document, null));
		}

		[Test]
		[ExpectedException(typeof(HibernateConfigException))]
		public void NoSessionFactoriesInConfiguration()
		{
			string cfgString = @"<?xml version='1.0' encoding='utf-8' ?><someElement />";

			Configuration cfg = new Configuration();
			cfg.Configure(new XmlTextReader(cfgString, XmlNodeType.Document, null));
		}

		[Test]
		public void CacheConfiguration()
		{
			string cfgString =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
								<session-factory>
									<mapping resource='NHibernate.DomainModel.ABC.hbm.xml' assembly='NHibernate.DomainModel' />
									<class-cache class='NHibernate.DomainModel.A, NHibernate.DomainModel' usage='read-write' region='xx' />
									<collection-cache collection='NHibernate.DomainModel.B.Map' usage='nonstrict-read-write' region='yy' />
								</session-factory>
							</hibernate-configuration>";

			Configuration cfg = new Configuration();
			cfg.Configure(new XmlTextReader(cfgString, XmlNodeType.Document, null)).BuildSessionFactory();
		}

		[Test]
		public void InvalidXmlInHbmFile()
		{
			string filename = "invalid.hbm.xml";
			// it's missing the class name - won't validate
			string hbm =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
								<class table='a'></class>
							</hibernate-mapping>";
			XmlDocument hbmDoc = new XmlDocument();
			hbmDoc.LoadXml(hbm);
			hbmDoc.Save(filename);

			Configuration cfg = new Configuration();
			try
			{
				cfg.Configure();
				cfg.AddXmlFile("invalid.hbm.xml");
			}
			catch (HibernateException)
			{
				// just absorb it - not what we are testing
			}
			finally
			{
				// clean up the bad file - if the AddXmlFile method cleans up after
				// itself we should be able to do this without problem.  If it does
				// property release the resource then this won't be able to access
				// the file to delete.
				File.Delete(filename);
			}
		}

		[Test]
		public void ProxyWithDefaultNamespaceAndAssembly()
		{
			string hbm =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'
								namespace='NHibernate.DomainModel'
								assembly='NHibernate.DomainModel'>
								<class name='A' proxy='A'>
									<id name='Id'>
										<generator class='native' />
									</id>
								</class>
							</hibernate-mapping>";

			Configuration cfg = new Configuration();
			cfg.AddXmlString(hbm)
				.BuildSessionFactory();
		}

		[Test]
		public void PersisterWithDefaultNamespaceAndAssembly()
		{
			string hbm =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'
								namespace='NHibernate.DomainModel'
								assembly='NHibernate.DomainModel'>
								<class name='A' persister='A'>
									<id name='Id'>
										<generator class='native' />
									</id>
								</class>
							</hibernate-mapping>";

			Configuration cfg = new Configuration();
			cfg.AddXmlString(hbm); //.BuildSessionFactory();
		}

		[Test]
		public void AddDocument()
		{
			string hbm =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='NHibernate.DomainModel.A, NHibernate.DomainModel'>
		<id name='Id' column='somecolumn'>
			<generator class='native' />
		</id>
	</class>
</hibernate-mapping>";

			Configuration cfg = new Configuration();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(hbm);
			cfg.AddDocument(doc);
		}

		[Test]
		public void ProxyValidator()
		{
			string hbm =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='NHibernate.DomainModel.NHSpecific.InvalidProxyClass, NHibernate.DomainModel'
		lazy='true'>
		<id name='Id' column='somecolumn'>
			<generator class='native' />
		</id>
	</class>
</hibernate-mapping>";

			Configuration cfg = new Configuration();

			try
			{
				cfg.AddXmlString(hbm).BuildSessionFactory();
				Assert.Fail("Validation should have failed");
			}
			catch (MappingException e)
			{
				Assert.IsTrue(e is InvalidProxyTypeException);
			}
		}

		[Test]
		public void DisabledProxyValidator()
		{
			string hbm =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='NHibernate.DomainModel.NHSpecific.InvalidProxyClass, NHibernate.DomainModel'
		lazy='true'>
		<id name='Id' column='somecolumn'>
			<generator class='native' />
		</id>
	</class>
</hibernate-mapping>";

			Configuration cfg = new Configuration();
			cfg.Properties[Environment.UseProxyValidator] = "false";
			cfg.AddXmlString(hbm).BuildSessionFactory();
		}

		/// <summary>
		/// Verify that setting the default assembly and namespace through
		/// <see cref="Configuration" /> works as intended.
		/// </summary>
		[Test]
		public void SetDefaultAssemblyAndNamespace()
		{
			string hbmFromDomainModel =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='A'>
		<id name='Id' column='somecolumn'>
			<generator class='native' />
		</id>
	</class>
</hibernate-mapping>";

			string hbmFromTest =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='LocatedInTestAssembly' lazy='false'>
		<id name='Id' column='somecolumn'>
			<generator class='native' />
		</id>
	</class>
</hibernate-mapping>";

			Configuration cfg = new Configuration();
			cfg
				.SetDefaultAssembly("NHibernate.DomainModel")
				.SetDefaultNamespace("NHibernate.DomainModel")
				.AddXmlString(hbmFromDomainModel);

			cfg
				.SetDefaultAssembly("NHibernate.Test")
				.SetDefaultNamespace(typeof(LocatedInTestAssembly).Namespace)
				.AddXmlString(hbmFromTest);

			cfg.BuildSessionFactory().Close();
		}
	}
}