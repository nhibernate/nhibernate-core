using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.DynamicComponentMapperTests
{
	public class DynCompAttributesSettingTest
	{
		private class Person
		{
			public int Id { get; set; }
			private IDictionary info = null;
			public IDictionary Info
			{
				get { return info; }
			}
		}

		[Test]
		public void CanSetAccessor()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);

			mapper.Access(Accessor.Field);
			component.access.Should().Be("field.camelcase");
		}

		[Test]
		public void CanSetUpdate()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);

			mapper.Update(false);
			component.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);

			mapper.Insert(false);
			component.insert.Should().Be.False();
		}

		[Test]
		public void CanSetOptimisticLock()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);

			mapper.OptimisticLock(false);
			component.OptimisticLock.Should().Be.False();
		}

		[Test]
		public void CanAddSimpleProperty()
		{
			var mapdoc = new HbmMapping();
			var component = new HbmDynamicComponent();
			var mapper = new DynamicComponentMapper(component, For<Person>.Property(p => p.Info), mapdoc);
			var dynObject = new { Pizza = 5 };
			mapper.Property(dynObject.GetType().GetProperty("Pizza"), x => { });

			component.Properties.Single().Should().Be.OfType<HbmProperty>().And.ValueOf.Name.Should().Be.EqualTo("Pizza");
		}

	}
}