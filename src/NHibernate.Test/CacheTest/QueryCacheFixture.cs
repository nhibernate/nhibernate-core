using System.Collections;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class QueryCacheFixture : TestCase
	{
		protected override string[] Mappings => new[] { "Simple.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseQueryCache, "true");
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				s.Save(new Simple(), 1L);
				s.Flush();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				s.Delete("from Simple");
				s.Flush();
			}
		}

		[Test]
		public void QueryCacheWithNullParameters()
		{
			using (var s = OpenSession())
			{
				const string query = "from Simple s where s = :s or s.Name = :name or s.Address = :address";
				s
					.CreateQuery(query)
					.SetEntity("s", s.Load(typeof(Simple), 1L))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				// Run a second time, just to test the query cache
				var result = s
					.CreateQuery(query)
					.SetEntity("s", s.Load(typeof(Simple), 1L))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				Assert.That(result, Is.Not.Null);
				Assert.That(s.GetIdentifier(result), Is.EqualTo(1));
			}
		}

		[Test]
		public void QueryCacheWithScalarReturn()
		{
			// Using decimal because:
			//  - int is yielded back as decimal by Oracle, wrecking the cast back to int.
			//  - Oracle requires a cast to binary_double for yielding a double instead of decimal.
			//  - double is not castable in MySql.
			// So long for SQLite which does not have a true decimal type.
			if (TestDialect.HasBrokenDecimalType)
				Assert.Ignore("Database does not support properly decimals.");

			using (var s = OpenSession())
			{
				var result = s
					.CreateSQLQuery("select cast(200012 as decimal) from Simple where id_ = 1")
					.SetCacheable(true)
					.UniqueResult<decimal>();

				Assert.That(result, Is.EqualTo(200012), "Unexpected non-cached result");

				result = s
					.CreateSQLQuery("select cast(200012 as decimal) from Simple where id_ = 1")
					.SetCacheable(true)
					.UniqueResult<decimal>();

				Assert.That(result, Is.EqualTo(200012), "Unexpected cached result");
			}
		}
	}
}
