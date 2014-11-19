using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class OneToOneMapperTest
	{
		private class MyClass
		{
			public Relation Relation { get; set; }
		}

		private class Relation
		{
			public string Whatever { get; set; }
		}

		[Test]
		public void AssignCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Cascade(Mapping.ByCode.Cascade.Persist | Mapping.ByCode.Cascade.Remove);
			Assert.That(hbm.cascade.Split(',').Select(w => w.Trim()), Contains.Item("persist").And.Contains("delete"));
		}

		[Test]
		public void CanSetAccessor()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, hbm);

			mapper.Access(Accessor.ReadOnly);
			Assert.That(hbm.Access, Is.EqualTo("readonly"));
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Lazy(LazyRelation.NoProxy);
			Assert.That(hbm.Lazy, Is.Not.Null);
			Assert.That(hbm.Lazy, Is.EqualTo(HbmLaziness.NoProxy));
		}

		[Test]
		public void CanSetConstrained()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Constrained(true);
			Assert.That(hbm.constrained, Is.True);
		}

		[Test]
		public void CanSetForeignKeyName()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.ForeignKey("Id");

			Assert.That(hbm.foreignkey, Is.EqualTo("Id"));
		}

		[Test]
		public void WhenForeignKeyIsNullForeignKeyMappingIsNull()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.ForeignKey(null);

			Assert.That(hbm.foreignkey, Is.Null);
		}

		[Test]
		public void WhenNoMemberPropertyRefAcceptAnything()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.PropertyReference(typeof(Array).GetProperty("Length"));

			Assert.That(hbm.propertyref, Is.EqualTo("Length"));
		}

		[Test]
		public void WhenNullMemberPropertyRefNull()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.PropertyReference(null);

			Assert.That(hbm.propertyref, Is.Null);
		}

		[Test]
		public void WhenMemberPropertyRefAcceptOnlyMemberOfExpectedType()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(typeof(MyClass).GetProperty("Relation"), hbm);
			mapper.PropertyReference(typeof(Relation).GetProperty("Whatever"));

			Assert.That(hbm.propertyref, Is.EqualTo("Whatever"));

			Assert.That(() => mapper.PropertyReference(typeof(Array).GetProperty("Length")), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void CanSetFormula()
		{
			var member = For<MyClass>.Property(c => c.Relation);
			var mapping = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, mapping);

			mapper.Formula("SomeFormula");
			Assert.That(mapping.formula1, Is.EqualTo("SomeFormula"));
		}

		[Test]
		public void WhenSetFormulaWithNullThenSetFormulaWithNull()
		{
			var member = For<MyClass>.Property(c => c.Relation);
			var mapping = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, mapping);
			mapper.Formula(null);
			Assert.That(mapping.formula, Is.Null);
			Assert.That(mapping.formula1, Is.Null);
		}

		[Test]
		public void WhenSetFormulaWithMultipleLinesThenSetFormulaNode()
		{
			var member = For<MyClass>.Property(c => c.Relation);
			var mapping = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, mapping);
			var formula = @"Line1
Line2";
			mapper.Formula(formula);
			Assert.That(mapping.formula1, Is.Null);
			var hbmFormula = mapping.formula.First();
			Assert.That(hbmFormula.Text.Length, Is.EqualTo(2));
			Assert.That(hbmFormula.Text[0], Is.EqualTo("Line1"));
			Assert.That(hbmFormula.Text[1], Is.EqualTo("Line2"));
			Assert.That(mapping.formula1, Is.Null);
		}
	}
}