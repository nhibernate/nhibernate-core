using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitMappingTests
{
	[TestFixture]
	public class DynamicComponentMappingTests
	{
		private class Person
		{
			public int Id { get; set; }
			private IDictionary info;
			public IDictionary Info
			{
				get { return info; }
				set { info = value; }
			}
		}

		private class PersonWithGenericInfo
		{
			public int Id { get; set; }
			private IDictionary<string, object> info;
			public IDictionary<string, object> Info
			{
				get { return info; }
				set { info = value; }
			}
		}

		private class PersonWithDynamicInfo
		{
			public int Id { get; set; }
			public dynamic Info { get; set; }
		}

		[Test]
		public void WhenMapDynCompoByDictionaryThenMapItAndItsProperties()
		{
			//NH-3704
			var mapper = new ModelMapper();
			mapper.Class<Person>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Component(
						x => x.Info,
						new Dictionary<string, System.Type>
							{{"MyInt", typeof(int)}, {"MyDate", typeof(DateTime)}},
						z => { z.Property("MyInt", pm => pm.Column("MY_COLUMN")); });
				});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(
				hbmDynamicComponent.Properties.Select(x => x.Name),
				Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapDynCompoByDictionaryThenMapItAndItsPropertiesGeneric()
		{
			//NH-3704
			var mapper = new ModelMapper();
			mapper.Class<PersonWithGenericInfo>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Component(
						x => x.Info,
						new Dictionary<string, System.Type>
							{{"MyInt", typeof(int)}, {"MyDate", typeof(DateTime)}},
						z => { z.Property("MyInt", pm => pm.Column("MY_COLUMN")); });
				});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithGenericInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(
				hbmDynamicComponent.Properties.Select(x => x.Name),
				Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapDynCompoByDictionaryThenMapItAndItsPropertiesDynamic()
		{
			//NH-3704
			var mapper = new ModelMapper();
			mapper.Class<PersonWithDynamicInfo>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Component(
						nameof(PersonWithDynamicInfo.Info),
						new Dictionary<string, System.Type>
							{{"MyInt", typeof(int)}, {"MyDate", typeof(DateTime)}},
						z =>
						{
							z.Property("MyInt", pm => pm.Column("MY_COLUMN"));
							z.Component<DateTime>("MyDate");
						});
				});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithDynamicInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(
				hbmDynamicComponent.Properties.Select(x => x.Name),
				Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapPrivateDynCompoByDictionaryThenMapItAndItsProperties()
		{
			//NH-3704
			var mapper = new ModelMapper();
			mapper.Class<Person>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Component(
						"Info",
						new Dictionary<string, System.Type>
							{{"MyInt", typeof(int)}, {"MyDate", typeof(DateTime)}},
						z => { z.Property("MyInt", pm => pm.Column("MY_COLUMN")); });
				});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(
				hbmDynamicComponent.Properties.Select(x => x.Name),
				Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapPrivateDynCompoByDictionaryThenMapItAndItsPropertiesGeneric()
		{
			//NH-3704
			var mapper = new ModelMapper();
			mapper.Class<PersonWithGenericInfo>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Component(
						"Info",
						new Dictionary<string, System.Type>
							{{"MyInt", typeof(int)}, {"MyDate", typeof(DateTime)}},
						z => { z.Property("MyInt", pm => pm.Column("MY_COLUMN")); });
				});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithGenericInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(
				hbmDynamicComponent.Properties.Select(x => x.Name),
				Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapDynCompoThenMapItAndItsProperties()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5, MyDate = DateTime.Now }, z => { });
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(hbmDynamicComponent.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapDynCompoThenMapItAndItsPropertiesGeneric()
		{
			var mapper = new ModelMapper();
			mapper.Class<PersonWithGenericInfo>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5, MyDate = DateTime.Now }, z => { });
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithGenericInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent, Is.Not.Null);
			Assert.That(hbmDynamicComponent.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "MyInt", "MyDate" }));
		}

		[Test]
		public void WhenMapDynCompoPropertiesThenShouldAssignPropertyType()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5, MyDate = DateTime.Now }, z => { });
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().Single();
			Assert.That(hbmDynamicComponent.Properties.OfType<HbmProperty>().All(x => !string.IsNullOrEmpty(x.type1)), Is.True);
		}

		[Test]
		public void WhenMapDynCompoPropertiesThenShouldAssignPropertyTypeGeneric()
		{
			var mapper = new ModelMapper();
			mapper.Class<PersonWithGenericInfo>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5, MyDate = DateTime.Now }, z => { });
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithGenericInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().Single();
			Assert.That(hbmDynamicComponent.Properties.OfType<HbmProperty>().All(x => !string.IsNullOrEmpty(x.type1)), Is.True);
		}

		[Test]
		public void WhenMapDynCompoAttributesThenMapAttributes()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(
					x => x.Info,
					new { MyInt = 5 },
					z =>
					{
						z.Access(Accessor.Field);
						z.Insert(false);
						z.Update(false);
						z.Unique(true);
						z.OptimisticLock(false);
					});
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent.access, Does.Contain("field"));
			Assert.That(hbmDynamicComponent.insert, Is.False);
			Assert.That(hbmDynamicComponent.update, Is.False);
			Assert.That(hbmDynamicComponent.optimisticlock, Is.False);
			Assert.That(hbmDynamicComponent.unique, Is.True);
		}

		[Test]
		public void WhenMapDynCompoAttributesThenMapAttributesGeneric()
		{
			var mapper = new ModelMapper();
			mapper.Class<PersonWithGenericInfo>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5 }, z =>
				{
					z.Access(Accessor.Field);
					z.Insert(false);
					z.Update(false);
					z.Unique(true);
					z.OptimisticLock(false);
				});
			});

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(PersonWithGenericInfo) });
			var hbmClass = hbmMapping.RootClasses[0];
			var hbmDynamicComponent = hbmClass.Properties.OfType<HbmDynamicComponent>().SingleOrDefault();
			Assert.That(hbmDynamicComponent.access, Does.Contain("field"));
			Assert.That(hbmDynamicComponent.insert, Is.False);
			Assert.That(hbmDynamicComponent.update, Is.False);
			Assert.That(hbmDynamicComponent.optimisticlock, Is.False);
			Assert.That(hbmDynamicComponent.unique, Is.True);
		}
	}
}
