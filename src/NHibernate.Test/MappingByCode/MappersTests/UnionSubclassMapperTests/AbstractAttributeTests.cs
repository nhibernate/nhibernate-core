using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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

			Assert.That(mapping.UnionSubclasses[0].abstractSpecified, Is.True);
			Assert.That(mapping.UnionSubclasses[0].@abstract, Is.True);
		}

		[Test]
		public void CanSetAbstractAttributeOnConcreteClass()
		{
			var mapping = new HbmMapping();
			var mapper = new UnionSubclassMapper(typeof(InventoryItem), mapping);

			Assert.That(mapping.UnionSubclasses[0].abstractSpecified, Is.False);
			Assert.That(mapping.UnionSubclasses[0].@abstract, Is.False);
		}
	}
}
