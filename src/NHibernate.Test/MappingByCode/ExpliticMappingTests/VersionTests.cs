using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class VersionTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			public int Version { get; set; }
		}

		[Test]
		public void WhenPropertyUsedAsPoidThenRegister()
		{
			var inspector = new ExplicitlyDeclaredModel();
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
					map.Version(x => x.Version, vm => { });
				});

			inspector.IsVersion(For<MyClass>.Property(x => x.Version)).Should().Be.True();
		}
	}
}