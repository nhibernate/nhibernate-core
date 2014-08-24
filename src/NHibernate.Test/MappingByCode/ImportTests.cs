using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode
{
	public class Dto { }

	public class Entity { }

	public class ImportTest
	{
		[Test]
		public void ImportClass()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity>(e => { });

			mapper.Import<Dto>();
			mapper.Import<Dto>("DtoRenamed");

			var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			hbmMapping.Imports.Length.Should().Be.EqualTo(2);

			hbmMapping.Imports[0].@class.Should().Be.EqualTo("Dto");
			hbmMapping.Imports[0].rename.Should().Be.EqualTo("Dto");

			hbmMapping.Imports[1].@class.Should().Be.EqualTo("Dto");
			hbmMapping.Imports[1].rename.Should().Be.EqualTo("DtoRenamed");
		}
	}
}