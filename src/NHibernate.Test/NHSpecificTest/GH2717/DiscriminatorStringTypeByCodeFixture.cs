using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2717
{
	[TestFixture]
	public class DiscriminatorStringTypeByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Discriminator(x => x.Type(NHibernateUtil.String));
				rc.Property(x => x.Name);
				rc.Abstract(true);
			});
			mapper.Subclass<Subclass>(rc => rc.DiscriminatorValue("xxx"));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Subclass { Name = "Bob" };
				session.Save(e1);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void QueryOnSubclassWithStringDiscriminator()
		{
			using (var session = OpenSession())
			{
				var result = (from e in session.Query<Subclass>()
							  where e.Name == "Bob"
							  select e)
							  .ToList();

				Assert.That(result.Count, Is.EqualTo(1));
			}
		}
	}
}
