using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3289
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
				rc.Property(x => x.Name);
				rc.Component(x => x.Component);
			});
			mapper.JoinedSubclass<SubEntity>(rc =>
			{
				rc.EntityName(typeof(ISubEntity).FullName);
				rc.Key(k => k.Column("Id"));
				rc.Property(x => x.SomeProperty);
			});
			mapper.Component<Component>(rc =>
			{
				rc.Property(x => x.Field);
				rc.Lazy(true);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var e1 = new SubEntity { Name = "Jim" };
			session.Save(e1);

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();

			if (Dialect.SupportsTemporaryTables)
				session.CreateQuery("delete from System.Object").ExecuteUpdate();
			else
				session.Delete("from System.Object");

			transaction.Commit();
		}

		[Test]
		public void TestSubEntityInterfaceWithFetchIsPropertyInitialized()
		{
			using var session = OpenSession();
			var data = session.Query<ISubEntity>()
				.Fetch(e => e.Component)
				.ToList();
			var result = NHibernateUtil.IsPropertyInitialized(data[0], "Component");

			Assert.That(result, Is.True);
		}

		[Test]
		public void TestSubEntityWithFetchIsPropertyInitialized()
		{
			using var session = OpenSession();
			var data = session.Query<SubEntity>()
				.Fetch(e => e.Component)
				.ToList();
			var result = NHibernateUtil.IsPropertyInitialized(data[0], "Component");

			Assert.That(result, Is.True);
		}
	}
}
