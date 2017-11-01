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
			});

			mapper.Class<EntityChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var child = new EntityChild { Name = "InitialChild" };
				var parent = new EntityParent
				{
					Name = "InitialParent",
					Child = child
				};
				session.Save(child);
				session.Save(parent);
				
				session.Flush();
				transaction.Commit();
				ParentId = parent.Id;
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void SessionIsDirtyShouldNotFailForNewManyToOneObject()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				EntityParent parent = GetParent(session);

				//parent.Child entity is not cascaded, I want to save it explictilty later
				parent.Child = new EntityChild { Name = "NewManyToOneChild" };

				bool isDirty = false;
				Assert.That(() => isDirty = session.IsDirty(), Throws.Nothing, "ISession.IsDirty() call should not fail for transient  many-to-one object referenced in session.");
				Assert.That(isDirty, "ISession.IsDirty() call should return true.");
			}
		}

		private EntityParent GetParent(ISession session)
		{
			return session.Get<EntityParent>(ParentId);
		}
	}
}
