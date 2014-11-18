using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	public class DynComponentPropertyOnDynamicCompoTests
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
			var propertyInfo = For<Person>.Property(p => p.Info);//just as another dyn-compo

			mapper.Component(propertyInfo, (IDynamicComponentMapper x) => { });

			Assert.That(component.Properties.Select(x => x.Name), Is.EquivalentTo(new[] { "Info" }));
		}

		[Test]
		public void WhenCustomizeThenCallCustomizer()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = For<Person>.Property(p => p.Info);//just as another dyn-compo

			var called = false;
			mapper.Component(propertyInfo, (IDynamicComponentMapper x) => called = true);

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenCustomizeAccessorThenIgnore()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var propertyInfo = For<Person>.Property(p => p.Info);//just as another dyn-compo

			mapper.Component(propertyInfo, (IDynamicComponentMapper x) => x.Access(Accessor.Field));

			Assert.That(component.Properties.OfType<HbmDynamicComponent>().Single().Access, Is.Null.Or.Empty);
		}
	}
}