using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class JoinMapperTests
	{
		private class MyClass
		{
			public int Something { get; set; }
		}

		[Test]
		public void WhenCreateWithEmptySplitGroupThenThrows()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			Executing.This(() => new JoinMapper(typeof(MyClass), null, hbmJoin, mapdoc)).Should().Throw<ArgumentNullException>();
			Executing.This(() => new JoinMapper(typeof(MyClass), "", hbmJoin, mapdoc)).Should().Throw<ArgumentOutOfRangeException>();
			Executing.This(() => new JoinMapper(typeof(MyClass), "     ", hbmJoin, mapdoc)).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenCreateWithNullHbmJoinThenThrows()
		{
			var mapdoc = new HbmMapping();
			Executing.This(() => new JoinMapper(typeof(MyClass), "AA", null, mapdoc)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCreateThenSetDefaultTableName()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			new JoinMapper(typeof(MyClass), "   AA   ", hbmJoin, mapdoc);
			hbmJoin.table.Should().Be("AA");
		}

		[Test]
		public void CanSetTable()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Table("   Pizza   ");
			hbmJoin.table.Should().Be("Pizza");
		}

		[Test]
		public void WhenSetTableNameEmptyThenThrows()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Executing(x => x.Table(null)).Throws<ArgumentNullException>();
			mapper.Executing(x => x.Table("")).Throws<ArgumentOutOfRangeException>();
			mapper.Executing(x => x.Table("    ")).Throws<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenTableNameChangesValueThenNotify()
		{
			bool eventCalled = false;
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.TableNameChanged += (m, x) =>
			                           {
																	 m.Should().Be.SameInstanceAs(mapper);
																	 x.OldName.Should().Be("AA");
																	 x.NewName.Should().Be("Pizza");
																	 eventCalled = true;
																 };
			mapper.Table("   Pizza   ");
			eventCalled.Should().Be.True();
		}
	}
}