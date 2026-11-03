using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3563
{
	[TestFixture]
	public class FixtureByCode : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Property(x => x.Status, m => m.Type<EnumStringType<EntityStatus>>());
			});

			mapper.Class<BaseClass>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Discriminator(x =>
				{
					x.Column("EnumDiscriminator");
					x.Type<EnumStringType<Kind>>();
				});
				rc.DiscriminatorValue(nameof(Kind.Base));
				rc.Property(x => x.Name);
			});
			mapper.Subclass<Subclass>(rc => rc.DiscriminatorValue(nameof(Kind.Derived)));

			mapper.Import<EntityStatus>();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.Save(new Entity { Name = "Bob", Status = EntityStatus.Active });
			session.Save(new Entity { Name = "Sally", Status = EntityStatus.Inactive });

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void CanQueryWithEnumStringConstant()
		{
			using var session = OpenSession();

			// ObjectToSQLString is used for rendering the constant.
			var list = session
				.CreateQuery("from Entity e where e.Status = EntityStatus.Inactive")
				.List<Entity>();
			Assert.That(list, Has.Count.EqualTo(1));
			Assert.That(list[0].Name, Is.EqualTo("Sally"));
		}

		[Test]
		public void CanUseEnumStringDiscriminator()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new BaseClass { Name = "Base" });
				session.Save(new Subclass { Name = "Derived" });

				// ObjectToSQLString is used for generating the inserts.
				transaction.Commit();
			}

			using (var session = OpenSession())
			{
				var all = session.CreateQuery("from BaseClass").List<BaseClass>();
				Assert.That(all, Has.Count.EqualTo(2));

				// ObjectToSQLString is used for generating the discriminator restriction.
				var subclasses = session.CreateQuery("from Subclass").List<Subclass>();
				Assert.That(subclasses, Has.Count.EqualTo(1));
				Assert.That(subclasses[0].Name, Is.EqualTo("Derived"));
			}
		}
	}
}
