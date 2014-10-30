using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3731
{
	public class ByCodeFixture : TestCaseMappingByCode
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
					c =>
					{
						c.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
					},
					x => x.OneToMany()
				);
			});
			mapper.Class<Child>(rc =>
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
				var c1 = new Child { Name = "Child 1" };
				var c2 = new Child { Name = "Child 2" };
				var c3 = new Child { Name = "Child 3" };

				var p = new Parent { Name = "Parent" };
				p.Children.Add(c1);
				p.Children.Add(c2);
				p.Children.Add(c3);

				session.Save(p);

				session.Flush();
				transaction.Commit();
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
		public void Serializing_Session_After_Reordering_Children_Should_Work()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var p = session.Query<Parent>().Single();
					var c = p.Children.Last();
					p.Children.Remove(c);
					p.Children.Insert(p.Children.Count - 1, c);
					session.Flush();
					transaction.Commit();
				}

				using (MemoryStream stream = new MemoryStream())
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, session);

					Assert.AreNotEqual(0, stream.Length);
				}
			}
		}

		[Test]
		public void Serializing_Session_After_Deleting_First_Child_Should_Work()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var p = session.Query<Parent>().Single();
					p.Children.RemoveAt(0);
					session.Flush();
					transaction.Commit();
				}

				using (MemoryStream stream = new MemoryStream())
				{
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(stream, session);

					Assert.AreNotEqual(0, stream.Length);
				}
			}
		}
	}
}