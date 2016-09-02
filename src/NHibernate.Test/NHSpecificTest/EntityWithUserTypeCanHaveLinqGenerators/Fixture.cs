using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Test.NHSpecificTest.FileStreamSql2008;
using NUnit.Framework;


namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{

	[TestFixture]
	public class Fixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return true;
		}

		protected override void Configure(Configuration cfg)
		{

			base.Configure(cfg);
			cfg.LinqToHqlGeneratorsRegistry<EntityWithUserTypePropertyGeneratorsRegistry>();
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
		public void EqualityWorksForUserType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = session.Query<EntityWithUserTypeProperty>()
					.Where(x=>x.Example == newItem)
					.ToList();

				Assert.AreEqual(1, entities.Count);
			}
		}

		[Test]
		public void LinqMethodWorksForUserType()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var entities = session.Query<EntityWithUserTypeProperty>()
					.Where(x => x.Example.IsEquivalentTo(newItem))
					.ToList();

				Assert.AreEqual(2, entities.Count);
			}
		}
		[Test]
		public void CanQueryWithHQL()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var newItem = new BarExample { Value = "Larry" };
				var q = session.CreateQuery("from EntityWithUserTypeProperty e where e.Example = :exampleItem");
				q.SetParameter("exampleItem", newItem);
				var entities = q.List<EntityWithUserTypeProperty>();

				Assert.AreEqual(1, entities.Count);
			}
		}
	}
}