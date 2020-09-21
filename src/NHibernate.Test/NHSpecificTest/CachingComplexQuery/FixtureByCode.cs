using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CachingComplexQuery
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	/// <remarks>
	/// This fixture is identical to <see cref="Fixture" /> except the <see cref="Person" /> mapping is performed 
	/// by code in the GetMappings method, and does not require the <c>Mappings.hbm.xml</c> file. Use this approach
	/// if you prefer.
	/// </remarks>
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Guid));
				rc.Property(x => x.Name);
				rc.Property(x => x.Age);
				rc.Set(
					x => x.Children,
					colMap =>
					{
						colMap.Inverse(true);
						colMap.Cascade(Mapping.ByCode.Cascade.DeleteOrphans);
						colMap.Cache(c => c.Usage(CacheUsage.ReadWrite));
					},
					rel => rel.OneToMany());
				rc.Set(
					x => x.Cars,
					colMap =>
					{
						colMap.Inverse(true);
						colMap.Cascade(Mapping.ByCode.Cascade.DeleteOrphans);
						colMap.Cache(c => c.Usage(CacheUsage.ReadWrite));
					},
					rel => rel.OneToMany());
				rc.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});
			mapper.Class<Child>(ch =>
			{
				ch.Id(x => x.Id, m => m.Generator(Generators.Guid));
				ch.Property(x => x.Name);
				ch.ManyToOne(c => c.Parent);

				ch.Set(
					x => x.Pets,
					colMap =>
					{
						colMap.Inverse(true);
						colMap.Cascade(Mapping.ByCode.Cascade.DeleteOrphans);
						colMap.Cache(c => c.Usage(CacheUsage.ReadWrite));
					},
					rel => rel.OneToMany());

				ch.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});
			mapper.Class<Pet>(ch =>
			{
				ch.Id(x => x.Id, m => m.Generator(Generators.Guid));
				ch.Property(x => x.Name);
				ch.ManyToOne(c => c.Owner);

				ch.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});
			mapper.Class<Car>(ch =>
			{
				ch.Id(x => x.Id, m => m.Generator(Generators.Guid));
				ch.Property(x => x.Name);
				ch.ManyToOne(c => c.Owner);

				ch.Cache(c => c.Usage(CacheUsage.ReadWrite));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = new Person { Name = "Person 1", Age = 18 };

				var car1 = new Car { Name = "Car1", Owner = person };
				var car2 = new Car { Name = "Car2", Owner = person };
				session.Save(car1);
				session.Save(car2);
				
				session.Save(person);
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Pet");
				session.Delete("from Child");
				session.Delete("from Car");
				session.Delete("from Person");

				transaction.Commit();
			}
		}

		[Test]
		public void TestQueryCachingWithThenFetchMany()
		{
			using (var session = OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					var query =
						session
							.Query<Person>()
							.FetchMany(p => p.Children)
								.ThenFetchMany(ch => ch.Pets)
							.FetchMany(p => p.Cars) as IQueryable<Person>;

					query = query.WithOptions(opt =>
						opt.SetCacheable(true)
							.SetCacheMode(CacheMode.Normal)
							.SetCacheRegion("Long_Cache"));

					query.ToList(); // First time the result will be cached
					Assert.That(() => query.ToList(), Throws.Nothing); // 
					transaction.Commit();
				}
			}
		}
	}
}
