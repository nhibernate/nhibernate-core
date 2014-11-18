using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.UnionSubclassMapperTests
{
	public class TablesSincronizationTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		private class InheritedSimple : EntitySimple
		{
		}

		[Test]
		public void WhenSetSyncWithNullThenDoesNotThrows()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			Assert.That(() => rc.Synchronize(null), Throws.Nothing);
		}

		[Test]
		public void WhenSetSyncMixedWithNullAndEmptyThenAddOnlyValid()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("", "  ATable   ", "     ", null);
			Assert.That(mapdoc.UnionSubclasses[0].Synchronize.Single().table, Is.EqualTo("ATable"));
		}

		[Test]
		public void WhenSetMoreSyncThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2", "T3", null);
			Assert.That(mapdoc.UnionSubclasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}

		[Test]
		public void WhenSetMoreThenOnceThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3");
			Assert.That(mapdoc.UnionSubclasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}

		[Test]
		public void WhenSetMoreThenOnceThenDoesNotDuplicate()
		{
			var mapdoc = new HbmMapping();
			var rc = new UnionSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3", "T2");
			Assert.That(mapdoc.UnionSubclasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}
	}
}