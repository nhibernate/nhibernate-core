using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2856
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Table("Person");
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Address, m => m.Column("AddressId"));
			});

			mapper.Class<Address>(rc =>
			{
				rc.Table("Addresses");
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void EntityIsReturnedFromCacheOnSubsequentQueriesWhenUsingCacheableFetchQuery()
		{
			using (var session = OpenSession())
			{
				var query = session.Query<Person>()
					.Fetch(p => p.Address)
					.Cacheable();

				Sfi.Statistics.Clear();

				var result = query.ToList(); // Execute the query

				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(0));

				Sfi.Statistics.Clear();

				var cachedResult = query.ToList(); // Re-execute the query

				Assert.That(cachedResult.Count, Is.EqualTo(1));
				Assert.That(Sfi.Statistics.QueryExecutionCount, Is.EqualTo(0));
				Assert.That(Sfi.Statistics.QueryCacheHitCount, Is.EqualTo(1));
			}
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var person = new Person { Id = 5, Name = "Joe Bloggs" };
				session.Save(person);

				var address = new Address { Id = 15, Name = "Home" };
				session.Save(address);

				person.Address = address;
				session.Flush();

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from Address");
				transaction.Commit();
			}
		}
	}
}