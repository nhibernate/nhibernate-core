using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3643
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id);
					rc.Bag(
						x => x.Children,
						m =>
						{
							m.Access(Accessor.Field);
							m.Key(k => k.Column("EntityId"));
							m.Cascade(Mapping.ByCode.Cascade.All);
						},
						r => r.OneToMany());
			
					rc.Cache(
						cm =>
						{
							cm.Include(CacheInclude.All);
							cm.Usage(CacheUsage.ReadWrite);
						});
				});

			mapper.Class<ChildEntity>(
				rc =>
				{
					rc.Id(x => x.Id);
					rc.Cache(
						cm =>
						{
							cm.Include(CacheInclude.All);
							cm.Usage(CacheUsage.ReadWrite);
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			var entity = new Entity { Id = EntityId.Id1 };
			entity.Children.Add(new ChildEntity { Id = 0 });
			entity.Children.Add(new ChildEntity { Id = 1 });
			session.Save(entity);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from ChildEntity").ExecuteUpdate();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}
	
		[Test]
		public void LoadsEntityWithEnumIdAndChildrenUsingQueryCache()
		{
			LoadEntityWithQueryCache(); // warm up cache

			var entity = LoadEntityWithQueryCache();

			Assert.That(entity.Children.Count(), Is.EqualTo(2));
			
			Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1), "Unexpected execution count");
			Assert.That(Sfi.Statistics.QueryCachePutCount, Is.EqualTo(1), "Unexpected cache put count");
			Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1), "Unexpected cache hit count");
		}
		
		private Entity LoadEntityWithQueryCache()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var entity = session
				.Query<Entity>()
				.FetchMany(x => x.Children)
				.WithOptions(opt => opt.SetCacheable(true))
				.ToList()[0];
			
			transaction.Commit();
			return entity;
		}
	}
}
