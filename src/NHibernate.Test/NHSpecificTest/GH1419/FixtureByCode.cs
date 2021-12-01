using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1419
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		private Guid ParentId;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EntityParent>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
				rc.ManyToOne(ep => ep.Child);
				rc.ManyToOne(ep => ep.ChildAssigned);
			});

			mapper.Class<EntityChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			mapper.Class<EntityChildAssigned>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var child = new EntityChild { Name = "InitialChild" };

				var assigned = new EntityChildAssigned { Id = 1, Name = "InitialChild" };

				var parent = new EntityParent
				{
					Name = "InitialParent",
					Child = child,
					ChildAssigned = assigned
				};
				session.Save(child);
				session.Save(parent);
				session.Save(assigned);

				session.Flush();
				transaction.Commit();
				ParentId = parent.Id;
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
		public void SessionIsDirtyShouldNotFailForNewManyToOneObject()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = GetParent(session);

				//parent.Child entity is not cascaded, I want to save it explictilty later
				parent.Child = new EntityChild { Name = "NewManyToOneChild" };

				var isDirty = false;
				Assert.That<bool>(() => isDirty = session.IsDirty(), Throws.Nothing, "ISession.IsDirty() call should not fail for transient  many-to-one object referenced in session.");
				Assert.That(isDirty, "ISession.IsDirty() call should return true.");
			}
		}

		[Test]
		public void SessionIsDirtyShouldNotFailForNewManyToOneObjectWithAssignedId()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var parent = GetParent(session);

				//parent.ChildAssigned entity is not cascaded, I want to save it explictilty later
				parent.ChildAssigned = new EntityChildAssigned { Id = 2, Name = "NewManyToOneChildAssignedId" };

				var isDirty = false;
				Assert.That<bool>(() => isDirty = session.IsDirty(), Throws.Nothing, "ISession.IsDirty() call should not fail for transient  many-to-one object referenced in session.");
				Assert.That(isDirty, "ISession.IsDirty() call should return true.");
			}
		}

		private EntityParent GetParent(ISession session)
		{
			return session.Get<EntityParent>(ParentId);
		}
	}
}
