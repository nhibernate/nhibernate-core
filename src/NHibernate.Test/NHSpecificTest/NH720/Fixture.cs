using System.Collections.Generic;
using System.Xml;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH720
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH720"; }
		}

		[Test]
		public void CacheTest()
		{
			Dictionary<string, string> properties = new Dictionary<string, string>();
			properties["expiration"] = "60";
			FooCacheProvider.BuildCacheStatic("xx", properties);
			properties["expiration"] = "120";
			FooCacheProvider.BuildCacheStatic("yy", properties);
			string cfgString =
				@"<?xml version='1.0' encoding='utf-8' ?> 
							<hibernate-configuration xmlns='urn:nhibernate-configuration-2.2'>
								<session-factory>
                                    <property name='cache.provider_class'>NHibernate.Test.NHSpecificTest.NH720.FooCacheProvider, NHibernate.Test</property>
									<mapping resource='NHibernate.Test.NHSpecificTest.NH720.Mappings.hbm.xml' assembly='NHibernate.Test' />
									<class-cache class='NHibernate.Test.NHSpecificTest.NH720.A, NHibernate.Test' usage='read-write' region='xx' />
									<collection-cache collection='NHibernate.Test.NHSpecificTest.NH720.A.Bees' usage='nonstrict-read-write' region='yy' />
								</session-factory>
							</hibernate-configuration>";

			Configuration config = new Configuration();
			config.Configure(new XmlTextReader(cfgString, XmlNodeType.Document, null));
			config.BuildSessionFactory();

			Assert.IsFalse(FooCacheProvider.RegionExists("NHibernate.Test.NHSpecificTest.NH720.A"),
			               "Separate region created for class. Configured region was not used.");
			Assert.IsFalse(FooCacheProvider.RegionExists("NHibernate.Test.NHSpecificTest.NH720.A.Bees"),
			               "Separate region created for collection. Configured region was not used.");
			Assert.AreEqual(4, FooCacheProvider.RegionCount);
		}
	}
}