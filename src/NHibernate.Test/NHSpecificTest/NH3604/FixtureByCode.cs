using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3604
{
	/// <summary>
	/// Tests ability to map a non-public property by code via expressions to access the hidden properties
	/// </summary>
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(Entity.PropertyAccessExpressions.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.OneToOne(x => x.Detail, m => m.Cascade(Mapping.ByCode.Cascade.All));
			});

			mapper.Class<EntityDetail>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(new ForeignGeneratorDef(ReflectionHelper.GetProperty(EntityDetail.PropertyAccessExpressions.Entity))));
				rc.OneToOne(EntityDetail.PropertyAccessExpressions.Entity, m => m.Constrained(true));
				rc.Property(x => x.ExtraInfo);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);


				var e2 = new Entity { Name = "Sally" };
				var ed2 = new EntityDetail(e2) { ExtraInfo = "Jo" };
				e2.Detail = ed2;

				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanPerformQueryOnMappedClassWithProtectedProperty()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = from e in session.Query<Entity>()
							 where e.Name == "Sally"
							 select e;

				var entities = result.ToList();
				Assert.AreEqual(1, entities.Count);
				Assert.AreEqual("Jo", entities[0].Detail.ExtraInfo);
			}
		}

		[Test]
		public void CanWriteMappingsReferencingProtectedProperty()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(Entity.PropertyAccessExpressions.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});
			mapper.CompileMappingForEachExplicitlyAddedEntity().WriteAllXmlMapping();
		}
	}
}