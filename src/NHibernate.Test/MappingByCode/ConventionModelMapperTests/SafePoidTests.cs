using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class SafePoidTests
	{
		private class MyClassWithoutPoid
		{
			
		}

		private class MyClass
		{
			private int _id;
			public int Id
			{
				get { return _id; }
			}
		}

		[Test]
		public void WhenClassWithoutPoidNorGeeneratorThenApplyGuid()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClassWithoutPoid>(x => { });
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClassWithoutPoid) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.generator.Should().Not.Be.Null();
			hbmId.generator.@class.Should().Be("guid");
			hbmId.type1.Should().Be("Guid");
		}

		[Test]
		public void WhenClassWithoutPoidWithGeneratorThenApplyDefinedGenerator()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClassWithoutPoid>(x => x.Id(null, idm=> idm.Generator(Generators.Native)));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClassWithoutPoid) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmId = hbmClass.Id;
			hbmId.Should().Not.Be.Null();
			hbmId.generator.Should().Not.Be.Null();
			hbmId.generator.@class.Should().Be("native");
			hbmId.type1.Should().Be(Generators.Native.DefaultReturnType.GetNhTypeName());
		}

		[Test]
		public void WhenPoidNoSetterThenApplyNosetter()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(x => x.Id(mc=> mc.Id));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			hbmClass.Id.access.Should().Be("nosetter.camelcase-underscore");
		}
	}
}