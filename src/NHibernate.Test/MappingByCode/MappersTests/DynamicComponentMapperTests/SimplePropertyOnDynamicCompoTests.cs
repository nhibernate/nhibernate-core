using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	[TestFixture]
	public class SimplePropertyOnDynamicCompoTests
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

		[Test]
		public void WhenAddThenHas()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = 5 }).GetType().GetProperty("A");

			mapper.Property(propertyInfo, x => { });

			Assert.That(component.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "A" }));
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = 5 }).GetType().GetProperty("A");
			var called = false;
			mapper.Property(propertyInfo, x => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = 5 }).GetType().GetProperty("A");

			mapper.Property(propertyInfo, x => x.Access(Accessor.Field));

			Assert.That(component.Properties.OfType<HbmProperty>().Single().Access, Is.Null.Or.Empty);
		}
	}
}
