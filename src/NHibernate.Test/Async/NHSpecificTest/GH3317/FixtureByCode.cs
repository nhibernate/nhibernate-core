﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3317
{
	using System.Threading.Tasks;
	[TestFixture(true)]
	[TestFixture(false)]
	public class ComponentsListFixtureAsync : TestCaseMappingByCode
	{
		private readonly bool _fetchJoinMapping;
		private Guid _id;

		public ComponentsListFixtureAsync(bool fetchJoinMapping)
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
		public async Task LazyLoadingAsync()
		{
			using var newSession = OpenSession();
			var reloadedRoot = await (newSession.GetAsync<Entity>(_id));
			Assert.AreEqual(1, reloadedRoot.Entries.Count);
		}

		[Test]
		public async Task QueryOverFetchAsync()
		{
			using var newSession = OpenSession();
			var reloadedRoot = await (newSession.QueryOver<Entity>()
				.Fetch(SelectMode.Fetch, x => x.Entries)
				.Where(x => x.Id == _id)
				.SingleOrDefaultAsync());
			Assert.AreEqual(1, reloadedRoot.Entries.Count);
		}

		[Test]
		public async Task LinqFetchAsync()
		{
			using var newSession = OpenSession();
			var reloadedRoot = await (newSession.Query<Entity>()
				.Fetch(x => x.Entries)
				.Where(x => x.Id == _id)
				.SingleOrDefaultAsync());
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
