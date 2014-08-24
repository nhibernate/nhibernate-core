using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NUnit.Framework;
using SharpTestsEx;

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
			hbmClass.Properties.Should().Have.Count.EqualTo(1);
			var hbmComponent = hbmClass.Properties.OfType<HbmComponent>().Single();
			hbmComponent.name.Should().Be("Name");
			hbmComponent.Properties.Should().Have.Count.EqualTo(2);
			var hbmp1 = hbmComponent.Properties.OfType<HbmProperty>().Single(x => x.name == "First");
			var hbmp2 = hbmComponent.Properties.OfType<HbmProperty>().Single(x => x.name == "Last");
			hbmp1.length.Should().Be("20");
			hbmp2.length.Should().Be("30");
			hbmComponent.unique.Should().Be.EqualTo(true);
		}
	}
}