using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class CollectionIdMapperTests
	{
		[Test]
		public void WhenCreateThenHasDefaultTypeAndGenerator()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId);
			Assert.That(hbmId.generator.@class, Is.Not.Null.And.Not.Empty);
			Assert.That(hbmId.type, Is.Not.Null.And.Not.Empty);
		}

		[Test]
		public void WhenSetGeneratorThenChangeType()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow);
			
			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmId.type.ToLowerInvariant(), Is.StringContaining("int"));
		}

		[Test]
		public void WhenForceTypeThenNotChangeType()
		{
			var hbmId = new HbmCollectionId();
			var collectionIdMapper = new CollectionIdMapper(hbmId);
			collectionIdMapper.Type((IIdentifierType) NHibernateUtil.Int64);
			collectionIdMapper.Generator(Generators.HighLow);

			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmId.type, Is.EqualTo("Int64"));
		}

		[Test]
		public void CanSetGenerator()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow);
			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
		}

		[Test]
		public void CanSetGeneratorWithParameters()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.HighLow, p => p.Params(new { max_low = 99, where = "TableName" }));
			Assert.That(hbmId.generator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmId.generator.param, Has.Length.EqualTo(2));
			Assert.That(hbmId.generator.param.Select(p => p.name), Is.EquivalentTo(new [] {"max_low", "where"}));
			Assert.That(hbmId.generator.param.Select(p => p.GetText()), Is.EquivalentTo(new [] {"99", "TableName"}));
		}

		[Test]
		public void CanSetGeneratorGuid()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Guid);
			Assert.That(hbmId.generator.@class, Is.EqualTo("guid"));
		}

		[Test]
		public void CanSetGeneratorGuidComb()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.GuidComb);
			Assert.That(hbmId.generator.@class, Is.EqualTo("guid.comb"));
		}

		[Test]
		public void CanSetGeneratorSequence()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Sequence);
			Assert.That(hbmId.generator.@class, Is.EqualTo("sequence"));
		}

		[Test]
		public void CanSetGeneratorIdentity()
		{
			var hbmId = new HbmCollectionId();
			new CollectionIdMapper(hbmId).Generator(Generators.Identity);
			Assert.That(hbmId.generator.@class, Is.EqualTo("identity"));
		}

		[Test]
		public void CantSetGeneratorAssigned()
		{
			var hbmId = new HbmCollectionId();
			var collectionIdMapper = new CollectionIdMapper(hbmId);
			Assert.That(() => collectionIdMapper.Generator(Generators.Assigned), Throws.TypeOf<NotSupportedException>());
		}

		[Test]
		public void CanSetColumnName()
		{
			var hbmId = new HbmCollectionId();
			var mapper = new CollectionIdMapper(hbmId);
			mapper.Column("MyName");
			Assert.That(hbmId.Columns.Single().name, Is.EqualTo("MyName"));
		}

		[Test]
		public void CanSetLength()
		{
			var hbmId = new HbmCollectionId();
			var mapper = new CollectionIdMapper(hbmId);
			mapper.Length(10);
			Assert.That(hbmId.length, Is.EqualTo("10"));
		}
	}
}