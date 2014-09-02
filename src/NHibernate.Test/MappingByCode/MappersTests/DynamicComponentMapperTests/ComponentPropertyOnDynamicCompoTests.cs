using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	public class ComponentPropertyOnDynamicCompoTests
	{
		private class Person
		{
			public int Id { get; set; }
			private IDictionary info;
			public IDictionary Info
			{
				get { return info; }
			}
		}

		private class PersonWithGenericInfo
		{
			public int Id { get; set; }
			private IDictionary<string, object> info;
			public IDictionary<string, object> Info
			{
				get { return info; }
			}
		}

		private class MyClass
		{
			public int Something { get; set; }
		}
		[Test]
		public void WhenAddThenHas()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => { });

			component.Properties.Select(x => x.Name).Should().Have.SameSequenceAs("A");
		}

		[Test]
		public void WhenAddThenHasGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => { });

			component.Properties.Select(x => x.Name).Should().Have.SameSequenceAs("A");
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			var called = false;
			mapper.Component(propertyInfo, (IComponentMapper x) => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void WhenCustomizeThenCallCustomizerGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			var called = false;
			mapper.Component(propertyInfo, (IComponentMapper x) => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => x.Access(Accessor.Field));

			component.Properties.OfType<HbmComponent>().Single().Access.Should().Be.NullOrEmpty();
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnoreGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass)null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => x.Access(Accessor.Field));

			component.Properties.OfType<HbmComponent>().Single().Access.Should().Be.NullOrEmpty();
		}
	}
}