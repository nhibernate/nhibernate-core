﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	using System.Threading.Tasks;

	[TestFixture]
	public class FixtureAsync : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.ByCode().LinqToHqlGeneratorsRegistry<EntityWithUserTypePropertyGeneratorsRegistry>();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new EntityWithUserTypeProperty
				{
					Id = 1,
					Name = "Bob",
					Example = new BarExample
					{
						Value = "Larry"
					}
				};
				session.Save(e1);

				var e2 = new EntityWithUserTypeProperty
				{
					Id = 2,
					Name = "Sally",
					Example = new FooExample
					{
						Value = "Larry"
					}
				};
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
				session.Delete("from EntityWithUserTypeProperty");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public async Task EqualityWorksForUserTypeAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = await (session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Example == newItem)
					.ToListAsync());

				Assert.AreEqual(1, entities.Count);
			}
		}

		[Test, Ignore("Not implemented yet")]
		public async Task LinqMethodWorksForUserTypeAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = await (session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Example.IsEquivalentTo(newItem))
					.ToListAsync());

				Assert.AreEqual(2, entities.Count);
			}
		}

		[Test]
		public async Task EqualityWorksForExplicitUserTypeAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = await (session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Example == newItem.MappedAs(NHibernateUtil.Custom(typeof(ExampleUserType))))
					.ToListAsync());

				Assert.AreEqual(1, entities.Count);
			}
		}

		[Test]
		public async Task LinqMethodWorksForExplicitUserTypeAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = await (session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Example.IsEquivalentTo(newItem.MappedAs(NHibernateUtil.Custom(typeof(ExampleUserType)))))
					.ToListAsync());

				Assert.AreEqual(2, entities.Count);
			}
		}

		[Test]
		public async Task LinqMethodWorksForStandardStringPropertyAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var entities = await (session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Name == "Bob")
					.ToListAsync());

				Assert.AreEqual(1, entities.Count);
			}
		}

		[Test]
		public async Task CanQueryWithHqlAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var q = session.CreateQuery("from EntityWithUserTypeProperty e where e.Example = :exampleItem");
				q.SetParameter("exampleItem", newItem);
				var entities = await (q.ListAsync<EntityWithUserTypeProperty>());

				Assert.AreEqual(1, entities.Count);
			}
		}
	}
}
