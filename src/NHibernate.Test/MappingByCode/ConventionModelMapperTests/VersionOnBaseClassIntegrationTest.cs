using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ConventionModelMapperTests
{
	public class VersionOnBaseClassIntegrationTest
	{
		private class BaseEntity
		{
			public int Id { get; set; }
			public int Version { get; set; }
		}

		private class Person : BaseEntity
		{
		}

		[Test]
		public void WhenPropertyVersionFromBaseEntityThenFindItAsVersion()
		{
			var mapper = new ConventionModelMapper();
			var baseEntityType = typeof(BaseEntity);
			mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
			mapper.IsRootEntity((t, declared) => baseEntityType.Equals(t.BaseType));
			mapper.Class<BaseEntity>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Version(x => x.Version, vm => { });
				});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmVersion = hbmClass.Version;
			Assert.That(hbmVersion, Is.Not.Null);
			Assert.That(hbmVersion.name, Is.EqualTo("Version"));
		}

		[Test]
		public void WhenVersionFromBaseEntityThenShouldntMapVersionAsProperty()
		{
			var mapper = new ConventionModelMapper();
			var baseEntityType = typeof(BaseEntity);
			mapper.IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface);
			mapper.IsRootEntity((t, declared) => baseEntityType == t.BaseType);
			mapper.Class<BaseEntity>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Version(x => x.Version, vm => { });
				});
			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(Person) });

			var hbmClass = hbmMapping.RootClasses[0];
			var hbmVersion = hbmClass.Version;
			Assert.That(hbmVersion, Is.Not.Null);
			Assert.That(hbmVersion.name, Is.EqualTo("Version"));
			Assert.That(hbmClass.Properties, Is.Empty, "because one is the POID and the other is the version");
		}
	}
}