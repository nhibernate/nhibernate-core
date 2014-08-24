using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

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
			hbm.cascade.Split(',').Select(w => w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Cascade(Mapping.ByCode.Cascade.Persist | Mapping.ByCode.Cascade.DeleteOrphans | Mapping.ByCode.Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w => w.Satisfy(cascade => !cascade.Contains("orphan")));
		}

		[Test]
		public void CanSetAccessor()
		{
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, hbm);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Lazy(LazyRelation.NoProxy);
			hbm.Lazy.Should().Have.Value();
			hbm.Lazy.Should().Be(HbmLaziness.NoProxy);
		}

		[Test]
		public void CanSetConstrained()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.Constrained(true);
			hbm.constrained.Should().Be.True();
		}

		[Test]
		public void CanSetForeignKeyName()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.ForeignKey("Id");

			hbm.foreignkey.Should().Be("Id");
		}

		[Test]
		public void WhenForeignKeyIsNullForeignKeyMappingIsNull()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.ForeignKey(null);

			hbm.foreignkey.Should().Be.Null();
		}

		[Test]
		public void WhenNoMemberPropertyRefAcceptAnything()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.PropertyReference(typeof(Array).GetProperty("Length"));

			hbm.propertyref.Should().Be("Length");
		}

		[Test]
		public void WhenNullMemberPropertyRefNull()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(null, hbm);
			mapper.PropertyReference(null);

			hbm.propertyref.Should().Be.Null();
		}

		[Test]
		public void WhenMemberPropertyRefAcceptOnlyMemberOfExpectedType()
		{
			var hbm = new HbmOneToOne();
			var mapper = new OneToOneMapper(typeof(MyClass).GetProperty("Relation"), hbm);
			mapper.PropertyReference(typeof(Relation).GetProperty("Whatever"));

			hbm.propertyref.Should().Be("Whatever");

			Executing.This(() => mapper.PropertyReference(typeof(Array).GetProperty("Length"))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanSetFormula()
		{
			var member = For<MyClass>.Property(c => c.Relation);
			var mapping = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, mapping);

			mapper.Formula("SomeFormula");
			mapping.formula1.Should().Be("SomeFormula");
		}

		[Test]
		public void WhenSetFormulaWithNullThenSetFormulaWithNull()
		{
			var member = For<MyClass>.Property(c => c.Relation);
			var mapping = new HbmOneToOne();
			var mapper = new OneToOneMapper(member, mapping);
			mapper.Formula(null);
			mapping.formula.Should().Be.Null();
			mapping.formula1.Should().Be.Null();
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
			mapping.formula1.Should().Be.Null();
			var hbmFormula = mapping.formula.First();
			hbmFormula.Text.Length.Should().Be(2);
			hbmFormula.Text[0].Should().Be("Line1");
			hbmFormula.Text[1].Should().Be("Line2");
			mapping.formula1.Should().Be.Null();
		}
	}
}