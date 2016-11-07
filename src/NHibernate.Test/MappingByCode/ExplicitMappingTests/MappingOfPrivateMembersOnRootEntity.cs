using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class MappingOfPrivateMembersOnRootEntity
	{
		public class MyClass
		{
			private int id;
			private int version;
			private string something;
		}

		[Test]
		public void MapClassWithIdAndProperty()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(ca =>
			{
				ca.Id("id", map =>
				{
					map.Column("MyClassId");
					map.Generator(Generators.HighLow, gmap => gmap.Params(new { max_low = 100 }));
				});
				ca.Version("version", map => { });
				ca.Property("something", map => map.Length(150));
			});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass, Is.Not.Null);
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.name, Is.EqualTo("id"));
			Assert.That(hbmId.access, Is.EqualTo("field"));
			var hbmGenerator = hbmId.generator;
			Assert.That(hbmGenerator, Is.Not.Null);
			Assert.That(hbmGenerator.@class, Is.EqualTo("hilo"));
			Assert.That(hbmGenerator.param[0].name, Is.EqualTo("max_low"));
			Assert.That(hbmGenerator.param[0].GetText(), Is.EqualTo("100"));
			var hbmVersion = hbmClass.Version;
			Assert.That(hbmVersion.name, Is.EqualTo("version"));
			var hbmProperty = hbmClass.Properties.OfType<HbmProperty>().Single();
			Assert.That(hbmProperty.name, Is.EqualTo("something"));
			Assert.That(hbmProperty.access, Is.EqualTo("field"));
			Assert.That(hbmProperty.length, Is.EqualTo("150"));
		}

		[Test]
		public void WhenPrivateMemberDoesNotExistsThenThrow()
		{
			var mapper = new ModelMapper();
			Assert.That(() => mapper.Class<MyClass>(ca =>
			{
				ca.Property("pizza", map => map.Length(150));
			}), Throws.TypeOf<MappingException>());
		}
	}
}