using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3352
{
	[TestFixture]
	public class FetchFromNotMappedBaseClassFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityNameMapped>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m => m.Lazy(true));
			});
			mapper.Class<EntityParentMapped>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.ManyToOne(x => x.Parent, m => m.ForeignKey("none"));
			});
			mapper.Class<EntityComponentMapped>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Component(x => x.Component);
			});
			mapper.Component<Component>(rc =>
			{
				rc.Property(x => x.Field);
				rc.ManyToOne(x => x.Entity, m => m.ForeignKey("none"));
				rc.Lazy(true);
			});
			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			var np = new EntityComponentMapped { Component = new Component { Field = "x" } };
			session.Save(np);
			var e = new EntityParentMapped { Parent = np };
			session.Save(e);
			var nameMapped = new EntityNameMapped { Name = "lazy" };
			session.Save(nameMapped);
			np.Component.Entity = nameMapped;

			transaction.Commit();
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateQuery("delete from System.Object").ExecuteUpdate();

			transaction.Commit();
		}

		[Test]
		public void CanFetchLazyComponentFromNotMappedBaseClass()
		{
			using var session = OpenSession();
			var list = session.Query<EntityComponentMapped>().Fetch(x => x.Component).ToList();

			Assert.That(list, Has.Count.EqualTo(1));
			var result = list[0];
			Assert.That(NHibernateUtil.IsPropertyInitialized(result, nameof(result.Component)));
			Assert.That(result.Component.Field, Is.EqualTo("x"));
		}

		[Test]
		public void CanFetchLazyComponentThenEntityFromNotMappedBaseClass()
		{
			using var session = OpenSession();
			var list = session.Query<EntityComponentMapped>()
				.Fetch(x => x.Component)
				.ThenFetch(x => x.Entity)
				.ThenFetch(x => x.Name)
				.ToList();

			Assert.That(list, Has.Count.EqualTo(1));
			var result = list[0];
			Assert.That(NHibernateUtil.IsPropertyInitialized(result, nameof(result.Component)));
			Assert.That(result.Component.Field, Is.EqualTo("x"));
			Assert.That(result.Component.Entity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.Component.Entity), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(result.Component.Entity, nameof(result.Name)), Is.True);
			Assert.That(result.Component.Entity.Name, Is.EqualTo("lazy"));
		}

		[Test]
		public void CanFetchLazyPropertyFromNotMappedBaseClass()
		{
			using var session = OpenSession();
			var list = session.Query<EntityNameMapped>().Fetch(x => x.Name).ToList();

			Assert.That(list, Has.Count.EqualTo(1));
			var result = list[0];
			Assert.That(NHibernateUtil.IsPropertyInitialized(result, nameof(result.Name)));
			Assert.That(result.Name, Is.EqualTo("lazy"));
		}

		[Test]
		public void CanThenFetchLazyComponentFromNotMappedBaseClass()
		{
			using var session = OpenSession();
			var list = session.Query<EntityParentMapped>().Fetch(x => x.Parent).ThenFetch(x => x.Component).ToList();

			Assert.That(list, Has.Count.EqualTo(1));
			var result = list[0].Parent;
			Assert.That(NHibernateUtil.IsInitialized(result), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(result, nameof(result.Component)));
			Assert.That(result.Component.Field, Is.EqualTo("x"));
		}

		[KnownBug("GH-3356")]
		[Test(Description = "GH-3356" )]
		public void FetchAfterSelect()
		{
			using var log = new SqlLogSpy();

			using var s = OpenSession();
			var list = s.Query<EntityParentMapped>()
				.Select(x => x.Parent)
				.Fetch(x => x.Component)
				.ThenFetch(x => x.Entity)
				.ThenFetch(x => x.Name)
				.ToList();
			Assert.That(list, Has.Count.EqualTo(1));
			var result = list[0];
			Assert.That(NHibernateUtil.IsPropertyInitialized(result, nameof(result.Component)));
			Assert.That(result.Component.Field, Is.EqualTo("x"));
			Assert.That(result.Component.Entity, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(result.Component.Entity), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(result.Component.Entity, nameof(result.Name)), Is.True);
			Assert.That(result.Component.Entity.Name, Is.EqualTo("lazy"));
		}

		[Test]
		public void CanFetchEntityFromNotMappedBaseClass()
		{
			using var session = OpenSession();
			var list = session.Query<EntityParentMapped>().Fetch(x => x.Parent).ToList();

			Assert.That(list, Has.Count.EqualTo(1));
			Assert.That(list[0].Parent, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(list[0].Parent));
		}

		[Test]
		public void FetchNotMappedAssociationThrows()
		{
			using var session = OpenSession();
			var query = session.Query<EntityNameMapped>().Fetch(x => x.Parent);

			Assert.Throws<QueryException>(() => query.ToList());
		}
	}
}
