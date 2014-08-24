using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Persister.Entity;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.JoinedSubclassMapperTests
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
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Executing(x => x.Synchronize(null)).NotThrows();
		}

		[Test]
		public void WhenSetSyncMixedWithNullAndEmptyThenAddOnlyValid()
		{
			var mapdoc = new HbmMapping();
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("", "  ATable   ", "     ", null);
			mapdoc.JoinedSubclasses[0].Synchronize.Single().table.Should().Be("ATable");
		}

		[Test]
		public void WhenSetMoreSyncThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2", "T3", null);
			mapdoc.JoinedSubclasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}

		[Test]
		public void WhenSetMoreThenOnceThenAddAll()
		{
			var mapdoc = new HbmMapping();
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3");
			mapdoc.JoinedSubclasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}

		[Test]
		public void WhenSetMoreThenOnceThenDoesNotDuplicate()
		{
			var mapdoc = new HbmMapping();
			var rc = new JoinedSubclassMapper(typeof(InheritedSimple), mapdoc);
			rc.Synchronize("T1", "T2");
			rc.Synchronize("T3", "T2");
			mapdoc.JoinedSubclasses[0].Synchronize.Select(x => x.table).Should().Have.SameValuesAs("T1", "T2", "T3");
		}
	}
}