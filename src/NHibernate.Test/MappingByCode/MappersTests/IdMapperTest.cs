using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class IdMapperTest
	{
		// The strategy Assigned is the default and does not need the "generator"
		//public void SetGeneratorAtCtor()
		//{
		//  var hbmId = new HbmId();
		//  new IdMapper(hbmId);
		//  hbmId.generator.Should().Not.Be.Null();
		//}

		[Test]
		public void CanSetGenerator()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.HighLow);
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
		}

		[Test]
		public void CanSetGeneratorWithParameters()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.HighLow, p => p.Params(new { max_low = 99, where = "TableName" }));
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
			hbmId.generator.param.Should().Have.Count.EqualTo(2);
			hbmId.generator.param.Select(p => p.name).Should().Have.SameValuesAs("max_low", "where");
			hbmId.generator.param.Select(p => p.GetText()).Should().Have.SameValuesAs("99", "TableName");
		}

		[Test]
		public void CanSetGeneratorGuid()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Guid);
			hbmId.generator.@class.Should().Be.EqualTo("guid");
		}

		[Test]
		public void CanSetGeneratorGuidComb()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.GuidComb);
			hbmId.generator.@class.Should().Be.EqualTo("guid.comb");
		}

		[Test]
		public void CanSetGeneratorSequence()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Sequence);
			hbmId.generator.@class.Should().Be.EqualTo("sequence");
		}

		[Test]
		public void CanSetGeneratorIdentity()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Identity);
			hbmId.generator.@class.Should().Be.EqualTo("identity");
		}

		private class MyClass
		{
			public int Id { get; set; }
			public Related OneToOne { get; set; }
		}

		private class Related
		{
			public int Id { get; set; }
		}

		[Test]
		public void CanSetGeneratorForeign()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Foreign<MyClass>(mc => mc.OneToOne));
			hbmId.generator.@class.Should().Be.EqualTo("foreign");
			hbmId.generator.param.Should().Not.Be.Null().And.Have.Count.EqualTo(1);
			hbmId.generator.param.Single().Satisfy(p => p.name == "property" && p.GetText() == "OneToOne");
		}

		[Test]
		public void CanSetGeneratorAssigned()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Assigned);
			hbmId.generator.@class.Should().Be.EqualTo("assigned");
		}

		[Test]
		public void CanSetGeneratorEnhancedSequence()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.EnhancedSequence);
			hbmId.generator.@class.Should().Be.EqualTo("enhanced-sequence");
		}

		[Test]
		public void CanSetGeneratorEnhancedTable()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.EnhancedTable);
			hbmId.generator.@class.Should().Be.EqualTo("enhanced-table");
		}
		private class BaseEntity
		{
			private int id;

			public int Id
			{
				get { return id; }
			}
		}

		private class Entity : BaseEntity
		{

		}

		[Test]
		public void WhenHasMemberCanSetAccessor()
		{
			var member = typeof(Entity).GetProperty("Id",
																							 BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
																							 | BindingFlags.FlattenHierarchy);
			var hbmId = new HbmId();
			var mapper = new IdMapper(member, hbmId);
			mapper.Access(Accessor.NoSetter);
			hbmId.access.Should().Be("nosetter.camelcase");
		}

		[Test]
		public void CanSetColumnName()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Column("MyName");
			hbmId.Columns.Single().name.Should().Be("MyName");
		}

		[TestCase(-1, "-1")]
		[TestCase(null, "null", Description = "CanSetExplicitNull")]
		public void CanSetUnsavedValue(object unsavedValue, string expectedUnsavedValue)
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.UnsavedValue(unsavedValue);
			hbmId.unsavedvalue.Should().Be(expectedUnsavedValue);
		}

		[Test]
		public void UnsavedValueUnsetWhenNotSet()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			hbmId.unsavedvalue.Should().Be(null);
		}

		[Test]
		public void CanSetLength()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Length(10);
			hbmId.length.Should().Be("10");
		}
	}
}