using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class IdBagMapperTest
	{
		private class Animal
		{
			public int Id { get; set; }
			private ICollection<Animal> children = null;
			public ICollection<Animal> Children
			{
				get { return children; }
			}
		}

		[Test]
		public void WhenCreatedHasId()
		{
			var hbm = new HbmIdbag();
			new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			Assert.That(hbm.collectionid, Is.Not.Null);
		}

		[Test]
		public void WhenConfigureIdMoreThanOnceThenUseSameMapper()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			ICollectionIdMapper firstInstance = null;
			ICollectionIdMapper secondInstance = null;
			mapper.Id(x => firstInstance = x);
			mapper.Id(x => secondInstance = x);

			Assert.That(firstInstance, Is.SameAs(secondInstance));
		}

		[Test]
		public void WhenConfigureIdThenCallMapper()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Id(x => x.Column("catchMe"));

			Assert.That(hbm.collectionid.Columns.Single().name, Is.EqualTo("catchMe"));
		}

		[Test]
		public void SetInverse()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Inverse(true);
			Assert.That(hbm.Inverse, Is.True);
			mapper.Inverse(false);
			Assert.That(hbm.Inverse, Is.False);
		}

		[Test]
		public void SetMutable()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Mutable(true);
			Assert.That(hbm.Mutable, Is.True);
			mapper.Mutable(false);
			Assert.That(hbm.Mutable, Is.False);
		}

		[Test]
		public void SetWhere()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Where("c > 10");
			Assert.That(hbm.Where, Is.EqualTo("c > 10"));
		}

		[Test]
		public void SetBatchSize()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.BatchSize(10);
			Assert.That(hbm.BatchSize, Is.EqualTo(10));
		}

		[Test]
		public void SetLazy()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Lazy(CollectionLazy.Extra);
			Assert.That(hbm.Lazy, Is.EqualTo(HbmCollectionLazy.Extra));
			mapper.Lazy(CollectionLazy.NoLazy);
			Assert.That(hbm.Lazy, Is.EqualTo(HbmCollectionLazy.False));
			mapper.Lazy(CollectionLazy.Lazy);
			Assert.That(hbm.Lazy, Is.EqualTo(HbmCollectionLazy.True));
		}

		[Test]
		public void CallKeyMapper()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			bool kmCalled = false;
			mapper.Key(km => kmCalled = true);
			Assert.That(hbm.Key, Is.Not.Null);
			Assert.That(kmCalled, Is.True);
		}

		[Test]
		public void SetCollectionTypeByWrongTypeShouldThrow()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			Assert.That(() => mapper.Type(null), Throws.TypeOf<ArgumentNullException>());
			Assert.That(() => mapper.Type(typeof(object)), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void SetCollectionTypeByGenericType()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type<FakeUserCollectionType>();
			Assert.That(hbm.CollectionType, Is.StringContaining("FakeUserCollectionType"));
		}

		[Test]
		public void SetCollectionTypeByType()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type(typeof(FakeUserCollectionType));
			Assert.That(hbm.CollectionType, Is.StringContaining("FakeUserCollectionType"));
		}

		[Test]
		public void CanChangeAccessor()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Access(Accessor.Field);

			Assert.That(hbm.Access, Is.Not.Null);
		}

		[Test]
		public void CanSetCache()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(x => x.Region("pizza"));

			Assert.That(hbm.cache, Is.Not.Null);
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(ch => ch.Region("pizza"));
			mapper.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = hbm.cache;
			Assert.That(hbmCache, Is.Not.Null);
			Assert.That(hbmCache.region, Is.EqualTo("pizza"));
			Assert.That(hbmCache.usage, Is.EqualTo(HbmCacheUsage.NonstrictReadWrite));
		}

		[Test]
		public void CanSetAFilterThroughAction()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			Assert.That(hbm.filter.Length, Is.EqualTo(1));
			Assert.That(hbm.filter[0].condition, Is.EqualTo("condition1"));
			Assert.That(hbm.filter[0].name, Is.EqualTo("filter1"));
		}

		[Test]
		public void CanSetMoreFiltersThroughAction()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			Assert.That(hbm.filter.Length, Is.EqualTo(2));
			Assert.True(hbm.filter.Any(f => f.name == "filter1" && f.condition == "condition1"));
			Assert.True(hbm.filter.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenSameNameThenOverrideCondition()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			mapper.Filter("filter1", f => f.Condition("anothercondition1"));
			Assert.That(hbm.filter.Length, Is.EqualTo(2));
			Assert.That(hbm.filter.Any(f => f.name == "filter1" && f.condition == "anothercondition1"), Is.True);
			Assert.That(hbm.filter.Any(f => f.name == "filter2" && f.condition == "condition2"), Is.True);
		}

		[Test]
		public void WhenActionIsNullThenAddFilterName()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", null);
			Assert.That(hbm.filter.Length, Is.EqualTo(1));
			var filter = hbm.filter[0];
			Assert.That(filter.condition, Is.EqualTo(null));
			Assert.That(filter.name, Is.EqualTo("filter1"));
		}

		[Test]
		public void SetFetchMode()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Fetch(CollectionFetchMode.Subselect);
			Assert.That(hbm.fetch, Is.EqualTo(HbmCollectionFetchMode.Subselect));
			Assert.That(hbm.fetchSpecified, Is.True);
			mapper.Fetch(CollectionFetchMode.Join);
			Assert.That(hbm.fetch, Is.EqualTo(HbmCollectionFetchMode.Join));
			Assert.That(hbm.fetchSpecified, Is.True);
			mapper.Fetch(CollectionFetchMode.Select);
			Assert.That(hbm.fetch, Is.EqualTo(HbmCollectionFetchMode.Select));
			Assert.That(hbm.fetchSpecified, Is.False);
		}
	}
}