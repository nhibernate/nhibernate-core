using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1738
{
	[TestFixture]
	public class RefreshLocallyRemovedCollectionItemFixture : TestCaseMappingByCode
	{
		private Guid _id;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.Bag(
					x => x.Children,
					m =>
					{
						m.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
						m.Inverse(true);
						m.Key(km => km.Column("Parent"));
					},
					relation => relation.OneToMany());
			});

			mapper.Class<Child>(rc =>
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
				var e1 = new Entity { Name = "Bob" };
				e1.Children.Add(new Child() { Name = "Child", Parent = e1 });
				session.Save(e1);
				transaction.Commit();
				_id = e1.Id;
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				transaction.Commit();
			}
		}

		[Test]
		public void RefreshLocallyRemovedCollectionItem()
		{
			Entity entity;
			using (var session = OpenSession())
			{
				entity = session.Get<Entity>(_id);
				entity.Children.RemoveAt(0);
			}

			using (var session = OpenSession())
			{
				session.Update(entity);
				session.Refresh(entity);
				foreach (var child in entity.Children)
				{
					session.Refresh(child);
				}

				Assert.That(session.GetSessionImplementation().PersistenceContext.IsReadOnly(entity), Is.False);
			}
		}
	}
}
