using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3918
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Owner>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(m => m.Owner, m =>
				{
					m.Column("OwnerId");
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var bob =  CreateOwner(session, "Bob");
				var carl = CreateOwner(session, "Carl");
				var doug = CreateOwner(session, "Doug");

				CreateEntity(session, "Test 1", bob);
				CreateEntity(session, "Test 2", carl);
				CreateEntity(session, "Test 3", doug);
				CreateEntity(session, "Test 4", bob);
				CreateEntity(session, "Test 5", carl);
				CreateEntity(session, "Test 6", doug);

				session.Flush();
				transaction.Commit();
			}
		}

		protected Owner CreateOwner(ISession session, string name)
		{
			var t = new Owner { Name = name };
			session.Save(t);
			return t;
		}

		protected void CreateEntity(ISession session, string name, Owner owner)
		{
			var t = new Entity
			{
				Name = name,
				Owner = owner,
			};
			session.Save(t);
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from Entity");
				session.Delete("from Owner");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void EntityComparisonTest()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var bob = session.Query<Owner>().Single(o => o.Name == "Bob");

				var queryWithWhere = session.Query<Entity>()
										.Where(WhereExpression(bob))
										.Select(e => e.Name);
				var queryWithSelect = session.Query<Entity>()
										.Select(SelectExpression(bob));

				var resultsFromWhere = queryWithWhere.ToList();
				var resultsFromSelect = queryWithSelect.ToList();

				Assert.That(resultsFromSelect.Where(x => (bool)x[1]).Select(x => (string)x[0]), Is.EquivalentTo(resultsFromWhere));
			}
		}

		[Test]
		public void EntityComparisonTestAgain()
		{
			// When the entire fixture is run this will execute the test again within the same ISessionFactory which will test caching
			EntityComparisonTest();
		}

		protected Expression<Func<Entity, bool>> WhereExpression(Owner owner)
		{
			return e => e.Owner == owner;
		}

		protected Expression<Func<Entity, object[]>> SelectExpression(Owner owner)
		{
			return e => new object[]
			{
				e.Name,
				e.Owner == owner
			};
		}
	}
}
