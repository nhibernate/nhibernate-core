using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(() => new JoinMapper(typeof(MyClass), null, hbmJoin, mapdoc), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => new JoinMapper(typeof(MyClass), "", hbmJoin, mapdoc), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => new JoinMapper(typeof(MyClass), "     ", hbmJoin, mapdoc), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void WhenCreateWithNullHbmJoinThenThrows()
		{
			var mapdoc = new HbmMapping();
			Assert.That(() => new JoinMapper(typeof(MyClass), "AA", null, mapdoc), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenCreateThenSetDefaultTableName()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			new JoinMapper(typeof(MyClass), "   AA   ", hbmJoin, mapdoc);
			Assert.That(hbmJoin.table, Is.EqualTo("AA"));
		}

		[Test]
		public void CanSetTable()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Table("   Pizza   ");
			Assert.That(hbmJoin.table, Is.EqualTo("Pizza"));
		}

		[Test]
		public void WhenSetTableNameEmptyThenThrows()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			Assert.That(() => mapper.Table(null), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => mapper.Table(""), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => mapper.Table("    "), Throws.TypeOf<ArgumentOutOfRangeException>());
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
																	 Assert.That(m, Is.SameAs(mapper));
																	 Assert.That(x.OldName, Is.EqualTo("AA"));
																	 Assert.That(x.NewName, Is.EqualTo("Pizza"));
																	 eventCalled = true;
																 };
			mapper.Table("   Pizza   ");
			Assert.That(eventCalled, Is.True);
		}

		[Test]
		public void CanSetCatalog()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Catalog("pizza");
			Assert.That(hbmJoin.catalog, Is.EqualTo("pizza"));
		}

		[Test]
		public void CanSetSchema()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Schema("pizza");
			Assert.That(hbmJoin.schema, Is.EqualTo("pizza"));
		}

		[Test]
		public void CanSetSqlInsert()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlInsert("blah");

			Assert.That(hbmJoin.SqlInsert, Is.Not.Null);
			Assert.That(hbmJoin.SqlInsert.Text[0], Is.EqualTo("blah"));
		}

		[Test]
		public void SetSqlUpdate()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlUpdate("blah");

			Assert.That(hbmJoin.SqlUpdate, Is.Not.Null);
			Assert.That(hbmJoin.SqlUpdate.Text[0], Is.EqualTo("blah"));
		}

		[Test]
		public void SetSqlDelete()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.SqlDelete("blah");

			Assert.That(hbmJoin.SqlDelete, Is.Not.Null);
			Assert.That(hbmJoin.SqlDelete.Text[0], Is.EqualTo("blah"));
		}

		[Test]
		public void CanSetSqlSubselect()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Subselect("blah");

			Assert.That(hbmJoin.Subselect, Is.Not.Null);
			Assert.That(hbmJoin.subselect.Text[0], Is.EqualTo("blah"));
		}

		[Test]
		public void CanSetInverse()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Inverse(true);
			Assert.That(hbmJoin.inverse, Is.True);
		}

		[Test]
		public void CanSetOptional()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Optional(true);
			Assert.That(hbmJoin.optional, Is.True);
		}

		[Test]
		public void CanSetFetch()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			mapper.Fetch(FetchKind.Select);
			Assert.That(hbmJoin.fetch, Is.EqualTo(HbmJoinFetch.Select));
		}

		[Test]
		public void CallKeyMapper()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);
			var keyMapperCalled = false;

			mapper.Key(km => keyMapperCalled = true);

			Assert.That(keyMapperCalled, Is.True);
		}

		[Test]
		public void WhenCallKeyMapperThenKeyMapperIsNotNull()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);

			mapper.Key(km => Assert.That(km, Is.Not.Null));
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

			Assert.That(firstCallInstance, Is.SameAs(secondCallInstance));
		}

		[Test]
		public void WhenAddPropertyThenAddItem()
		{
			var mapdoc = new HbmMapping();
			var hbmJoin = new HbmJoin();
			var mapper = new JoinMapper(typeof(MyClass), "AA", hbmJoin, mapdoc);

			mapper.Property(For<MyClass>.Property(mc => mc.Something), x => { });

			Assert.That(hbmJoin.Properties.Count(), Is.EqualTo(1));
		}
	}
}