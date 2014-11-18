using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbmAny.idtype, Is.EqualTo("Int32"));
		}

		[Test]
		public void AtCreationSetTheTwoRequiredColumnsNodes()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			Assert.That(hbmAny.Columns.Count(), Is.EqualTo(2));
			Assert.That(hbmAny.Columns.All(c => !string.IsNullOrEmpty(c.name)), Is.True);
		}

		[Test]
		public void CanSetIdTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(NHibernateUtil.Int64);
			Assert.That(hbmAny.idtype, Is.EqualTo("Int64"));
		}

		[Test]
		public void CanSetIdTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType<long>();
			Assert.That(hbmAny.idtype, Is.EqualTo("Int64"));
		}

		[Test]
		public void CanSetIdTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.IdType(typeof(long));
			Assert.That(hbmAny.idtype, Is.EqualTo("Int64"));
		}

		[Test]
		public void CanSetMetaTypeThroughIType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(NHibernateUtil.Character);
			Assert.That(hbmAny.MetaType, Is.EqualTo("Char"));
		}

		[Test]
		public void CanSetMetaTypeThroughGenericMethod()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType<char>();
			Assert.That(hbmAny.MetaType, Is.EqualTo("Char"));
		}

		[Test]
		public void CanSetMetaTypeThroughType()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaType(typeof(char));
			Assert.That(hbmAny.MetaType, Is.EqualTo("Char"));
		}

		[Test]
		public void CanSetCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.All);
			Assert.That(hbmAny.cascade, Is.EqualTo("all"));
		}

		[Test]
		public void AutoCleanInvalidCascade()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.All | Mapping.ByCode.Cascade.DeleteOrphans);
			Assert.That(hbmAny.cascade, Is.EqualTo("all"));
		}

		[Test]
		public void CanSetIndex()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Index("pizza");
			Assert.That(hbmAny.index, Is.EqualTo("pizza"));
		}

		[Test]
		public void CanSetLazy()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.Lazy(true);
			Assert.That(hbmAny.lazy, Is.EqualTo(true));
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
			Assert.That(columnsBefore[idColumnIndex], Is.SameAs(columnsAfter[idColumnIndex]));
			Assert.That(columnsBefore[idColumnIndex].length, Is.EqualTo("10"));
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
			Assert.That(columnsBefore[metaValueColumnIndex], Is.SameAs(columnsAfter[metaValueColumnIndex]));
			Assert.That(columnsBefore[metaValueColumnIndex].length, Is.EqualTo("500"));
		}

		[Test]
		public void MetaTypeShouldBeImmutable()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(() => mapper.MetaType(NHibernateUtil.Int32), Throws.TypeOf<ArgumentException>());
			Assert.That(() => mapper.MetaType<int>(), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void WhenNullParameterThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			Assert.That(() => mapper.MetaValue(null, typeof(MyReferenceClass)), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => mapper.MetaValue('A', null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenSetFirstMetaValueThenSetMetaTypeIfNotClass()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(hbmAny.MetaType, Is.EqualTo("Char"));
		}

		[Test]
		public void WhenSetMetaValueWithClassThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			Assert.That(() => mapper.MetaValue(typeof(MyReferenceClass), typeof(MyReferenceClass)), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void WhenSetSecondMetaValueThenCheckCompatibility()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(() => mapper.MetaValue(5, typeof(MyClass)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void WhenDuplicatedMetaValueThenRegisterOne()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(hbmAny.metavalue, Has.Length.EqualTo(1));
		}

		[Test]
		public void WhenDuplicatedMetaValueWithDifferentTypeThenThrow()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(() => mapper.MetaValue('A', typeof(MyClass)), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void WhenSetTwoMetaValueThenHasTwoMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			mapper.MetaValue('B', typeof(MyClass));
			Assert.That(hbmAny.metavalue, Has.Length.EqualTo(2));
			Assert.That(hbmAny.metavalue.Select(mv => mv.value), Is.EquivalentTo(new [] {"A", "B"}));
			Assert.That(hbmAny.metavalue.Any(mv => mv.@class.Contains("MyReferenceClass")), Is.True);
			Assert.That(hbmAny.metavalue.Any(mv => mv.@class.Contains("MyClass")), Is.True);
		}

		[Test]
		public void AtCreationSetColumnsUsingMemberName()
		{
			var member = typeof(MyClass).GetProperty("MyReferenceClass");
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			new AnyMapper(member, typeof(int), hbmAny, hbmMapping);
			Assert.That(hbmAny.Columns.ElementAt(0).name, Is.StringContaining("MyReferenceClass"));
			Assert.That(hbmAny.Columns.ElementAt(1).name, Is.StringContaining("MyReferenceClass"));
		}

		[Test]
		public void IdMetaTypeShouldBeImmutableAfterAddMetaValues()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);
			mapper.MetaValue('A', typeof(MyReferenceClass));
			Assert.That(() => mapper.IdType(NHibernateUtil.Int32), Throws.Nothing);
			Assert.That(() => mapper.IdType<int>(), Throws.Nothing);
			Assert.That(() => mapper.IdType<string>(), Throws.TypeOf<ArgumentException>());
			Assert.That(() => mapper.IdType(NHibernateUtil.String), Throws.TypeOf<ArgumentException>());
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Update(false);
			Assert.That(hbmAny.update, Is.False);
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var hbmAny = new HbmAny();
			var mapper = new AnyMapper(null, typeof(int), hbmAny, hbmMapping);

			mapper.Insert(false);
			Assert.That(hbmAny.insert, Is.False);
		}
	}
}