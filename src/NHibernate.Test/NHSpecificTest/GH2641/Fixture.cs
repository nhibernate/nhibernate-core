using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2641
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var entity = new Entity { Id = 1, Value = 0.00000000000000422030887989616 };
				session.Save(entity);

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

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.OracleSuppressDecimalInvalidCastException, "true");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return Dialect is Oracle8iDialect;
		}

		[Test]
		public void ShouldNotThrow()
		{
			using (var session = OpenSession())
			{
				session.Query<Entity>().ToList();
			}
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				m =>
				{
					m.Table("Entity");
					m.Id(x => x.Id, (i) => i.Generator(Generators.Assigned));
					m.Property(x => x.Value);
				});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
