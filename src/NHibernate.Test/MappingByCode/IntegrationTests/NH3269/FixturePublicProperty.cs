using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3269
{
	public class FixturePublicProperty : TestCaseMappingByCode
	{
		[Test]
		public void ShouldThrowExceptionWhenTryingToSaveInherited1WithDuplicateName()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Inherited1 { Name = "Bob" };
				session.Save(e1);

				Assert.That(() => { transaction.Commit(); }, Throws.Exception);
			}
		}

		[Test]
		public void ShouldNotThrowExceptionWhenTryingToSaveInherited2WithDuplicateName()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e2 = new Inherited2 { Name = "Sally" };
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Inherited1>(rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Guid));
					rc.Property(x => x.Name, m => m.UniqueKey("Inherited1_UX_Name"));
				});
	
			mapper.Class<Inherited2>(rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.Guid));
					rc.Property(x => x.Name, m => m.Length(200));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Inherited1 { Name = "Bob" };
				session.Save(e1);

				var e2 = new Inherited2 { Name = "Sally" };
				session.Save(e2);

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
