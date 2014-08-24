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
			hbmDynamicComponent.access.Should().Contain("field");
			hbmDynamicComponent.insert.Should().Be.False();
			hbmDynamicComponent.update.Should().Be.False();
			hbmDynamicComponent.optimisticlock.Should().Be.False();
			hbmDynamicComponent.unique.Should().Be.True();
		}
	}
}