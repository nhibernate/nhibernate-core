using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class TablesSincronizationTests
	{
		private class EntitySimple
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenSetSyncWithNullThenDoesNotThrows()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			Assert.That(() => rc.Synchronize(null), Throws.Nothing);
		}

		[Test]
		public void WhenSetSyncMixedWithNullAndEmptyThenAddOnlyValid()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("", "  ATable   ", "     ", null);
			Assert.That(mapdoc.RootClasses[0].Synchronize.Single().table, Is.EqualTo("ATable"));
		}

		[Test]
		public void WhenSetMoreSyncThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2", "T3", null);
			Assert.That(mapdoc.RootClasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}

		[Test]
		public void WhenSetMoreThenOnceThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3");
			Assert.That(mapdoc.RootClasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}

		[Test]
		public void WhenSetMoreThenOnceThenDoesNotDuplicate()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3", "T2");
			Assert.That(mapdoc.RootClasses[0].Synchronize.Select(x => x.table), Is.EquivalentTo(new [] {"T1", "T2", "T3"}));
		}
	}
}