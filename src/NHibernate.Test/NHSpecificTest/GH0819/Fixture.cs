using System.Linq;
using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH0819
{
	[TestFixture]
	public class Fixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.List(
					x => x.Children,
					m =>
					{
						m.Cascade(Mapping.ByCode.Cascade.All);
						m.Key(k => k.Column("ParentId"));
						m.Index(i => i.Column("Idx"));
					},
					r => r.OneToMany());
			});
			mapper.Class<Child>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.ManyToOne(x => x.Parent);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Save(new Parent
				{
					Name = "Bob",
					Children = new List<Child>
					{
						new Child {Name = "Fred"},
						new Child {Name = "Anna"},
					}
				});

				var e2 = new Parent
				{
					Name = "Sally",
					Children = new List<Child>
					{
						new Child {Name = "Maria"},
						new Child {Name = "Theodor"},
					}
				};
				session.Save(e2);

				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from Child").ExecuteUpdate();
				session.CreateQuery("delete from Parent").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CanUseIndexerInQuery()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = (
					from e in session.Query<Parent>()
					where e.Children[0].Name == "Maria"
					select e
				).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
				transaction.Commit();
			}
		}
		
		[Test]
		public void CanUseElementAtInQuery()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var result = (
					from e in session.Query<Parent>()
					where e.Children.ElementAt(0).Name == "Maria"
					select e
				).ToList();

				Assert.That(result, Has.Count.EqualTo(1));
				transaction.Commit();
			}
		}
	}
}
