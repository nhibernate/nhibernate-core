using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.generator, Is.Not.Null);
			Assert.That(hbmId.generator.@class, Is.EqualTo("guid"));
			Assert.That(hbmId.type1, Is.EqualTo("Guid"));
		}

		[Test]
		public void WhenClassWithoutPoidWithGeneratorThenApplyDefinedGenerator()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClassWithoutPoid>(x => x.Id(null, idm=> idm.Generator(Generators.Native)));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClassWithoutPoid) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmId = hbmClass.Id;
			Assert.That(hbmId, Is.Not.Null);
			Assert.That(hbmId.generator, Is.Not.Null);
			Assert.That(hbmId.generator.@class, Is.EqualTo("native"));
			Assert.That(hbmId.type1, Is.EqualTo(Generators.Native.DefaultReturnType.GetNhTypeName()));
		}

		[Test]
		public void WhenPoidNoSetterThenApplyNosetter()
		{
			var mapper = new ConventionModelMapper();
			mapper.Class<MyClass>(x => x.Id(mc=> mc.Id));
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });

			var hbmClass = hbmMapping.RootClasses[0];
			Assert.That(hbmClass.Id.access, Is.EqualTo("nosetter.camelcase-underscore"));
		}
	}
}