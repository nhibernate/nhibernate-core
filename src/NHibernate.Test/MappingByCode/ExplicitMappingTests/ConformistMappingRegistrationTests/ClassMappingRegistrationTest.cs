using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests.ConformistMappingRegistrationTests
{
	public class ClassMappingRegistrationTest
	{
		public class MyClass
		{
			public int Id { get; set; }
			public string Something { get; set; }
		}

		private class MyClassBaseMap<T> : ClassMapping<T> where T:MyClass
		{
			public MyClassBaseMap()
			{
				Id(x => x.Id, map => map.Column("MyClassId"));
			}
		}

		private class MyClassMap: ClassMapping<MyClass>
		{
			public MyClassMap()
			{
				Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				Property(x => x.Something, map => map.Length(150));
			}
		}

		[Test]
		public void WhenRegisterClassMappingThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<MyClassMap>();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(MyClassMap));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughCollectionOfTypeThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMappings(new []{typeof(MyClassMap)});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughCollectionOfTypeThenFilterValidMappings()
		{
			var mapper = new ModelMapper();
			Assert.That(() => mapper.AddMappings(new[] { typeof(object), typeof(MyClassMap), typeof(MyClass), typeof(MyClassBaseMap<>) }), Throws.Nothing);
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenGetMapping()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(MyClassMap));
			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			ModelIsWellFormed(hbmMapping);
		}

		private void ModelIsWellFormed(HbmMapping hbmMapping)
		{
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.name, Is.EqualTo("Id"));
			var hbmGenerator = hbmId.generator;
			Assert.That(hbmGenerator, Is.Not.Null);
			Assert.That(hbmGenerator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmGenerator.param[0].name, Is.EqualTo("max_low"));
			Assert.That(hbmGenerator.param[0].GetText(), Is.EqualTo("100"));
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			Assert.That(hbmProperty.name, Is.EqualTo("Something"));
			Assert.That(hbmProperty.length, Is.EqualTo("150"));
		}
	}
}