using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

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
			hbm.collectionid.Should().Not.Be.Null();
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

			firstInstance.Should().Be.SameInstanceAs(secondInstance);
		}

		[Test]
		public void WhenConfigureIdThenCallMapper()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Id(x => x.Column("catchMe"));

			hbm.collectionid.Columns.Single().name.Should().Be("catchMe");
		}

		[Test]
		public void SetInverse()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Inverse(true);
			hbm.Inverse.Should().Be.True();
			mapper.Inverse(false);
			hbm.Inverse.Should().Be.False();
		}

		[Test]
		public void SetMutable()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Mutable(true);
			hbm.Mutable.Should().Be.True();
			mapper.Mutable(false);
			hbm.Mutable.Should().Be.False();
		}

		[Test]
		public void SetWhere()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Where("c > 10");
			hbm.Where.Should().Be.EqualTo("c > 10");
		}

		[Test]
		public void SetBatchSize()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.BatchSize(10);
			hbm.BatchSize.Should().Be.EqualTo(10);
		}

		[Test]
		public void SetLazy()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Lazy(CollectionLazy.Extra);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.Extra);
			mapper.Lazy(CollectionLazy.NoLazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.False);
			mapper.Lazy(CollectionLazy.Lazy);
			hbm.Lazy.Should().Be.EqualTo(HbmCollectionLazy.True);
		}

		[Test]
		public void CallKeyMapper()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			bool kmCalled = false;
			mapper.Key(km => kmCalled = true);
			hbm.Key.Should().Not.Be.Null();
			kmCalled.Should().Be.True();
		}

		[Test]
		public void SetCollectionTypeByWrongTypeShouldThrow()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			Executing.This(() => mapper.Type(null)).Should().Throw<ArgumentNullException>();
			Executing.This(() => mapper.Type(typeof(object))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void SetCollectionTypeByGenericType()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type<FakeUserCollectionType>();
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void SetCollectionTypeByType()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Type(typeof(FakeUserCollectionType));
			hbm.CollectionType.Should().Contain("FakeUserCollectionType");
		}

		[Test]
		public void CanChangeAccessor()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Access(Accessor.Field);

			hbm.Access.Should().Not.Be.Null();
		}

		[Test]
		public void CanSetCache()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(x => x.Region("pizza"));

			hbm.cache.Should().Not.Be.Null();
		}

		[Test]
		public void WhenSetTwoCachePropertiesInTwoActionsThenSetTheTwoValuesWithoutLostTheFirst()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Cache(ch => ch.Region("pizza"));
			mapper.Cache(ch => ch.Usage(CacheUsage.NonstrictReadWrite));

			var hbmCache = hbm.cache;
			hbmCache.Should().Not.Be.Null();
			hbmCache.region.Should().Be("pizza");
			hbmCache.usage.Should().Be(HbmCacheUsage.NonstrictReadWrite);
		}

		[Test]
		public void CanSetAFilterThroughAction()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == "condition1");
		}

		[Test]
		public void CanSetMoreFiltersThroughAction()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "condition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenSameNameThenOverrideCondition()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", f => f.Condition("condition1"));
			mapper.Filter("filter2", f => f.Condition("condition2"));
			mapper.Filter("filter1", f => f.Condition("anothercondition1"));
			hbm.filter.Length.Should().Be(2);
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter1" && f.condition == "anothercondition1"));
			hbm.filter.Satisfy(filters => filters.Any(f => f.name == "filter2" && f.condition == "condition2"));
		}

		[Test]
		public void WhenActionIsNullThenAddFilterName()
		{
			var hbm = new HbmIdbag { name = "Children" };
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Filter("filter1", null);
			hbm.filter.Length.Should().Be(1);
			hbm.filter[0].Satisfy(f => f.name == "filter1" && f.condition == null);
		}

		[Test]
		public void SetFetchMode()
		{
			var hbm = new HbmIdbag();
			var mapper = new IdBagMapper(typeof(Animal), typeof(Animal), hbm);
			mapper.Fetch(CollectionFetchMode.Subselect);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Subselect);
			hbm.fetchSpecified.Should().Be.True();
			mapper.Fetch(CollectionFetchMode.Join);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Join);
			hbm.fetchSpecified.Should().Be.True();
			mapper.Fetch(CollectionFetchMode.Select);
			hbm.fetch.Should().Be(HbmCollectionFetchMode.Select);
			hbm.fetchSpecified.Should().Be.False();
		}
	}
}