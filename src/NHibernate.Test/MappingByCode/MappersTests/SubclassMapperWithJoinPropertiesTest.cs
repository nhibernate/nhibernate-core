using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class SubclassMapperWithJoinPropertiesTest
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		private class Inherited : MyClass
		{
		}

		[Test]
		public void WhenDefineJoinThenAddJoinWithTableNameAndKey()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);

			mapper.Join("MyTable", x => { });

			var hbmClass = mapdoc.SubClasses[0];
			var hbmJoin = hbmClass.Joins.Single();
			hbmJoin.table.Should().Be("MyTable");
			hbmJoin.key.Should().Not.Be.Null();
			hbmJoin.key.column1.Should().Not.Be.Null();
		}

		[Test]
		public void WhenDefineJoinThenCallJoinMapper()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);
			var called = false;
			mapper.Join("MyTable", x =>
			{
				x.Should().Not.Be.Null();
				called = true;
			});

			called.Should().Be.True();
		}

		[Test]
		public void WhenDefineMoreJoinsThenTableNameShouldBeUnique()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);

			mapper.Join("T1", x => { });
			mapper.Join("T2", x => { });

			var hbmClass = mapdoc.SubClasses[0];
			hbmClass.Joins.Should().Have.Count.EqualTo(2);
			hbmClass.Joins.Select(x => x.table).Should().Have.UniqueValues();
		}

		[Test]
		public void WhenDefineMoreJoinsWithSameIdThenUseSameJoinMapperInstance()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);
			IJoinMapper firstCallInstance = null;
			IJoinMapper secondCallInstance = null;

			mapper.Join("T1", x => firstCallInstance = x);
			mapper.Join("T1", x => secondCallInstance = x);

			firstCallInstance.Should().Be.SameInstanceAs(secondCallInstance);
			var hbmClass = mapdoc.SubClasses[0];
			hbmClass.Joins.Should().Have.Count.EqualTo(1);
		}
	}
}