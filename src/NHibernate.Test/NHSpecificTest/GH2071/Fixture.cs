using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2071
{
	public abstract class FixtureBase : TestCaseMappingByCode
	{
		private object domesticCatId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Cat>(rc =>
			{
				rc.EntityName("CatEntity");
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			MapSubclass(mapper);

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected abstract void MapSubclass(ModelMapper mapper);

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				domesticCatId = session.Save(new DomesticCat { Name = "Tom", OwnerName = "Jerry" });

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				transaction.Commit();
			}
		}

		[Test]
		public void CanLoadDomesticCat()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var cat = session.Load<Cat>(domesticCatId);
				Assert.That(cat, Is.Not.Null);
				Assert.That(cat.Name, Is.EqualTo("Tom"));
			}
		}
	}

	[TestFixture]
	public class SubclassFixture : FixtureBase
	{
		protected override void MapSubclass(ModelMapper mapper)
		{
			mapper.Subclass<DomesticCat>(
				rc =>
				{
					rc.Extends("CatEntity");
					rc.Property(x => x.OwnerName);
				});
		}
	}

	[TestFixture]
	public class JoinedSubclassFixture : FixtureBase
	{
		protected override void MapSubclass(ModelMapper mapper)
		{
			mapper.JoinedSubclass<DomesticCat>(
				rc =>
				{
					rc.Extends("CatEntity");
					rc.Property(x => x.OwnerName);
				});
		}
	}

	[TestFixture]
	public class UnionSubclassFixture : FixtureBase
	{
		protected override void MapSubclass(ModelMapper mapper)
		{
			mapper.UnionSubclass<DomesticCat>(
				rc =>
				{
					rc.Extends("CatEntity");
					rc.Property(x => x.OwnerName);
				});
		}
	}
}
