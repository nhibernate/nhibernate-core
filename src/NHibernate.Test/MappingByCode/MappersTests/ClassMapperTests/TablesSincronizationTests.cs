using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

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
			rc.Executing(x=>x.Synchronize(null)).NotThrows();
		}

		[Test]
		public void WhenSetSyncMixedWithNullAndEmptyThenAddOnlyValid()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("", "  ATable   ", "     ", null);
			mapdoc.RootClasses[0].Synchronize.Single().table.Should().Be("ATable");
		}

		[Test]
		public void WhenSetMoreSyncThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2", "T3", null);
			mapdoc.RootClasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}

		[Test]
		public void WhenSetMoreThenOnceThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3");
			mapdoc.RootClasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}

		[Test]
		public void WhenSetMoreThenOnceThenDoesNotDuplicate()
		{
			var mapdoc = new HbmMapping();
			var rc = new ClassMapper(typeof(EntitySimple), mapdoc, For<EntitySimple>.Property(x => x.Id));
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3", "T2");
			mapdoc.RootClasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}
	}
}