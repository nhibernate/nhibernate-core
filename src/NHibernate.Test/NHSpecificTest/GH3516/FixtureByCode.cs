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
	}
}
