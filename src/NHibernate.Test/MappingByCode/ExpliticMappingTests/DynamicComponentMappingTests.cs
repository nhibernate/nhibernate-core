using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class DynamicComponentMappingTests
	{
		private class Person
		{
			public int Id { get; set; }
			public IDictionary Info { get; set; }
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
			hbmDynamicComponent.Should().Not.Be.Null();
			hbmDynamicComponent.Properties.Select(x=> x.Name).Should().Have.SameValuesAs("MyInt", "MyDate");
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
			hbmDynamicComponent.Properties.OfType<HbmProperty>().Select(x => x.type1).All(x=> x.Satisfy(value=> !string.IsNullOrEmpty(value)));
		}
	}
}