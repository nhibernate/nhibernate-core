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
				rc.List(x => x.ChildrenList, c => c.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans), x => x.OneToMany());
				rc.Map(x => x.ChildrenMap, c => c.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans), x => x.OneToMany());
			});
			mapper.Class<ListChild>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name);
			});
			mapper.Class<MapChild>(rc =>
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
				var p = new Parent { Name = "Parent" };
				p.ChildrenList.Add(new ListChild { Name = "ListChild 1" });
				p.ChildrenList.Add(new ListChild { Name = "ListChild 2" });
				p.ChildrenList.Add(new ListChild { Name = "ListChild 3" });
				p.ChildrenMap.Add("first", new MapChild { Name = "MapChild 1" });
				p.ChildrenMap.Add("second", new MapChild { Name = "MapChild 2" });
				p.ChildrenMap.Add("third", new MapChild { Name = "MapChild 3" });

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
		public void Serializing_Session_After_Reordering_ChildrenList_Should_Work()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var p = session.Query<Parent>().Single();
					var c = p.ChildrenList.Last();
					p.ChildrenList.Remove(c);
					p.ChildrenList.Insert(p.ChildrenList.Count - 1, c);
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
		public void Serializing_Session_After_Deleting_First_Child_In_List_Should_Work()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var p = session.Query<Parent>().Single();
					p.ChildrenList.RemoveAt(0);
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
		public void Serializing_Session_After_Changing_Key_ChildrenMap_Should_Work()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var p = session.Query<Parent>().Single();
					var firstChild = p.ChildrenMap["first"];
					var secondChild = p.ChildrenMap["second"];
					p.ChildrenMap.Remove("first");
					p.ChildrenMap.Remove("second");
					p.ChildrenMap.Add("first", secondChild);
					p.ChildrenMap.Add("second", firstChild);
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