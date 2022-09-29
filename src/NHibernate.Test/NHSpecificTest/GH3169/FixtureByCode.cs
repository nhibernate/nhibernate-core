using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Transform;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3169
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		public class ResultDto
		{
			public string regionCode { get; set; }
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}


		[Test]
		public void CachedQueryWithTransformer()
		{
			IList<ResultDto> GetCacheableSqlQueryResults(ISession s)
			{
				return s.CreateSQLQuery(
							"select 'REGIONCODE' as regionCode  ")
						.AddScalar("regionCode", NHibernateUtil.String)
						.SetResultTransformer(Transformers.AliasToBean<ResultDto>())
						.SetCacheable(true)
						.List<ResultDto>();
			}

			using (var session = OpenSession())
			{
				using (EnableStatisticsScope())
				{
					var l = GetCacheableSqlQueryResults(session);
					Assert.AreEqual(1, l.Count);
					//Uncomment if we properly fix caching auto discovery type queries with transformers
					//Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(1), "results are expected from DB");
					//Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "results are expected from DB");
				}

				using (EnableStatisticsScope())
				{
					var l2 = GetCacheableSqlQueryResults(session);
					Assert.AreEqual(1, l2.Count);
					//Uncomment if we properly fix caching auto discovery type queries with transformers
					//Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "results are expected from cache");
					//Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "results are expected from cache");
				}
			}
		}
	}
}
