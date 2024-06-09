using System.Collections;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3516
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
			});

			mapper.Class<BaseClass>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Discriminator(x => x.Column("StringDiscriminator"));
				rc.Property(x => x.Name);
				rc.Abstract(true);
			});
			mapper.Subclass<Subclass1>(rc => rc.DiscriminatorValue(Entity.NameWithSingleQuote));
			mapper.Subclass<Subclass2>(rc => rc.DiscriminatorValue(Entity.NameWithEscapedSingleQuote));

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var e = new Entity { Name = Entity.NameWithSingleQuote };
			session.Save(e);
			e = new Entity { Name = Entity.NameWithEscapedSingleQuote };
			session.Save(e);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		private static readonly string[] _stringInjectionsProperties =
			new[]
			{
				nameof(Entity.NameWithSingleQuote),
				nameof(Entity.NameWithEscapedSingleQuote)
			};

		[TestCaseSource(nameof(_stringInjectionsProperties))]
		public void SqlInjectionInStrings(string propertyName)
		{
			using var session = OpenSession();

			var query = session.CreateQuery($"from Entity e where e.Name = Entity.{propertyName}");
			IList<Entity> list = null;
			Assert.That(() => list = query.List<Entity>(), Throws.Nothing);
			Assert.That(list, Has.Count.EqualTo(1), $"Unable to find entity with name {propertyName}");
		}

		[Test]
		public void SqlInjectionInStringDiscriminator()
		{
			using var session = OpenSession();

			session.Save(new Subclass1 { Name = "Subclass1" });
			session.Save(new Subclass2 { Name = "Subclass2" });

			// ObjectToSQLString is used for generating the inserts.
			Assert.That(session.Flush, Throws.Nothing, "Unable to flush the subclasses");

			foreach (var entityName in new[] { nameof(Subclass1), nameof(Subclass2) })
			{
				var query = session.CreateQuery($"from {entityName}");
				IList list = null;
				Assert.That(() => list = query.List(), Throws.Nothing, $"Unable to list entities of {entityName}");
				Assert.That(list, Has.Count.EqualTo(1), $"Unable to find the {entityName} entity");
			}
		}
	}
}
