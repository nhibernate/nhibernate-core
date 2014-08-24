using System;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
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

		[Test]
		public void CanSetCatalog()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Catalog("pizza");
			hbmJoin.catalog.Should().Be("pizza");
		}

		[Test]
		public void CanSetSchema()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Schema("pizza");
			hbmJoin.schema.Should().Be("pizza");
		}

		[Test]
		public void CanSetSqlInsert()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlInsert("blah");

			hbmJoin.SqlInsert.Should().Not.Be.Null();
			hbmJoin.SqlInsert.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlUpdate()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlUpdate("blah");

			hbmJoin.SqlUpdate.Should().Not.Be.Null();
			hbmJoin.SqlUpdate.Text[0].Should().Be("blah");
		}

		[Test]
		public void SetSqlDelete()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlDelete("blah");

			hbmJoin.SqlDelete.Should().Not.Be.Null();
			hbmJoin.SqlDelete.Text[0].Should().Be("blah");
		}

		[Test]
		public void CanSetSqlSubselect()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Subselect("blah");

			hbmJoin.Subselect.Should().Not.Be.Null();
			hbmJoin.subselect.Text[0].Should().Be("blah");
		}

		[Test]
		public void CanSetInverse()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Inverse(true);
			hbmJoin.inverse.Should().Be.True();
		}

		[Test]
		public void CanSetOptional()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Optional(true);
			hbmJoin.optional.Should().Be.True();
		}

		[Test]
		public void CanSetFetch()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Fetch(FetchKind.Select);
			hbmJoin.fetch.Should().Be(HbmJoinFetch.Select);
		}

		[Test]
		public void CallKeyMapper()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			var keyMapperCalled = false;

			mapper.Key(km => keyMapperCalled = true);

			keyMapperCalled.Should().Be.True();
		}

		[Test]
		public void WhenCallKeyMapperThenKeyMapperIsNotNull()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);

			mapper.Key(km => km.Should().Not.Be.Null());
		}

		[Test]
		public void WhenCallKeyMapperMoreThanOnceThenKeyMapperIsTheSame()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			IKeyMapper firstCallInstance = null;
			IKeyMapper secondCallInstance= null;

			mapper.Key(km => firstCallInstance = km);
			mapper.Key(km => secondCallInstance = km);

			firstCallInstance.Should().Be.SameInstanceAs(secondCallInstance);
		}

		[Test]
		public void WhenAddPropertyThenAddItem()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);

			mapper.Property(For<MyClass>.Property(mc => mc.Something), x => { });

			hbmJoin.Properties.Should().Have.Count.EqualTo(1);
		}
	}
}