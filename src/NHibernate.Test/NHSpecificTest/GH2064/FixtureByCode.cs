using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2064
{

	[TestFixture]
	public class OneToOneSelectProjectionFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<OneToOneEntity>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.Assigned));
					rc.Property(e => e.Name);
				});

			mapper.Class<ParentEntity>(
				rc =>
				{
					rc.Id(e => e.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(e => e.Name);
					rc.OneToOne(e => e.OneToOne, m => { });
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var nullableOwner = new ParentEntity() {Name = "Owner",};
				var oneToOne = new OneToOneEntity() {Name = "OneToOne"};
				nullableOwner.OneToOne = oneToOne;
				session.Save(nullableOwner);
				oneToOne.Id = nullableOwner.Id;
				session.Save(oneToOne);
				session.Flush();

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void QueryOneToOne()
		{
			using (var session = OpenSession())
			{
				var entity =
					session
						.Query<ParentEntity>()
						.FirstOrDefault();
				Assert.That(entity.OneToOne, Is.Not.Null);
			}
		}

		[Test]
		public void QueryOneToOneProjection()
		{
			using (var session = OpenSession())
			{
				var entity =
					session
						.Query<ParentEntity>()
						.Select(
							x => new
							{
								x.Id,
								SubType = new {x.OneToOne, x.Name},
								SubType2 = new
								{
									x.Id,
									x.OneToOne,
									SubType3 = new {x.Id, x.OneToOne}
								},
								x.OneToOne
							}).FirstOrDefault();
				Assert.That(entity.OneToOne, Is.Not.Null);
				Assert.That(entity.SubType.OneToOne, Is.Not.Null);
				Assert.That(entity.SubType2.OneToOne, Is.Not.Null);
				Assert.That(entity.SubType2.SubType3.OneToOne, Is.Not.Null);
			}
		}
	}
}
