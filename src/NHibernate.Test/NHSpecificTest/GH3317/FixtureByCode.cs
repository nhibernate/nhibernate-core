using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3317
{
	[TestFixture(true)]
	[TestFixture(false)]
	public class ComponentsListFixture : TestCaseMappingByCode
	{
		private readonly bool _fetchJoinMapping;
		private Guid _id;

		public ComponentsListFixture(bool fetchJoinMapping)
		{
			_fetchJoinMapping = fetchJoinMapping;
		}

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var tr = session.BeginTransaction();
			var root = new Entity();
			root.Entries.Add(new ComponentListEntry { ComponentReference = null, DummyString = "one", });

			session.Save(root);
			tr.Commit();
			_id = root.Id;
		}

		[Test]
		public void LazyLoading()
		{
			using var newSession = OpenSession();
			var reloadedRoot = newSession.Get<Entity>(_id);
			Assert.AreEqual(1, reloadedRoot.Entries.Count);
		}

		[Test]
		public void QueryOverFetch()
		{
			using var newSession = OpenSession();
			var reloadedRoot = newSession.QueryOver<Entity>()
				.Fetch(SelectMode.Fetch, x => x.Entries)
				.Where(x => x.Id == _id)
				.SingleOrDefault();
			Assert.AreEqual(1, reloadedRoot.Entries.Count);
		}

		[Test]
		public void LinqFetch()
		{
			using var newSession = OpenSession();
			var reloadedRoot = newSession.Query<Entity>()
				.Fetch(x => x.Entries)
				.Where(x => x.Id == _id)
				.SingleOrDefault();
			Assert.AreEqual(1, reloadedRoot.Entries.Count);
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.CreateSQLQuery("delete from Entries").ExecuteUpdate();
			session.Delete("from System.Object");
			transaction.Commit();
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				rc.Lazy(false);
				rc.Property(x => x.Name);

				rc.Bag(
					x => x.Entries,
					v =>
					{
						if (_fetchJoinMapping)
							v.Fetch(CollectionFetchMode.Join);
					},
					h => h.Component(cmp =>
					{
						cmp.Property(x => x.DummyString);
						cmp.ManyToOne(x => x.ComponentReference);
					}));
			});
			mapper.Class<EntityWithParent>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				rc.Lazy(false);
				rc.ManyToOne(x => x.Parent, m => m.NotNullable(true));
			});
			mapper.Class<ParentEntity>(rc =>
			{
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				rc.Lazy(false);
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
