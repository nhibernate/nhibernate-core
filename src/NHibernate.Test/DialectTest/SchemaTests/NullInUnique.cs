using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.DialectTest.SchemaTests
{
	[TestFixture]
	public class NullInUniqueFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name, m => m.Unique(true));
					rc.Property(
						x => x.Name1,
						m =>
						{
							m.NotNullable(true);
							m.UniqueKey("Test");
						});
					rc.Property(x => x.Name2, m => m.UniqueKey("Test"));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Entity").ExecuteUpdate();
				transaction.Commit();
			}
		}

		[Test]
		public void InsertNullInUnique()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Entity { Name1 = "1" });
				session.Save(new Entity { Name = "N", Name1 = "1", Name2 = "2" });
				session.Save(new Entity { Name = "Na", Name1 = "2", Name2 = "1" });
				session.Save(new Entity { Name = "Nam", Name1 = "2" });
				transaction.Commit();
			}
		}
	}
}
