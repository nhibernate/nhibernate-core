using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class AnyMapperTest
	{
		private class MyClass
		{
			public int Id { get; set; }
			public MyReferenceClass MyReferenceClass { get; set; }
		}

		private class MyReferenceClass
		{
			public int Id { get; set; }
		}

		[Test]
		public void AtCreationSetIdType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			hbmAny.idtype.Should().Be("Int32");
		}

		[Test]
		public void AtCreationSetTheTwoRequiredColumnsNodes()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			hbmAny.Columns.Should().Have.Count.EqualTo(2);
			hbmAny.Columns.Select(c => c.name).All(n => n.Satisfy(name => !string.IsNullOrEmpty(name)));
		}

		[Test]
		public void CanSetIdTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(NHibernateUtil.Int64);
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType<long>();
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetIdTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(typeof(long));
			hbmAny.idtype.Should().Be("Int64");
		}

		[Test]
		public void CanSetMetaTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(NHibernateUtil.Character);
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType<char>();
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetMetaTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(typeof(char));
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void CanSetCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.All);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void AutoCleanInvalidCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
			hbmAny.cascade.Should().Be("all");
		}

		[Test]
		public void CanSetIndex()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Index("pizza");
			hbmAny.index.Should().Be("pizza");
		}

		[Test]
		public void CanSetLazy()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Lazy(true);
			hbmAny.lazy.Should().Be(true);
		}

		[Test]
		public void WhenSetIdColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			const int idColumnIndex = 1;
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => idcm.Length(10), metacm => { });
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[idColumnIndex].Should().Be.SameInstanceAs(columnsAfter[idColumnIndex]);
			columnsBefore[idColumnIndex].length.Should().Be("10");
		}

		[Test]
		public void WhenSetMetaColumnPropertiesThenWorkOnSameHbmColumnCreatedAtCtor()
		{
			// The first column in the mapping is the MetaValue
			const int metaValueColumnIndex = 0;
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			var columnsBefore = hbmAny.Columns.ToArray();
			mapper.Columns(idcm => { }, metacm => metacm.Length(500));
			var columnsAfter = hbmAny.Columns.ToArray();
			columnsBefore[metaValueColumnIndex].Should().Be.SameInstanceAs(columnsAfter[metaValueColumnIndex]);
			columnsBefore[metaValueColumnIndex].length.Should().Be("500");
		}

		[Test]
		public void MetaTypeShouldBeImmutable()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Executing.This(() => mapper.MetaType(NHibernateUtil.Int32)).Should().Throw<ArgumentException>();
			Executing.This(mapper.MetaType<int>).Should().Throw<ArgumentException>();
		}

		[Test]
		public void WhenNullParameterThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			Executing.This(() => mapper.MetaValue(null, typeof(MyReferenceClass))).Should().Throw<ArgumentNullException>();
			Executing.This(() => mapper.MetaValue('A', null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenSetFirstMetaValueThenSetMetaTypeIfNotClass()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			hbmAny.MetaType.Should().Be("Char");
		}

		[Test]
		public void WhenSetMetaValueWithClassThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			Executing.This(() => mapper.MetaValue(typeof(MyReferenceClass), typeof(MyReferenceClass))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void WhenSetSecondMetaValueThenCheckCompatibility()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Executing.This(() => mapper.MetaValue(5, typeof(MyClass))).Should().Throw<ArgumentException>();
		}

		[Test]
		public void WhenDuplicatedMetaValueThenRegisterOne()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('A', typeof(MyReferenceClass));
			hbmAny.metavalue.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenDuplicatedMetaValueWithDifferentTypeThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Executing.This(() => mapper.MetaValue('A', typeof(MyClass))).Should().Throw<ArgumentException>();
		}

		[Test]
		public void WhenSetTwoMetaValueThenHasTwoMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('B', typeof(MyClass));
			hbmAny.metavalue.Should().Have.Count.EqualTo(2);
			hbmAny.metavalue.Select(mv => mv.value).Should().Have.SameValuesAs("A", "B");
			hbmAny.metavalue.Select(mv => mv.@class).Satisfy(c => c.Any(clazz => clazz.Contains("MyReferenceClass")));
			hbmAny.metavalue.Select(mv => mv.@class).Satisfy(c => c.Any(clazz => clazz.Contains("MyClass")));
		}

		[Test]
		public void AtCreationSetColumnsUsingMemberName()
		{
			var member = typeof(MyClass).GetProperty("MyReferenceClass");
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(member, typeof(int), hbmAny, hbmMapping);
			hbmAny.Columns.ElementAt(0).name.Should().Contain("MyReferenceClass");
			hbmAny.Columns.ElementAt(1).name.Should().Contain("MyReferenceClass");
		}

		[Test]
		public void IdMetaTypeShouldBeImmutableAfterAddMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Executing.This(() => mapper.IdType(NHibernateUtil.Int32)).Should().NotThrow();
			Executing.This(mapper.IdType<int>).Should().NotThrow();
			Executing.This(mapper.IdType<string>).Should().Throw<ArgumentException>();
			Executing.This(() => mapper.IdType(NHibernateUtil.String)).Should().Throw<ArgumentException>();
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Update(false);
			hbmAny.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Insert(false);
			hbmAny.insert.Should().Be.False();
		}
	}
}