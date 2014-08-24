using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.Generic;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	public class SetPropertyOnDynamicCompoTests
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
			var propertyInfo = (new { A = (IEnumerable<int>)null }).GetType().GetProperty("A");

			mapper.Set(propertyInfo, x => { }, rel => { });

			component.Properties.Select(x => x.Name).Should().Have.SameSequenceAs("A");
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (IEnumerable<int>)null }).GetType().GetProperty("A");

			var called = false;
			mapper.Set(propertyInfo, x => called = true, rel => { });

			called.Should().Be.True();
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = (new { A = (IEnumerable<int>)null }).GetType().GetProperty("A");

			mapper.Set(propertyInfo, x => x.Access(Accessor.Field), rel => { });

			component.Properties.OfType<HbmSet>().Single().Access.Should().Be.NullOrEmpty();
		}		
	}
}