using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests.ConformistMappingRegistrationTests
{
	public class ComponentMappingRegistrationTest
	{
		public class MyClass
		{
			public int Id { get; set; }
			public Name Name { get; set; }
		}

		public class Name
		{
			public string First { get; set; }
			public string Last { get; set; }
		}

		private class MyClassMap : ClassMapping<MyClass>
		{
			public MyClassMap()
			{
				Id(x => x.Id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow);
				});
				Component(x => x.Name);
			}
		}

		private class NameMap : ComponentMapping<Name>
		{
			public NameMap()
			{
				Property(x => x.First, map => map.Length(20));
				Property(x => x.Last, map => map.Length(30));
				Unique(true);
			}
		}

		[Test]
		public void WhenRegisterClassMappingThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping<MyClassMap>();
			mapper.AddMapping<NameMap>();
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		[Test]
		public void WhenRegisterClassMappingThroughTypeThenMapTheClass()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(MyClassMap));
			mapper.AddMapping(typeof(NameMap));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			ModelIsWellFormed(hbmMapping);
		}

		private void ModelIsWellFormed(HbmMapping hbmMapping)
		{
			var hbmClass = hbmMapping.RootClasses.Single();
			Assert.That(hbmClass.Properties.Count(), Is.EqualTo(1));
			var hbmComponent = hbmClass.Properties.OfType<HbmComponent>().Single();
			Assert.That(hbmComponent.name, Is.EqualTo("Name"));
			Assert.That(hbmComponent.Properties.Count(), Is.EqualTo(2));
			var hbmp1 = hbmComponent.Properties.OfType<HbmProperty>().Single(x => x.name == "First");
			var hbmp2 = hbmComponent.Properties.OfType<HbmProperty>().Single(x => x.name == "Last");
			Assert.That(hbmp1.length, Is.EqualTo("20"));
			Assert.That(hbmp2.length, Is.EqualTo("30"));
			Assert.That(hbmComponent.unique, Is.EqualTo(true));
		}
	}
}