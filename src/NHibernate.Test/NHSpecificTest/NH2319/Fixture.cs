using System;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Collection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2319
{
	[TestFixture]
	public abstract class FixtureBase : TestCaseMappingByCode
	{
		private Guid _parentId;
		private Guid _child1Id;

		[Test]
		public void ShouldBeAbleToFindChildrenByName()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Get<Parent>(_parentId);

				Assert.That(parent, Is.Not.Null);

				var filtered = parent.Children
					.AsQueryable()
					.Where(x => x.Name == "Jack")
					.ToList();

				Assert.That(filtered, Has.Count.EqualTo(1));
				Assert.That(filtered[0].Id, Is.EqualTo(_child1Id));
			}
		}

		[Test]
		public void ShouldBeAbleToPerformComplexFiltering()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Get<Parent>(_parentId);

				Assert.NotNull(parent);

				var filtered = parent.Children
					.AsQueryable()
					.Where(x => x.Name == "Piter")
					.SelectMany(x => x.GrandChildren)
					.Select(x => x.Id)
					.Count();

				Assert.That(filtered, Is.EqualTo(2));
			}
		}

		[Test]
		public void ShouldNotInitializeCollectionWhenPerformingQuery()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Get<Parent>(_parentId);
				Assert.That(parent, Is.Not.Null);

				var persistentCollection = (IPersistentCollection) parent.Children;

				var filtered = parent.Children
					.AsQueryable()
					.Where(x => x.Name == "Jack")
					.ToList();

				Assert.That(filtered, Has.Count.EqualTo(1));
				Assert.That(persistentCollection.WasInitialized, Is.False);
			}
		}

		[Test]
		public void ShouldPerformSqlQueryEvenIfCollectionAlreadyInitialized()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Get<Parent>(_parentId);
				Assert.That(parent, Is.Not.Null);

				var loaded = parent.Children.ToList();
				Assert.That(loaded, Has.Count.EqualTo(2));

				var countBeforeFiltering = session.SessionFactory.Statistics.QueryExecutionCount;

				var filtered = parent.Children
					.AsQueryable()
					.Where(x => x.Name == "Jack")
					.ToList();

				var countAfterFiltering = session.SessionFactory.Statistics.QueryExecutionCount;

				Assert.That(filtered, Has.Count.EqualTo(1));
				Assert.That(countAfterFiltering, Is.EqualTo(countBeforeFiltering + 1));
			}
		}

		[Test]
		public void TestFilter()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = session.Get<Parent>(_parentId);
				Assert.That(parent, Is.Not.Null);

				var children = session.CreateFilter(parent.Children, "where this.Name = 'Jack'")
					.List<Child>();

				Assert.That(children, Has.Count.EqualTo(1));
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty("show_sql", "true");
			configuration.SetProperty("generate_statistics", "true");
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var parent1 = new Parent {Name = "Bob"};
				_parentId = (Guid) session.Save(parent1);

				var parent2 = new Parent {Name = "Martin"};
				session.Save(parent2);

				var child1 = new Child
				{
					Name = "Jack",
					Parent = parent1
				};
				parent1.Children.Add(child1);
				_child1Id = (Guid) session.Save(child1);

				var child2 = new Child
				{
					Name = "Piter",
					Parent = parent1
				};
				parent1.Children.Add(child2);
				session.Save(child2);

				var grandChild1 = new GrandChild
				{
					Name = "Kate",
					Child = child2
				};
				child2.GrandChildren.Add(grandChild1);
				session.Save(grandChild1);

				var grandChild2 = new GrandChild
				{
					Name = "Mary",
					Child = child2
				};
				child2.GrandChildren.Add(grandChild2);
				session.Save(grandChild2);

				var child3 = new Child
				{
					Name = "Jack",
					Parent = parent2
				};
				parent2.Children.Add(child3);
				session.Save(child3);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}
	}

	[TestFixture]
	public class BagFixture : FixtureBase
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Bag(x => x.Children,
					   map => map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans),
					   rel => rel.OneToMany());
			});

			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
				rc.Bag(x => x.GrandChildren, map => map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans), rel => rel.OneToMany());
			});

			mapper.Class<GrandChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Child, x => x.Column("child_id"));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

	}

	[TestFixture]
	public class SetFixture : FixtureBase
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Set(x => x.Children,
					   map => map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans),
					   rel => rel.OneToMany());
			});

			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
				rc.Set(x => x.GrandChildren,
					   map => map.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans),
					   rel => rel.OneToMany());
			});

			mapper.Class<GrandChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Child, x => x.Column("child_id"));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}

	[TestFixture]
	public class ListFixture : FixtureBase
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
					list =>
					{
						list.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						list.Index(i => i.Column("i"));
					},
					rel => rel.OneToMany());
			});

			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Parent);
				rc.List(
					c => c.GrandChildren,
					list =>
					{
						list.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						list.Index(i => i.Column("i"));
					},
					rel => rel.OneToMany());
			});

			mapper.Class<GrandChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(x => x.Child, x => x.Column("child_id"));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}

	[TestFixture]
	public class IdBagFixture : FixtureBase
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Parent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.IdBag(
					x => x.Children,
					list =>
					{
						list.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					},
					rel => rel.ManyToMany());
			});

			mapper.Class<Child>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				//rc.ManyToOne(x => x.Parent);
				rc.IdBag(
					c => c.GrandChildren,
					list =>
					{
						list.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					},
					rel => rel.ManyToMany());
			});

			mapper.Class<GrandChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				//rc.ManyToOne(x => x.Child, x => x.Column("child_id"));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
