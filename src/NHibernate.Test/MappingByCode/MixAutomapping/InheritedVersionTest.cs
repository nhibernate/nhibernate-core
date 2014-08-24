using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class InheritedVersionTest
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
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);
			mapper.Class<BaseEntity>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Version(x => x.Version, vm => { });
				});

			inspector.IsVersion(For<Person>.Property(x => x.Version)).Should().Be.True();
		}
	}
}