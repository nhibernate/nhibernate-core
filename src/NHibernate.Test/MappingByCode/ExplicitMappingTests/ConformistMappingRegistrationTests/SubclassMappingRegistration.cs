using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NUnit.Framework;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests.ConformistMappingRegistrationTests
{
	public class SubclassMappingRegistration
	{
		public class MyClass
		{
			public int Id { get; set; }
			public string Something { get; set; }
		}
		public class Inherited: MyClass
		{
			public string SomethingElse { get; set; }
		}

		private class MyClassMap: ClassMapping<MyClass>
		{
			public MyClassMap()
			{
				Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow);
				});
				Property(x => x.Something, map => map.Length(150));
			}
		}
		private class InheritedMap : SubclassMapping<Inherited>
		{
			public InheritedMap()
			{
				Property(x => x.SomethingElse, map => map.Length(15));
			}
		}

		[Test]
		public void WhenRegisterClassMappingThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<MyClassMap>();
			mapper.AddMapping<InheritedMap>();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Inherited) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(MyClassMap));
			mapper.AddMapping(typeof(InheritedMap));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Inherited) });

			ModelIsWellFormed(hbmMapping);
		}

		private void ModelIsWellFormed(HbmMapping hbmMapping)
		{
			var hbmClass = hbmMapping.SubClasses.Single();
			Assert.That(hbmClass, Is.Not.Null);
			Assert.That(hbmClass.extends, Is.StringContaining("MyClass"));
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			Assert.That(hbmProperty.name, Is.EqualTo("SomethingElse"));
			Assert.That(hbmProperty.length, Is.EqualTo("15"));
		}
	}
}