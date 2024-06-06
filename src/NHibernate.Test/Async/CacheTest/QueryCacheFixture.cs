﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	using System.Threading.Tasks;
	[TestFixture]
	public class QueryCacheFixtureAsync : TestCase
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
		public async Task QueryCacheWithNullParametersAsync()
		{
			using (var s = OpenSession())
			{
				const string query = "from Simple s where s = :s or s.Name = :name or s.Address = :address";
				await (s
					.CreateQuery(query)
					.SetEntity("s", await (s.LoadAsync(typeof(Simple), 1L)))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResultAsync());

				// Run a second time, just to test the query cache
				var result = await (s
					.CreateQuery(query)
					.SetEntity("s", await (s.LoadAsync(typeof(Simple), 1L)))
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResultAsync());

				Assert.That(result, Is.Not.Null);
				Assert.That(s.GetIdentifier(result), Is.EqualTo(1));
			}
		}

		[Test]
		public async Task QueryCacheWithScalarReturnAsync()
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
				var result = await (s
					.CreateSQLQuery("select cast(200012 as DECIMAL(18,5)) from Simple where id_ = 1")
					.SetCacheable(true)
					.UniqueResultAsync<decimal>());

				Assert.That(result, Is.EqualTo(200012), "Unexpected non-cached result");

				result = await (s
					.CreateSQLQuery("select cast(200012 as DECIMAL(18,5)) from Simple where id_ = 1")
					.SetCacheable(true)
					.UniqueResultAsync<decimal>());

				Assert.That(result, Is.EqualTo(200012), "Unexpected cached result");
			}
		}
	}
}
