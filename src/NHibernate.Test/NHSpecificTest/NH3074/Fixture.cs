using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3074
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		private const int Id = 123;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Animal>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Weight);
			});

			mapper.UnionSubclass<Cat>(x => x.Property(p => p.NumberOfLegs));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var cat = new Cat { Id = Id, NumberOfLegs = 2, Weight = 100 };
				s.Save(cat);
				tx.Commit();
			}
		}

		[Test]
		[Ignore("Failing")]
		public void CanSetLockMode()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.CreateQuery("select c from Animal c where c.Id=:id")
					.SetInt32("id", Id)
					.SetLockMode("c", LockMode.Upgrade)
					.List<Cat>()
					.Should().Not.Be.Empty();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete(s.Get<Cat>(Id));
				tx.Commit();
			}
		}
	}
}