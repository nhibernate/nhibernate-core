using System.Collections;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Transform;
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
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		private const long SimpleId = 1;

		protected override void OnSetUp()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			s.Save(new Simple(), SimpleId);
			t.Commit();
		}

		protected override void OnTearDown()
		{
			using var s = OpenSession();
			using var t = s.BeginTransaction();
			s.Delete("from Simple");
			t.Commit();
		}

		[Test]
		public void QueryCacheWithNullParameters()
		{
			const string query = "from Simple s where s = :s or s.Name = :name or s.Address = :address";
			Sfi.Statistics.Clear();
			object simple;
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				simple = s.Load(typeof(Simple), SimpleId);
				s
					.CreateQuery(query)
					.SetEntity("s", simple)
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				// Run a second time, just to test the query cache.
				var result = s
					.CreateQuery(query)
					.SetEntity("s", simple)
					.SetString("name", null)
					.SetString("address", null)
					.SetCacheable(true)
					.UniqueResult();

				Assert.That(result, Is.Not.Null);
				Assert.That(s.GetIdentifier(result), Is.EqualTo(SimpleId));
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
				t.Commit();
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

			const string query = "select cast(200012 as decimal) from Simple where id_ = 1";
			Sfi.Statistics.Clear();
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var result = s
					.CreateSQLQuery(query)
					.SetCacheable(true)
					.UniqueResult<decimal>();

				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));
				Assert.That(result, Is.EqualTo(200012), "Unexpected non-cached result");
				t.Commit();
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{

				var result = s
					.CreateSQLQuery(query)
					.SetCacheable(true)
					.UniqueResult<decimal>();

				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
				Assert.That(result, Is.EqualTo(200012), "Unexpected cached result");
			}
		}

		[Test]
		public void QueryCacheWithAliasToBeanTransformer()
		{
			if (!TestDialect.HasActualIntegerTypes)
				Assert.Ignore("Database does not support properly integers.");

			const string query = "select s.id_ as Id from Simple s;";
			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var result1 = s
					.CreateSQLQuery(query)
					.SetCacheable(true)
					.SetResultTransformer(Transformers.AliasToBean<SimpleDTO>())
					.UniqueResult();

				Assert.That(result1, Is.InstanceOf<SimpleDTO>());
				var dto1 = (SimpleDTO) result1;
				Assert.That(dto1.Id, Is.EqualTo(SimpleId), "Unexpected entity Id");
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "Unexpected query cache hit count");
				t.Commit();
			}

			// Run a second time, just to test the query cache.
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				var result2 = s
					.CreateSQLQuery(query)
					.SetCacheable(true)
					.SetResultTransformer(Transformers.AliasToBean<SimpleDTO>())
					.UniqueResult();

				Assert.That(result2, Is.InstanceOf<SimpleDTO>());
				var dto2 = (SimpleDTO) result2;
				Assert.That(dto2.Id, Is.EqualTo(SimpleId), "Unexpected entity Id");
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected query cache hit count");
				t.Commit();
			}
		}

		private class SimpleDTO
		{
			public long Id { get; set; }
			public string Address { get; set; }
		}
	}
}
