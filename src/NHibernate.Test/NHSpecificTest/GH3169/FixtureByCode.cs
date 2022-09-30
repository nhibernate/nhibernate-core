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
		class ResultDto
		{
			public string regionCode { get; set; }
		}

		class Entity
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Table("Entity");
				rc.Id(x => x.Id, m => m.Generator(Generators.Native));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void CachedQueryWithTransformer()
		{
			IList<ResultDto> GetCacheableSqlQueryResults(ISession s)
			{
				return s.CreateSQLQuery(
							"select Name as regionCode from Entity")
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
					//TODO: Uncomment if we properly fix caching auto discovery type queries with transformers
					//Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(1), "results are expected from DB");
					//Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0), "results are expected from DB");
				}

				using (EnableStatisticsScope())
				{
					var l2 = GetCacheableSqlQueryResults(session);
					Assert.AreEqual(1, l2.Count);
					//TODO: Uncomment if we properly fix caching auto discovery type queries with transformers
					//Assert.That(Sfi.Statistics.QueryCacheMissCount, Is.EqualTo(0), "results are expected from cache");
					//Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "results are expected from cache");
				}
			}
		}
	}
}
