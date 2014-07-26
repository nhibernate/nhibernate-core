using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3316
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(e =>
			{
				e.Id(x => x.Id, id => id.Generator(Generators.GuidComb));
				e.Set(x => x.Children, c =>
				{
					c.Key(key => key.Column("EntityId"));
					c.Cascade(NHibernate.Mapping.ByCode.Cascade.All | NHibernate.Mapping.ByCode.Cascade.DeleteOrphans);
				},
				r =>
				{
					r.Component(c =>
					{
						c.Parent(x => x.Parent);
						c.Property(x => x.Value, pm=>pm.Column("`Value`"));
					});
				});
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
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
		public void Test_That_Parent_Property_Can_Be_Persisted_And_Retrieved()
		{
			Guid id = Guid.Empty;
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Entity e = new Entity();
				e.AddChild(1);
				e.AddChild(2);

				session.Save(e);
				session.Flush();
				transaction.Commit();
				id = e.Id;
			}

			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				Entity e = session.Get<Entity>(id);
				Assert.AreEqual(2, e.Children.Count());
				foreach (ChildComponent c in e.Children)
					Assert.AreEqual(e, c.Parent);

				session.Flush();
				transaction.Commit();
			}
		}
	}
}