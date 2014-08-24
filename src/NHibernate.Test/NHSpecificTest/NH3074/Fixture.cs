using NHibernate.Cfg.MappingSchema;
using NHibernate.Criterion;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

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
		[Ignore("Fails on at least Oracle and PostgreSQL. See NH-3074 and NH-2408.")]
		public void HqlCanSetLockMode()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var cats = s.CreateQuery("select c from Animal c where c.Id=:id")
							.SetInt32("id", Id)
							.SetLockMode("c", LockMode.Upgrade)
							.List<Cat>();

				Assert.That(cats, Is.Not.Empty);
			}
		}

		[Test, Ignore("Not fixed yet")]
		public void CritriaCanSetLockMode()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var cats = s.CreateCriteria<Animal>("c")
							.Add(Restrictions.IdEq(Id))
							.SetLockMode("c", LockMode.Upgrade)
							.List<Cat>();

				Assert.That(cats, Is.Not.Empty);
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