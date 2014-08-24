using System;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2510
{
	public class Image
	{
		public virtual int Id { get; set; }
		public virtual byte[] Data { get; set; }
	}
	public class Fixture: TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Image>(rc =>
			                    {
														rc.Cache(map => map.Usage(CacheUsage.NonstrictReadWrite));
														rc.Id(x=> x.Id);
														rc.Property(x => x.Data, map=> map.Lazy(true));
			                    });
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			return mappings;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.Cache(x=> x.Provider<HashtableCacheProvider>());
		}

		private class Scenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public Scenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (var session = factory.OpenSession())
				using (session.BeginTransaction())
				{
					session.Persist(new Image { Id = 1 });
					session.Transaction.Commit();
				}
			}

			public void Dispose()
			{
				using (var session = factory.OpenSession())
				using (session.BeginTransaction())
				{
					session.CreateQuery("delete from Image").ExecuteUpdate();
					session.Transaction.Commit();
				}				
			}
		}

		[Test]
		public void WhenReadFromCacheThenDoesNotThrow()
		{
			using (new Scenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					var book = s.Get<Image>(1);
				}
				using (ISession s = OpenSession())
				{
					var book = s.Get<Image>(1);
				}
			}
		}
	}
}