using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3772
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
					rc.Property(x => x.Name);

					rc.Set(
						x => x.SubEntities,
						x => x.Type<CustomGenericCollection<SubEntity>>(),
						x => x.ManyToMany());
				});

			mapper.Class<SubEntity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity {Name = "Bob"});

				session.Save(new Entity {Name = "Sally"});

				session.Save(new SubEntity {Name = "Bob"});

				session.Save(new SubEntity {Name = "Sally"});

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				session.Flush();

				transaction.Commit();
			}
		}

		[Test]
		public void CustomCollectionType_ThrowsHibernateException_WhenUserCollectionTypeReturnsInitializedCollection()
		{
			CustomGenericCollection<SubEntity>.InstantiateInitializedCollection = true;

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				Entity Action() => (from e in session.Query<Entity>() where e.Name == "Bob" select e).First();

				Assert.That(
					Action,
					Throws.InstanceOf<HibernateException>().And.Message.EqualTo(
						"UserCollectionType.Instantiate should return a non-initialized persistent " +
						"collection. Implement UserCollectionType.Instantiate(int anticipatedSize) to " +
						"actually create the collection that needs to be wrapped by the persistent collection."));
			}
		}

		[Test]
		public void CustomCollectionType_DoesNotThrowHibernateException_WhenUserCollectionTypeReturnsUninitializedCollection()
		{
			CustomGenericCollection<SubEntity>.InstantiateInitializedCollection = false;

			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				Entity Action() => (from e in session.Query<Entity>() where e.Name == "Bob" select e).First();

				Assert.That(Action, Throws.Nothing);
			}
		}
	}
}
