using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests.ClassMapperTests
{
	public class ClassMapperWithJoinPropertiesTest
	{
		private class MyClass
		{
			public int Id { get; set; }
		}

		[Test]
		public void WhenDefineJoinThenAddJoinWithTableNameAndKey()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(MyClass), mapdoc, For<MyClass>.Property(x=> x.Id));
			mapper.Join("MyTable",x => { });

			var hbmClass = mapdoc.RootClasses[0];
			var hbmJoin = hbmClass.Joins.Single();
			hbmJoin.table.Should().Be("MyTable");
			hbmJoin.key.Should().Not.Be.Null();
			hbmJoin.key.column1.Should().Not.Be.Null();
		}

		[Test]
		public void WhenDefineJoinThenCallJoinMapper()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(MyClass), mapdoc, For<MyClass>.Property(x => x.Id));
			var called = false;
			mapper.Join("MyTable", x =>
			            {
										x.Should().Not.Be.Null();
										called = true;
			            });

			called.Should().Be.True();
		}

		//[Test]
		//public void WhenDefineJoinTableNameAsTableOfRootThenThrows()
		//{
		// We can't give support to this check.
		// The name of the table of the root-class may change during the mapping process where the name of the joined table is immutable (or...perhaps what is really immutable is the Id used in the explicit mapping ;) )
		// We are using the name of the joined table as id for the splitted property group. I can't find another way to be 100% sure to re-use the same
		// instance of JoinMapper when the Join method is used more than once.
		// The case of "inconsistent" name should be checked by binders since the problem is the same using XML mappings
		//
		//  var mapdoc = new HbmMapping();
		//  var mapper = new ClassMapper(typeof(MyClass), mapdoc, For<MyClass>.Property(x => x.Id));
		//  Executing.This(()=> mapper.Join("MyClass", x => { })).Should().Throw<MappingException>();
		//}

		[Test]
		public void WhenDefineMoreJoinsThenTableNameShouldBeUnique()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(MyClass), mapdoc, For<MyClass>.Property(x => x.Id));
			mapper.Join("T1", x => { });
			mapper.Join("T2",x => { });

			var hbmClass = mapdoc.RootClasses[0];
			hbmClass.Joins.Should().Have.Count.EqualTo(2);
			hbmClass.Joins.Select(x=> x.table).Should().Have.UniqueValues();
		}

		[Test]
		public void WhenDefineMoreJoinsWithSameIdThenUseSameJoinMapperInstance()
		{
			var mapdoc = new HbmMapping();
			var mapper = new ClassMapper(typeof(MyClass), mapdoc, For<MyClass>.Property(x => x.Id));
			IJoinMapper firstCallInstance = null;
			IJoinMapper secondCallInstance = null;

			mapper.Join("T1", x => firstCallInstance = x);
			mapper.Join("T1", x => secondCallInstance = x);

			firstCallInstance.Should().Be.SameInstanceAs(secondCallInstance);
			var hbmClass = mapdoc.RootClasses[0];
			hbmClass.Joins.Should().Have.Count.EqualTo(1);
		}
	}
}