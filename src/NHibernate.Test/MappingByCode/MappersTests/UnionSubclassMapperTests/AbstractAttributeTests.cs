using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.UnionSubclassMapperTests
{
	[TestFixture]
	public class AbstractAttributeTests
	{
		private abstract class EntityBase
		{
		}

		private abstract class Item : EntityBase
		{
		}

		private class InventoryItem : Item
		{
		}

		[Test]
		public void CanSetAbstractAttributeOnAbstractClass()
		{
			var mapping = new HbmMapping();
			var mapper = new UnionSubclassMapper(typeof(Item), mapping);

			mapping.UnionSubclasses[0].abstractSpecified.Should().Be.True();
			mapping.UnionSubclasses[0].@abstract.Should().Be.True();
		}

		[Test]
		public void CanSetAbstractAttributeOnConcreteClass()
		{
			var mapping = new HbmMapping();
			var mapper = new UnionSubclassMapper(typeof(InventoryItem), mapping);

			mapping.UnionSubclasses[0].abstractSpecified.Should().Be.False();
			mapping.UnionSubclasses[0].@abstract.Should().Be.False();
		}
	}
}
