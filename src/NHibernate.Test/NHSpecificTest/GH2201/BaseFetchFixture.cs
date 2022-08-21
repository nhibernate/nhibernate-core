using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2201
{
	public class BaseFetchFixture : TestCaseMappingByCode
	{
		protected int _id;
		protected int _depth;

		protected BaseFetchFixture(int depth)
		{
			_depth = depth < 0 ? int.MaxValue : depth;
		}

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
				m.ManyToOne(c => c.Level1, p =>
				{
					p.Column("Level1Id");
					p.Fetch(FetchKind.Join);
					p.Cascade(Mapping.ByCode.Cascade.Persist);
					p.ForeignKey("none");
				});
			});

			mapper.Class<Level1Entity>(m =>
			{
				m.Id(c => c.Id, id =>
				{
					id.Generator(Generators.Native);
				});
				m.Property(c => c.Name);
				m.ManyToOne(c => c.Level2, p =>
				{
					p.Column("Level2Id");
					p.Fetch(FetchKind.Join);
					p.Cascade(Mapping.ByCode.Cascade.Persist);
					p.ForeignKey("none");
				});
			});

			mapper.Class<Level2Entity>(m =>
			{
				m.Id(c => c.Id, id =>
				{
					id.Generator(Generators.Native);
				});
				m.Property(c => c.Name);
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
					Level1 = new Level1Entity() { Level2 = new Level2Entity() }
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
				Level1 = new Level1Entity() { Level2 = new Level2Entity() }
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

		protected void Verify(Entity result)
		{
			VerifyChildrenInitialized(result);

			VerifyChildrenNotInitialized(result.AdditionalEntity);
			VerifyChildrenNotInitialized(result.SourceEntity);
			VerifyChildrenNotInitialized(result.ReferencedEntity);
		}

		protected void VerifyChildrenInitialized(Entity result)
		{
			var isInited = _depth > 0 ? (NUnit.Framework.Constraints.IResolveConstraint) Is.True : Is.False;
			Assert.That(result, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result), isInited);
			Assert.That(result.AdditionalEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.AdditionalEntity), isInited);
			Assert.That(result.SourceEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.SourceEntity), isInited);
			Assert.That(result.ReferencedEntity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.ReferencedEntity), isInited);
			Assert.That(result.Level1, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.Level1), isInited);
			if (_depth >= 1)
			{
				Assert.That(result.Level1.Level2, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(result.Level1.Level2), _depth > 1 ? Is.True : Is.False);
			}
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
			Assert.That(result.Level1, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.Level1), Is.False);
		}
	}
}
