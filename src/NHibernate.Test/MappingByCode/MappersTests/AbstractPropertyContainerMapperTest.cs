using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class AbstractPropertyContainerMapperTest
	{
		private class EntitySimple
		{
			public string Name { get; set; }
		}
		private class InheritedEntitySimple : EntitySimple
		{
		}

		private class OtherSimple
		{
			public string Name { get; set; }
		}

		private class MyClass
		{
			public object Reference { get; set; }
		}
		private class MyClassWithDictionary
		{
			public IDictionary<string, string> Dictionary { get; set; }
		}
		private class MyClassWithDynamic
		{
			public IDictionary DynCompo { get; set; }
		}

		[Test]
		public void CantCreateWithoutHbmMapping()
		{
			Executing.This(() => new HackPropertyContainerMapper(typeof(EntitySimple), null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void CantCreateWithoutContainerType()
		{
			Executing.This(() => new HackPropertyContainerMapper(null, new HbmMapping())).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void CanAddSimpleProperty()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<EntitySimple>(properties);
			map.Property(typeof (EntitySimple).GetProperty("Name"), x => { });
		
			properties.Should().Have.Count.EqualTo(1);
			properties.First().Should().Be.OfType<HbmProperty>().And.ValueOf.Name.Should().Be.EqualTo("Name");
		}

		[Test]
		public void CallPropertyMapper()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<EntitySimple>(properties);
			var called = false;
			map.Property(typeof(EntitySimple).GetProperty("Name"), x => called = true );

			called.Should().Be.True();
		}

		[Test]
		public void CantAddPropertyOfNotInheritedType()
		{
			var map = new StubPropertyContainerMapper<OtherSimple>(new List<object>());
			Executing.This(() => map.Property(typeof(EntitySimple).GetProperty("Name"), x => { })).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanAddPropertyOfInheritedType()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<InheritedEntitySimple>(properties);
			map.Property(typeof(InheritedEntitySimple).GetProperty("Name"), x => { });

			properties.Should().Have.Count.EqualTo(1);
			properties.First().Should().Be.OfType<HbmProperty>().And.ValueOf.Name.Should().Be.EqualTo("Name");
		}

		[Test]
		public void CallAnyMapper()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<MyClass>(properties);
			var called = false;
			map.Any(typeof(MyClass).GetProperty("Reference"), typeof(int), x => called = true);

			called.Should().Be.True();
		}

		[Test]
		public void CallDictionaryMappers()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<EntitySimple>(properties);
			var collectionPropsCalled = false;
			var keyRelationCalled = false;
			var elementRelationCalled = false;
			map.Map(typeof (MyClassWithDictionary).GetProperty("Dictionary"), cp => collectionPropsCalled = true,
			        km => keyRelationCalled = true, er => elementRelationCalled = true);

			collectionPropsCalled.Should().Be.True();
			keyRelationCalled.Should().Be.True();
			elementRelationCalled.Should().Be.True();
		}

		[Test]
		public void AddDynamicComponentProperty()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<MyClassWithDynamic>(properties);
			map.Component(For<MyClassWithDynamic>.Property(x => x.DynCompo), (IDynamicComponentMapper cp) => { });
			properties.Single().Should().Be.OfType<HbmDynamicComponent>().And.ValueOf.Name.Should().Be.EqualTo("DynCompo");
		}

		[Test]
		public void CallDynamicComponentMapper()
		{
			var properties = new List<object>();
			var map = new StubPropertyContainerMapper<MyClassWithDynamic>(properties);
			var called = false;
			map.Component(For<MyClassWithDynamic>.Property(x=> x.DynCompo), (IDynamicComponentMapper cp) => called = true);
			called.Should().Be.True();
		}

		private class HackPropertyContainerMapper : AbstractPropertyContainerMapper
		{
			public HackPropertyContainerMapper(System.Type container, HbmMapping mapDoc) : base(container, mapDoc) {}

			#region Overrides of AbstractPropertyContainerMapper

			protected override void AddProperty(object property)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		private class StubPropertyContainerMapper<T> : AbstractPropertyContainerMapper
		{
			private readonly ICollection<object> elements;

			public StubPropertyContainerMapper(ICollection<object> elements)
				: base(typeof(T), new HbmMapping())
			{
				this.elements = elements;
			}

			#region Overrides of AbstractClassMapping<StateProvince>

			protected override void AddProperty(object property)
			{
				elements.Add(property);
			}

			#endregion
		}
	}
}