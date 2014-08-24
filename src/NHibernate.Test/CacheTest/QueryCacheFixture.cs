using System;
using System.Collections;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class QueryCacheFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {"Simple.hbm.xml"}; }
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.UseQueryCache, "true");
		}

		[Test]
		public void QueryCacheWithNullParameters()
		{
			Simple simple = new Simple();

			using (ISession s = OpenSession())
			{
				s.Save(simple, 1L);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				s
					.CreateQuery("from Simple s where s = :s or s.Name = :name or s.Address = :address")
					.SetEntity("s", s.Load(typeof(Simple), 1L))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				// Run a second time, just to test the query cache
				object result = s
					.CreateQuery("from Simple s where s = :s or s.Name = :name or s.Address = :address")
					.SetEntity("s", s.Load(typeof(Simple), 1L))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				Assert.IsNotNull(result);
				Assert.AreEqual(1L, (long) s.GetIdentifier(result));
			}

			using (ISession s = OpenSession())
			{
				s.Delete("from Simple");
				s.Flush();
			}
		}
	}
}