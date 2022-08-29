using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	[TestFixture]
	public class ComponentPropertyOnDynamicCompoTests
	{
		private class Person
		{
			public int Id { get; set; }
			// Assigned by reflection
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private IDictionary info;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
			public IDictionary Info
			{
				get { return info; }
			}
		}

		private class PersonWithGenericInfo
		{
			public int Id { get; set; }
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private IDictionary<string, object> info;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
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
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => { });

			Assert.That(component.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "A" }));
		}

		[Test]
		public void WhenAddThenHasGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => { });

			Assert.That(component.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "A" }));
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			var called = false;
			mapper.Component(propertyInfo, (IComponentMapper x) => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenCustomizeThenCallCustomizerGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			var called = false;
			mapper.Component(propertyInfo, (IComponentMapper x) => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => x.Access(Accessor.Field));

			Assert.That(component.Properties.OfType<HbmComponent>().Single().Access, Is.Null.Or.Empty);
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnoreGeneric()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<PersonWithGenericInfo>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (MyClass) null }).GetType().GetProperty("A");

			mapper.Component(propertyInfo, (IComponentMapper x) => x.Access(Accessor.Field));

			Assert.That(component.Properties.OfType<HbmComponent>().Single().Access, Is.Null.Or.Empty);
		}
	}
}
