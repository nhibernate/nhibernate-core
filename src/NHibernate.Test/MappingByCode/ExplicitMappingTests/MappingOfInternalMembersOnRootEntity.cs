using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class MappingOfInternalMembersOnRootEntity
	{
		public class MyClass
		{
			internal int id;
			internal int version;
			internal string something;
		}

		[Test]
		public void MapClassWithInternalIdAndProperty()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x.id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Version(x => x.version, map => { });
				ca.Property(x => x.something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			hbmClass.Should().Not.Be.Null();
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.name.Should().Be("id");
			hbmId.access.Should().Be("field");
			var hbmGenerator = hbmId.generator;
			hbmGenerator.Should().Not.Be.Null();
			hbmGenerator.@class.Should().Be("hilo");
			hbmGenerator.param[0].name.Should().Be("max_low");
			hbmGenerator.param[0].GetText().Should().Be("100");
			var hbmVersion = hbmClass.Version;
			hbmVersion.name.Should().Be("version");
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			hbmProperty.name.Should().Be("something");
			hbmProperty.access.Should().Be("field");
			hbmProperty.length.Should().Be("150");
		}
	}
}