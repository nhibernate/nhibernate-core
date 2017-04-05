using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	public class MapPropertyOnDynamicCompoTests
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

		[Test]
		public void WhenAddThenHas()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (IDictionary<int, int>)null }).GetType().GetProperty("A");

			mapper.Map(propertyInfo, x => { }, km => { }, rel => { });

			Assert.That(component.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "A" }));
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (IDictionary<int, int>)null }).GetType().GetProperty("A");

			var called = false;
			mapper.Map(propertyInfo, x => called = true, km => { }, rel => { });

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (IDictionary<int, int>)null }).GetType().GetProperty("A");

			mapper.Map(propertyInfo, x => x.Access(Accessor.Field), km => { }, rel => { });

			Assert.That(component.Properties.OfType<HbmMap>().Single().Access, Is.Null.Or.Empty);
		}		
	}
}