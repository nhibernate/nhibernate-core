using System;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3138
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override Cfg.MappingSchema.HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(ca =>
			{
				ca.Lazy(false);
				ca.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				ca.Property(x => x.EnglishName);
				ca.Property(x => x.GermanName);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		[Test]
		public void PageQueryWithDistinctAndOrderByContainingFunctionWithCommaSeparatedParameters()
		{
			using (var session = OpenSession())
			{
				Assert.DoesNotThrow(() =>
					session
						.CreateQuery("select distinct e.Id, coalesce(e.EnglishName, e.GermanName) from Entity e order by coalesce(e.EnglishName, e.GermanName) asc")
						.SetFirstResult(10)
						.SetMaxResults(20)
						.List<Entity>());
			}
		}

		[Test]
		[Ignore("Failing")]
		public void PageQueryWithDistinctAndOrderByContainingAliasedFunction()
		{
			using (var session = OpenSession())
			{
				Assert.DoesNotThrow(() =>
					session
						.CreateQuery("select distinct e.Id, coalesce(e.EnglishName, e.GermanName) as LocalizedName from Entity e order by LocalizedName asc")
						.SetFirstResult(10)
						.SetMaxResults(20)
						.List<Entity>());
			}
		}
	}

	class Entity
	{
		public Guid Id { get; set; }
		public string EnglishName { get; set; }
		public string GermanName { get; set; }
	}
}