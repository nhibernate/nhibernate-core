using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class ManyToManyKeyIdColumnTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public ICollection<MyBidirect> MyBidirects { get; set; }
			public IDictionary<MyBidirect, string> MapKey { get; set; }
			public IDictionary<string, MyBidirect> MapValue { get; set; }
			public MyComponent MyComponent { get; set; }
		}

		private class MyComponent
		{
			public ICollection<MyBidirect> MyBidirects { get; set; }
		}

		private class MyBidirect
		{
			public int Id { get; set; }
			public ICollection<MyClass> MyClasses { get; set; }
		}

		private HbmMapping GetModelMapping()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(map =>
			{
				map.Bag(mc => mc.MyBidirects, c => { }, rel => rel.ManyToMany());
				map.Map(mc => mc.MapKey, c => { }, mk => mk.ManyToMany(), mv => { });
				map.Map(mc => mc.MapValue, c => { }, mk => { }, mv => mv.ManyToMany());
			});
			mapper.Class<MyBidirect>(map =>
			{
				map.Bag(mc => mc.MyClasses, c => { }, rel => rel.ManyToMany());
			});
			mapper.Component<MyComponent>(map =>
			{
				map.Bag(mc => mc.MyBidirects, c => { }, rel => rel.ManyToMany());
			});
			return mapper.CompileMappingFor(new[] { typeof(MyClass), typeof(MyBidirect) });
		}

		[Test]
		public void WhenManyToManyCollectionThenApplyTableAlphabetical()
		{
			HbmMapping mapping = GetModelMapping();

			var hbmMyClass = mapping.RootClasses.Where(x => x.Name.Contains("MyClass")).Single();
			var hbmMyBidirect = mapping.RootClasses.Where(x => x.Name.Contains("MyBidirect")).Single();

			var hbmBagMyClass = hbmMyClass.Properties.Where(p => p.Name == "MyBidirects").OfType<HbmBag>().Single();
			var hbmBagMyBidirect = hbmMyBidirect.Properties.Where(p => p.Name == "MyClasses").OfType<HbmBag>().Single();

			hbmBagMyClass.Key.column1.Should().Be("MyClassId");
			hbmBagMyBidirect.Key.column1.Should().Be("MyBidirectId");
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryKeyThenNoMatch()
		{
			HbmMapping mapping = GetModelMapping();

			var hbmMyClass = mapping.RootClasses.Where(x => x.Name.Contains("MyClass")).Single();
			var hbmMapKey = hbmMyClass.Properties.Where(p => p.Name == "MapKey").OfType<HbmMap>().Single();

			hbmMapKey.Key.Columns.Single().name.Should().Not.Be("MyClassId");
		}

		[Test]
		public void WhenRelationDeclaredAsManyToManyForDictionaryValueThenMatch()
		{
			HbmMapping mapping = GetModelMapping();

			var hbmMyClass = mapping.RootClasses.Where(x => x.Name.Contains("MyClass")).Single();
			var hbmMapValue = hbmMyClass.Properties.Where(p => p.Name == "MapValue").OfType<HbmMap>().Single();

			hbmMapValue.Key.column1.Should().Be("MyClassId");
		}

		[Test]
		public void WhenManyToManyCollectionInsideComponentThenApplyFromEntityToEntity()
		{
			HbmMapping mapping = GetModelMapping();

			var hbmMyClass = mapping.RootClasses.Where(x => x.Name.Contains("MyClass")).Single();
			var hbmMyComponent = hbmMyClass.Properties.Where(p => p.Name == "MyComponent").OfType<HbmComponent>().Single();
			var hbmBagMyClass = hbmMyComponent.Properties.Where(p => p.Name == "MyBidirects").OfType<HbmBag>().Single();

			hbmBagMyClass.Key.column1.Should().Be("MyClassId");
		}
	}
}