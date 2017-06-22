using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitMappingTests
{
	[TestFixture]
	public class MappingOfInternalMembersOnRootEntity
	{
		public class MyClass
		{
			protected internal int _id;
			protected internal int _version;
			protected internal string _something;
		}

		[Test]
		public void MapClassWithInternalIdAndProperty()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id(x => x._id, map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Version(x => x._version, map => { });
				ca.Property(x => x._something, map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);

			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.name, Is.EqualTo("_id"));
			Assert.That(hbmId.access, Is.EqualTo("field"));

			var hbmIdGenerator = hbmId.generator;
			Assert.That(hbmIdGenerator, Is.Not.Null);
			Assert.That(hbmIdGenerator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmIdGenerator.param[0].name, Is.EqualTo("max_low"));
			Assert.That(hbmIdGenerator.param[0].GetText(), Is.EqualTo("100"));

			var hbmVersion = hbmClass.Version;
			Assert.That(hbmVersion, Is.Not.Null);
			Assert.That(hbmVersion.name, Is.EqualTo("_version"));

			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			Assert.That(hbmProperty.name, Is.EqualTo("_something"));
			Assert.That(hbmProperty.access, Is.EqualTo("field"));
			Assert.That(hbmProperty.length, Is.EqualTo("150"));
		}
	}
}