using System.Collections;
using System.Collections.Generic;
using log4net.Config;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Metadata;
using NUnit.Framework;

namespace NHibernate.Caches.MemCache.Tests.NH1086
{
	[TestFixture]
	public class Fixture 
	{
		private MemCacheProvider provider;
		private Dictionary<string,string> props;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			XmlConfigurator.Configure();
			props = new Dictionary<string, string>();
			props.Add("compression_enabled", "false");
			props.Add("expiration", "20");
			provider = new MemCacheProvider();
			provider.Start(props);
		}

		[TestFixtureTearDown]
		public void FixtureStop()
		{
			provider.Stop();
		}

		[Test]
		public void Test()
		{
			Configuration config = new Configuration();
			config.AddResource("NHibernate.Caches.MemCache.Tests.NH1086.Mappings.hbm.xml",
			                   this.GetType().Assembly);
			ISessionFactoryImplementor sessions = null;
			try
			{
				sessions = config.BuildSessionFactory() as ISessionFactoryImplementor;
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debugger.Break();
			}
			IClassMetadata classMetadata = sessions.GetClassMetadata(typeof (ClassWithCompId));

			ICache cache = provider.BuildCache("NH1086", props);
			Assert.IsNotNull(cache, "no cache returned");

			// Create the object to be cached
			ClassWithCompId obj = new ClassWithCompId();
			obj.ID = new CompId("Test object", 1);

			// Create the CacheKey
			CacheKey key = new CacheKey(obj.ID,
										classMetadata.IdentifierType,
										typeof(ClassWithCompId).FullName,
										EntityMode.Poco,
										sessions);

			// Put the object into cache; mainly to test Serialization goes okay
			cache.Put(key, obj);

			// Get the object back from cache
			ClassWithCompId retrievedFromCache = cache.Get(key) as ClassWithCompId;

			Assert.AreNotSame(obj, retrievedFromCache, 
				"Object put into cache and object retrieved from cache should not be the same");
		}
	}
}
