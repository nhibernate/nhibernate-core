using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1413
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private int _parentId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityParent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Native));
				rc.Property(x => x.Name);
				rc.Bag(x => x.Children, m =>
				{
					m.Cascade(Mapping.ByCode.Cascade.All);
					m.Inverse(true);
				}, a => a.OneToMany()
				);
			});

			mapper.Class<EntityChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Native));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var parent = new EntityParent
				{
					Name = "InitialParent",
				};
				session.Save(parent);

				session.Flush();
				transaction.Commit();
				_parentId = parent.Id;
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

		[Test]
		[KnownBug("#1413")]
		public void SessionIsDirtyShouldNotTriggerCascadeSaving()
		{
			Sfi.Statistics.IsStatisticsEnabled = true;
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = GetParent(session);
				var entityChild = new EntityChild
				{
					Name = "NewListElem"
				};

				//parent.Children is cascaded
				parent.Children.Add(entityChild);

				Sfi.Statistics.Clear();
				var isDirty = session.IsDirty();

				Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(0), "Dirty has triggered an insert");
				Assert.That(
					entityChild.Id,
					Is.EqualTo(0),
					"Transient objects should not be saved by ISession.IsDirty() call (expected 0 as Id)");
				Assert.That(isDirty, "ISession.IsDirty() call should return true.");
			}
		}

		private EntityParent GetParent(ISession session)
		{
			return session.Get<EntityParent>(_parentId);
		}
	}
}
