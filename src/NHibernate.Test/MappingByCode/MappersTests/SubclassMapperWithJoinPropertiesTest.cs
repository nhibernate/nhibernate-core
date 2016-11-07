using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbmJoin.table, Is.EqualTo("MyTable"));
			Assert.That(hbmJoin.key, Is.Not.Null);
			Assert.That(hbmJoin.key.column1, Is.Not.Null);
		}

		[Test]
		public void WhenDefineJoinThenCallJoinMapper()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);
			var called = false;
			mapper.Join("MyTable", x =>
			{
				Assert.That(x, Is.Not.Null);
				called = true;
			});

			Assert.That(called, Is.True);
		}

		[Test]
		public void WhenDefineMoreJoinsThenTableNameShouldBeUnique()
		{
			var mapdoc = new HbmMapping();
			var mapper = new SubclassMapper(typeof(Inherited), mapdoc);

			mapper.Join("T1", x => { });
			mapper.Join("T2", x => { });

			var hbmClass = mapdoc.SubClasses[0];
			Assert.That(hbmClass.Joins.Count(), Is.EqualTo(2));
			Assert.That(hbmClass.Joins.Select(x => x.table), Is.Unique);
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

			Assert.That(firstCallInstance, Is.SameAs(secondCallInstance));
			var hbmClass = mapdoc.SubClasses[0];
			Assert.That(hbmClass.Joins.Count(), Is.EqualTo(1));
		}
	}
}