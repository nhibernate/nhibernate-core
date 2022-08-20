using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class BaseFetchFixture : TestCaseMappingByCode
	{
		protected int _id;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(m =>
			{
				m.Id(c => c.EntityId, id =>
				{
					id.Generator(Generators.Native);
				});
				m.Property(c => c.EntityNumber);
				m.ManyToOne(c => c.ReferencedEntity, p =>
				{
					p.Column("ReferencedEntityId");
					p.Fetch(FetchKind.Join);
					p.Cascade(Mapping.ByCode.Cascade.Persist);
					p.ForeignKey("none");
				});
				m.ManyToOne(c => c.AdditionalEntity, p =>
				{
					p.Column("AdditionalEntityId");
					p.Fetch(FetchKind.Join);
					p.Cascade(Mapping.ByCode.Cascade.Persist);
					p.ForeignKey("none");
				});
				m.ManyToOne(c => c.SourceEntity, p =>
				{
					p.Column("SourceEntityId");
					p.Fetch(FetchKind.Join);
					p.Cascade(Mapping.ByCode.Cascade.Persist);
					p.ForeignKey("none");
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity
				{
					EntityNumber = "Bob",
					AdditionalEntity = CreateEntity(),
					ReferencedEntity = CreateEntity(),
					SourceEntity = CreateEntity(),
				};
				session.Save(e1);
				transaction.Commit();
				_id = e1.EntityId;
			}
		}
		private static Entity CreateEntity()
		{
			return new Entity
			{
				SourceEntity = new Entity(),
				AdditionalEntity = new Entity(),
				ReferencedEntity = new Entity(),
			};
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

		protected static void Verify(Entity result)
		{

			VerifyChildrenInitialized(result);

			VerifyChildrenNotInitialized(result.AdditionalEntity);
			VerifyChildrenNotInitialized(result.SourceEntity);
			VerifyChildrenNotInitialized(result.ReferencedEntity);
		}

		protected static void VerifyChildrenInitialized(Entity result)
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result), Is.True);
			Assert.That(result.AdditionalEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.AdditionalEntity), Is.True);
			Assert.That(result.SourceEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.SourceEntity), Is.True);
			Assert.That(result.ReferencedEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.ReferencedEntity), Is.True);
		}

		protected static void VerifyChildrenNotInitialized(Entity result)
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result), Is.True);
			Assert.That(result.AdditionalEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.AdditionalEntity), Is.False);
			Assert.That(result.SourceEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.SourceEntity), Is.False);
			Assert.That(result.ReferencedEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.ReferencedEntity), Is.False);
		}
	}
}
