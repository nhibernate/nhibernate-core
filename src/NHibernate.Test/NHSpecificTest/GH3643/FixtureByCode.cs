﻿using System.Linq;
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
			
			session.CreateSQLQuery(
				"INSERT INTO Entity (Id) VALUES (0)"
			).ExecuteUpdate();

			session.CreateSQLQuery(
				"INSERT INTO ChildEntity (Id, EntityId) VALUES (0, 0)"
			).ExecuteUpdate();

			session.CreateSQLQuery(
				"INSERT INTO ChildEntity (Id, EntityId) VALUES (1, 0)"
			).ExecuteUpdate();
			
			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateSQLQuery("DELETE FROM ChildEntity").ExecuteUpdate();
			session.CreateSQLQuery("DELETE FROM Entity").ExecuteUpdate();

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
