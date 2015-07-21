using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
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
			Assert.That(hbmDynamicComponent.Properties.Select(x=> x.Name), Is.EquivalentTo(new [] {"MyInt", "MyDate"}));
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
		public void WhenMapDynCompoAttributesThenMapAttributes()
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(map =>
			{
				map.Id(x => x.Id, idmap => { });
				map.Component(x => x.Info, new { MyInt = 5}, z =>
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
			Assert.That(hbmDynamicComponent.access, Is.StringContaining("field"));
			Assert.That(hbmDynamicComponent.insert, Is.False);
			Assert.That(hbmDynamicComponent.update, Is.False);
			Assert.That(hbmDynamicComponent.optimisticlock, Is.False);
			Assert.That(hbmDynamicComponent.unique, Is.True);
		}
	}
}