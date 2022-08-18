using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2727
{
	/// <summary>
	/// Fixture using 'by code' mappings
	/// </summary>
	[TestFixture]
	public class LazyPropByCodeFixture : TestCaseMappingByCode
	{
		private Guid id1;
		private Guid id2;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(
				rc =>
				{
					rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
					rc.Property(x => x.Name);
					rc.Property(x => x.LazyProp, m => m.Lazy(true));
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
				session.Save(e2);

				transaction.Commit();
				id1 = e1.Id;
				id2 = e2.Id;
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// The HQL delete does all the job inside the database without loading the entities, but it does
				// not handle delete order for avoiding violating constraints if any. Use
				// session.Delete("from System.Object");
				// instead if in need of having NHbernate ordering the deletes, but this will cause
				// loading the entities in the session.
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void CanLoadUsingInstance()
		{
			using (var s = OpenSession())
			{
				var e1 = s.Load<Entity>(id1);
				s.Load(e1, id2);
			}
		}

		[Test(Description = "GH-2928")]
		public void CanSessionRefreshEntityWithLazyProperties()
		{
			using (var s = OpenSession())
			{
				var e1 = s.Get<Entity>(id1);
				s.Refresh(e1);
				s.Clear();
				s.Refresh(e1);
			}
		}
	}
}
