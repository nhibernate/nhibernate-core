using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2347
{
	public class Fixture : TestCaseMappingByCode
	{
		[Test]
		public void CanSumIntegersAsLongIntegers()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var totalQuantity = (from e in session.Query<Entity>()
									 select e).Sum(x => (long) x.Quantity);

				const long expected = (long)int.MaxValue + int.MaxValue;
				Assert.That(totalQuantity, Is.EqualTo(expected));
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
									 {
										 rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
										 rc.Property(x => x.Quantity);
									 });

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity {Quantity = int.MaxValue});
				session.Save(new Entity {Quantity = int.MaxValue});

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}
	}
}