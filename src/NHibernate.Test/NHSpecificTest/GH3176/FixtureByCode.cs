using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3176
{
	[TestFixture(CacheFactory.ReadOnly)]
	[TestFixture(CacheFactory.NonstrictReadWrite)]
	[TestFixture(CacheFactory.ReadWrite)]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		public ByCodeFixture(string cacheStrategy)
		{
			CacheConcurrencyStrategy = cacheStrategy;
		}

		protected override string CacheConcurrencyStrategy { get; }

		private int _id;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
					rc.Property(x => x.Name);
					rc.Component(
						x => x.Component,
						m =>
						{
							m.Property(x => x.Field);
							m.Lazy(true);
						});
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Environment.UseSecondLevelCache] = "true";
			configuration.Properties[Environment.GenerateStatistics] = "true";
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob", Component = new Component() { Field = "Jim" } };
				session.Save(e1);
				_id = e1.Id;

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void TestPreLoadedData()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Load the entities into the second-level cache.
				session.Query<Entity>().WithOptions(o => o.SetCacheable(true)).ToList();

				var result = session.Query<Entity>().WithOptions(o => o.SetCacheable(true)).Fetch(e => e.Component).First();

				Assert.That(NHibernateUtil.IsPropertyInitialized(result, "Component"), Is.True);

				var field = result.Component?.Field;

				Assert.That(field, Is.EqualTo("Jim"));
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.Query<Entity>().WithOptions(o => o.SetCacheable(true)).Fetch(e => e.Component).First();

				Assert.That(NHibernateUtil.IsPropertyInitialized(result, "Component"), Is.True);

				var field = result.Component?.Field;

				Assert.That(field, Is.EqualTo("Jim"));
			}
		}

		[Test]
		public void InitializedLazyPropertyShouldBeCached()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e = session.Get<Entity>(_id);
				Assert.That(e.Component?.Field, Is.EqualTo("Jim"));

				Assert.That(NHibernateUtil.IsPropertyInitialized(e, "Component"), Is.True);
				transaction.Commit();
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e = session.Get<Entity>(_id);

				Assert.That(NHibernateUtil.IsPropertyInitialized(e, "Component"), Is.True, "Lazy property is not cached");
				var field = e.Component?.Field;

				Assert.That(field, Is.EqualTo("Jim"));
				transaction.Commit();
			}
		}

		[Test]
		public void TestNonPreLoadedData()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.Query<Entity>().WithOptions(o => o.SetCacheable(true)).Fetch(e => e.Component).First();

				Assert.That(NHibernateUtil.IsPropertyInitialized(result, "Component"), Is.True);

				var field = result.Component?.Field;

				Assert.That(field, Is.EqualTo("Jim"));
			}

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = session.Query<Entity>().WithOptions(o => o.SetCacheable(true)).Fetch(e => e.Component).First();

				Assert.That(NHibernateUtil.IsPropertyInitialized(result, "Component"), Is.True);

				var field = result.Component?.Field;

				Assert.That(field, Is.EqualTo("Jim"));
			}
		}
	}
}
