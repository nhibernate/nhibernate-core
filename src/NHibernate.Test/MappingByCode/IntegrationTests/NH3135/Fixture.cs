using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3135
{
	[TestFixture]
	public class MappingByCodeTest
	{
		[Test]
		public void Bag_InBaseEntity_WithDifferentTables_ShouldBeMappedAccordingly()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity1>(cm =>
			{
				cm.Table("Legacy1");
				cm.Id(p => p.Id);
				cm.Property(p => p.Foo);
				cm.Bag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy1_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity2>(cm =>
			{
				cm.Table("Legacy2");
				cm.Id(p => p.Id);
				cm.Property(p => p.Bar);
				cm.Bag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy2_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity3>(cm =>
			{
				cm.Id(p => p.Id);
				cm.Bag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
				cm.Property(p => p.Baz);
			});
			//set some defaults: 
			//our bags are by default stored in a table called RootType_Component (unless overridden by the mapping)
			mapper.BeforeMapBag += (mi, m, pc) => pc.Table(m.GetRootMember().ReflectedType.Name + "_Component");
			//all entities are by default stored in a table called Type
			mapper.BeforeMapClass += (mi, t, pc) => pc.Table(t.Name);
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var classMappings = mappings.Items.OfType<HbmClass>();
			Assert.That(classMappings.Count(), Is.EqualTo(3));
			var e1Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity1).Name);
			Assert.That(e1Mapping, Is.Not.Null);
			Assert.That(e1Mapping.table, Is.EqualTo("Legacy1"));

			var e1Bag = e1Mapping.Items.OfType<HbmBag>().FirstOrDefault();
			Assert.That(e1Bag, Is.Not.Null);
			Assert.That(e1Bag.Table, Is.EqualTo("Legacy1_Comp"));

			var e2Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity2).Name);
			Assert.That(e2Mapping, Is.Not.Null);
			Assert.That(e2Mapping.table, Is.EqualTo("Legacy2"));

			var e2Bag = e2Mapping.Items.OfType<HbmBag>().FirstOrDefault();
			Assert.That(e2Bag, Is.Not.Null);
			Assert.That(e2Bag.Table, Is.EqualTo("Legacy2_Comp"));

			var e3Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity3).Name);
			Assert.That(e3Mapping, Is.Not.Null);
			Assert.That(e3Mapping.table, Is.EqualTo("Entity3"));

			var e3Bag = e3Mapping.Items.OfType<HbmBag>().FirstOrDefault();
			Assert.That(e3Bag, Is.Not.Null);
			Assert.That(e3Bag.Table, Is.EqualTo("Entity3_Component"));
		}
		
		[Test]
		public void Set_InBaseEntity_WithDifferentTables_ShouldBeMappedAccordingly()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity1>(cm =>
			{
				cm.Table("Legacy1");
				cm.Id(p => p.Id);
				cm.Property(p => p.Foo);
				cm.Set(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy1_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity2>(cm =>
			{
				cm.Table("Legacy2");
				cm.Id(p => p.Id);
				cm.Property(p => p.Bar);
				cm.Set(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy2_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity3>(cm =>
			{
				cm.Id(p => p.Id);
				cm.Set(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
				cm.Property(p => p.Baz);
			});
			//set some defaults: 
			//our bags are by default stored in a table called RootType_Component (unless overridden by the mapping)
			mapper.BeforeMapSet += (mi, m, pc) => pc.Table(m.GetRootMember().ReflectedType.Name + "_Component");
			//all entities are by default stored in a table called Type
			mapper.BeforeMapClass += (mi, t, pc) => pc.Table(t.Name);
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var classMappings = mappings.Items.OfType<HbmClass>();
			Assert.That(classMappings.Count(), Is.EqualTo(3));
			var e1Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity1).Name);
			Assert.That(e1Mapping, Is.Not.Null);
			Assert.That(e1Mapping.table, Is.EqualTo("Legacy1"));

			var e1Bag = e1Mapping.Items.OfType<HbmSet>().FirstOrDefault();
			Assert.That(e1Bag, Is.Not.Null);
			Assert.That(e1Bag.Table, Is.EqualTo("Legacy1_Comp"));

			var e2Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity2).Name);
			Assert.That(e2Mapping, Is.Not.Null);
			Assert.That(e2Mapping.table, Is.EqualTo("Legacy2"));

			var e2Bag = e2Mapping.Items.OfType<HbmSet>().FirstOrDefault();
			Assert.That(e2Bag, Is.Not.Null);
			Assert.That(e2Bag.Table, Is.EqualTo("Legacy2_Comp"));

			var e3Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity3).Name);
			Assert.That(e3Mapping, Is.Not.Null);
			Assert.That(e3Mapping.table, Is.EqualTo("Entity3"));

			var e3Bag = e3Mapping.Items.OfType<HbmSet>().FirstOrDefault();
			Assert.That(e3Bag, Is.Not.Null);
			Assert.That(e3Bag.Table, Is.EqualTo("Entity3_Component"));
		}

		[Test]
		public void List_InBaseEntity_WithDifferentTables_ShouldBeMappedAccordingly()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity1>(cm =>
			{
				cm.Table("Legacy1");
				cm.Id(p => p.Id);
				cm.Property(p => p.Foo);
				cm.List(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy1_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity2>(cm =>
			{
				cm.Table("Legacy2");
				cm.Id(p => p.Id);
				cm.Property(p => p.Bar);
				cm.List(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy2_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity3>(cm =>
			{
				cm.Id(p => p.Id);
				cm.List(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
				cm.Property(p => p.Baz);
			});
			//set some defaults: 
			//our bags are by default stored in a table called RootType_Component (unless overridden by the mapping)
			mapper.BeforeMapList += (mi, m, pc) => pc.Table(m.GetRootMember().ReflectedType.Name + "_Component");
			//all entities are by default stored in a table called Type
			mapper.BeforeMapClass += (mi, t, pc) => pc.Table(t.Name);
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var classMappings = mappings.Items.OfType<HbmClass>();
			Assert.That(classMappings.Count(), Is.EqualTo(3));
			var e1Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity1).Name);
			Assert.That(e1Mapping, Is.Not.Null);
			Assert.That(e1Mapping.table, Is.EqualTo("Legacy1"));

			var e1Bag = e1Mapping.Items.OfType<HbmList>().FirstOrDefault();
			Assert.That(e1Bag, Is.Not.Null);
			Assert.That(e1Bag.Table, Is.EqualTo("Legacy1_Comp"));

			var e2Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity2).Name);
			Assert.That(e2Mapping, Is.Not.Null);
			Assert.That(e2Mapping.table, Is.EqualTo("Legacy2"));

			var e2Bag = e2Mapping.Items.OfType<HbmList>().FirstOrDefault();
			Assert.That(e2Bag, Is.Not.Null);
			Assert.That(e2Bag.Table, Is.EqualTo("Legacy2_Comp"));

			var e3Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity3).Name);
			Assert.That(e3Mapping, Is.Not.Null);
			Assert.That(e3Mapping.table, Is.EqualTo("Entity3"));

			var e3Bag = e3Mapping.Items.OfType<HbmList>().FirstOrDefault();
			Assert.That(e3Bag, Is.Not.Null);
			Assert.That(e3Bag.Table, Is.EqualTo("Entity3_Component"));
		}

		[Test]
		public void IdBag_InBaseEntity_WithDifferentTables_ShouldBeMappedAccordingly()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity1>(cm =>
			{
				cm.Table("Legacy1");
				cm.Id(p => p.Id);
				cm.Property(p => p.Foo);
				cm.IdBag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy1_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity2>(cm =>
			{
				cm.Table("Legacy2");
				cm.Id(p => p.Id);
				cm.Property(p => p.Bar);
				cm.IdBag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
						m.Table("Legacy2_Comp");
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
			});
			mapper.Class<Entity3>(cm =>
			{
				cm.Id(p => p.Id);
				cm.IdBag(p => p.ComponentCollection,
					m =>
					{
						m.Access(Accessor.Field);
						m.Fetch(CollectionFetchMode.Select);
					},
					m => m.Component(c =>
					{
						c.Property(p => p.Property1);
						c.Property(p => p.Property2);
					}));
				cm.Property(p => p.Baz);
			});
			//set some defaults: 
			//our bags are by default stored in a table called RootType_Component (unless overridden by the mapping)
			mapper.BeforeMapIdBag += (mi, m, pc) => pc.Table(m.GetRootMember().ReflectedType.Name + "_Component");
			//all entities are by default stored in a table called Type
			mapper.BeforeMapClass += (mi, t, pc) => pc.Table(t.Name);
			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var classMappings = mappings.Items.OfType<HbmClass>();
			Assert.That(classMappings.Count(), Is.EqualTo(3));
			var e1Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity1).Name);
			Assert.That(e1Mapping, Is.Not.Null);
			Assert.That(e1Mapping.table, Is.EqualTo("Legacy1"));

			var e1Bag = e1Mapping.Items.OfType<HbmIdbag>().FirstOrDefault();
			Assert.That(e1Bag, Is.Not.Null);
			Assert.That(e1Bag.Table, Is.EqualTo("Legacy1_Comp"));

			var e2Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity2).Name);
			Assert.That(e2Mapping, Is.Not.Null);
			Assert.That(e2Mapping.table, Is.EqualTo("Legacy2"));

			var e2Bag = e2Mapping.Items.OfType<HbmIdbag>().FirstOrDefault();
			Assert.That(e2Bag, Is.Not.Null);
			Assert.That(e2Bag.Table, Is.EqualTo("Legacy2_Comp"));

			var e3Mapping = classMappings.FirstOrDefault(c => c.Name == typeof(Entity3).Name);
			Assert.That(e3Mapping, Is.Not.Null);
			Assert.That(e3Mapping.table, Is.EqualTo("Entity3"));

			var e3Bag = e3Mapping.Items.OfType<HbmIdbag>().FirstOrDefault();
			Assert.That(e3Bag, Is.Not.Null);
			Assert.That(e3Bag.Table, Is.EqualTo("Entity3_Component"));
		}
	}
}
