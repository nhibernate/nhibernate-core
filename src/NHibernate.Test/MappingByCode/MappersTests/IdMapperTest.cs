using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
		}

		[Test]
		public void CanSetGeneratorWithParameters()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.HighLow, p => p.Params(new { max_low = 99, where = "TableName" }));
			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmId.generator.param, Has.Length.EqualTo(2));
			Assert.That(hbmId.generator.param.Select(p => p.name), Is.EquivalentTo(new [] {"max_low", "where"}));
			Assert.That(hbmId.generator.param.Select(p => p.GetText()), Is.EquivalentTo(new [] {"99", "TableName"}));
		}

		[Test]
		public void CanSetGeneratorGuid()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Guid);
			Assert.That(hbmId.generator.@class, Is.EqualTo("guid"));
		}

		[Test]
		public void CanSetGeneratorGuidComb()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.GuidComb);
			Assert.That(hbmId.generator.@class, Is.EqualTo("guid.comb"));
		}

		[Test]
		public void CanSetGeneratorSequence()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Sequence);
			Assert.That(hbmId.generator.@class, Is.EqualTo("sequence"));
		}

		[Test]
		public void CanSetGeneratorIdentity()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Identity);
			Assert.That(hbmId.generator.@class, Is.EqualTo("identity"));
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
			Assert.That(hbmId.generator.@class, Is.EqualTo("foreign"));
			Assert.That(hbmId.generator.param, Is.Not.Null.And.Length.EqualTo(1));
			var p = hbmId.generator.param.Single();
			Assert.That(p.GetText(), Is.EqualTo("OneToOne"));
			Assert.That(p.name, Is.EqualTo("property"));
		}

		[Test]
		public void CanSetGeneratorAssigned()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.Assigned);
			Assert.That(hbmId.generator.@class, Is.EqualTo("assigned"));
		}

		[Test]
		public void CanSetGeneratorEnhancedSequence()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.EnhancedSequence);
			Assert.That(hbmId.generator.@class, Is.EqualTo("enhanced-sequence"));
		}

		[Test]
		public void CanSetGeneratorEnhancedTable()
		{
			var hbmId = new HbmId();
			new IdMapper(hbmId).Generator(Generators.EnhancedTable);
			Assert.That(hbmId.generator.@class, Is.EqualTo("enhanced-table"));
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
			Assert.That(hbmId.access, Is.EqualTo("nosetter.camelcase"));
		}

		[Test]
		public void CanSetColumnName()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Column("MyName");
			Assert.That(hbmId.Columns.Single().name, Is.EqualTo("MyName"));
		}

		[TestCase(-1, "-1")]
		[TestCase(null, "null", Description = "CanSetExplicitNull")]
		public void CanSetUnsavedValue(object unsavedValue, string expectedUnsavedValue)
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.UnsavedValue(unsavedValue);
			Assert.That(hbmId.unsavedvalue, Is.EqualTo(expectedUnsavedValue));
		}

		[Test]
		public void UnsavedValueUnsetWhenNotSet()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			Assert.That(hbmId.unsavedvalue, Is.EqualTo(null));
		}

		[Test]
		public void CanSetLength()
		{
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Length(10);
			Assert.That(hbmId.length, Is.EqualTo("10"));
		}

		[Test]
		public void CanSetPrecision()
		{
			//NH-2824
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Column(x => x.Precision(10));
			Assert.That(hbmId.column[0].precision, Is.EqualTo("10"));
		}

		[Test]
		public void CanSetScale()
		{
			//NH-2824
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Column(x => x.Scale(10));
			Assert.That(hbmId.column[0].scale, Is.EqualTo("10"));
		}

		[Test]
		public void CanSqlType()
		{
			//NH-3452
			var hbmId = new HbmId();
			var mapper = new IdMapper(null, hbmId);
			mapper.Column(x => x.SqlType("CHAR(10)"));
			Assert.That(hbmId.column[0].sqltype, Is.EqualTo("CHAR(10)"));
		}
	}
}