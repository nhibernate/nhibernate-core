using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2905
{
	public class Fixture : TestCaseMappingByCode
	{
		private Guid _entity3Id;

		#region Test Configuration

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity1>(rc =>
			{
				rc.Id(x => x.Id, m =>
				{
					m.Generator(Generators.Guid);
					m.Column("id");
				});
				rc.ManyToOne(x => x.Entity2, m => m.Column("entity2_id"));
			});
			mapper.Class<Entity2>(rc =>
			{
				rc.Id(x => x.Id, m =>
				{
					m.Generator(Generators.Guid);
					m.Column("id");
				});
				rc.Set(x => x.Entity3s, m =>
				{
					m.Inverse(true);
					m.Key(km => km.Column("entity2_id"));
				},
				e => e.OneToMany(m => m.Class((typeof(Entity3)))));
			});
			mapper.Class<Entity3>(rc =>
			{
				rc.Id(x => x.Id, m =>
				{
					m.Generator(Generators.Guid);
					m.Column("id");
				});
				rc.ManyToOne(x => x.Entity2, m => m.Column("entity2_id"));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var entity1 = new Entity1();
				var entity2 = new Entity2();
				var entity3 = new Entity3();

				entity1.Entity2 = entity2;
				entity2.Entity3s.Add(entity3);
				entity3.Entity2 = entity2;

				session.Save(entity1);
				session.Save(entity2);
				session.Save(entity3);

				_entity3Id = entity3.Id;

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete("from Entity3");
				session.Delete("from Entity1");
				session.Delete("from Entity2");
				tx.Commit();
			}
		}

		#endregion

		[Test]
		public void JoinOverMultipleSteps_MethodSyntax_SelectAndSelectMany()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity1>()
					.Select(x => x.Entity2)
					.SelectMany(x => x.Entity3s)
					.Where(x => x.Id == _entity3Id)
					.ToList();

				tx.Commit();

				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].Id, Is.EqualTo(_entity3Id));
			}
		}

		[Test]
		public void JoinOverMultipleSteps_MethodSyntax_OnlySelectMany()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = session.Query<Entity1>()
					.SelectMany(x => x.Entity2.Entity3s)
					.Where(x => x.Id == _entity3Id)
					.ToList();

				tx.Commit();

				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].Id, Is.EqualTo(_entity3Id));
			}
		}

		[Test]
		public void JoinOverMultipleSteps_QuerySyntax_LetAndFrom()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = (from e1 in session.Query<Entity1>()
							  let e2 = e1.Entity2
							  from e3 in e2.Entity3s
							  where e3.Id == _entity3Id
							  select e3).ToList();

				tx.Commit();

				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].Id, Is.EqualTo(_entity3Id));
			}
		}

		[Test]
		public void JoinOverMultipleSteps_QuerySyntax_OnlyFrom()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var result = (from e1 in session.Query<Entity1>()
							  from e3 in e1.Entity2.Entity3s
							  where e3.Id == _entity3Id
							  select e3).ToList();

				tx.Commit();

				Assert.That(result.Count, Is.EqualTo(1));
				Assert.That(result[0].Id, Is.EqualTo(_entity3Id));
			}
		}
	}
}