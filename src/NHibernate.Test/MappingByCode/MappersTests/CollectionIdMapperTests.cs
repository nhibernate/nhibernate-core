using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class CollectionIdMapperTests
	{
		[Test]
		public void WhenCreateThenHasDefaultTypeAndGenerator()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId);
			hbmId.generator.@class.Should().Not.Be.NullOrEmpty();
			hbmId.type.Should().Not.Be.NullOrEmpty();
		}

		[Test]
		public void WhenSetGeneratorThenChangeType()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow);
			
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
			hbmId.type.ToLowerInvariant().Should().Contain("int");
		}

		[Test]
		public void WhenForceTypeThenNotChangeType()
		{
			var hbmId = new HbmCollectionId();
			var collectionIdMapper = new CollectionIdMapper(hbmId);
			collectionIdMapper.Type((IIdentifierType) NHibernateUtil.Int64);
			collectionIdMapper.Generator(Generators.HighLow);

			hbmId.generator.@class.Should().Be.EqualTo("hilo");
			hbmId.type.Should().Be("Int64");
		}

		[Test]
		public void CanSetGenerator()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow);
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
		}

		[Test]
		public void CanSetGeneratorWithParameters()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow, p => p.Params(new { max_low = 99, where = "TableName" }));
			hbmId.generator.@class.Should().Be.EqualTo("hilo");
			hbmId.generator.param.Should().Have.Count.EqualTo(2);
			hbmId.generator.param.Select(p => p.name).Should().Have.SameValuesAs("max_low", "where");
			hbmId.generator.param.Select(p => p.GetText()).Should().Have.SameValuesAs("99", "TableName");
		}

		[Test]
		public void CanSetGeneratorGuid()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Guid);
			hbmId.generator.@class.Should().Be.EqualTo("guid");
		}

		[Test]
		public void CanSetGeneratorGuidComb()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.GuidComb);
			hbmId.generator.@class.Should().Be.EqualTo("guid.comb");
		}

		[Test]
		public void CanSetGeneratorSequence()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Sequence);
			hbmId.generator.@class.Should().Be.EqualTo("sequence");
		}

		[Test]
		public void CanSetGeneratorIdentity()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Identity);
			hbmId.generator.@class.Should().Be.EqualTo("identity");
		}

		[Test]
		public void CantSetGeneratorAssigned()
		{
			var hbmId = new HbmCollectionId();
			var collectionIdMapper = new CollectionIdMapper(hbmId);
			collectionIdMapper.Executing(x=> x.Generator(Generators.Assigned)).Throws<NotSupportedException>();
		}

		[Test]
		public void CanSetColumnName()
		{
			var hbmId = new HbmCollectionId();
			var mapper = new CollectionIdMapper(hbmId);
			mapper.Column("MyName");
			hbmId.Columns.Single().name.Should().Be("MyName");
		}

		[Test]
		public void CanSetLength()
		{
			var hbmId = new HbmCollectionId();
			var mapper = new CollectionIdMapper(hbmId);
			mapper.Length(10);
			hbmId.length.Should().Be("10");
		}
	}
}