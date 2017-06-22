using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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

			Assert.That(hbmMapping.Imports.Length, Is.EqualTo(2));

			Assert.That(hbmMapping.Imports[0].@class, Is.EqualTo("Dto"));
			Assert.That(hbmMapping.Imports[0].rename, Is.EqualTo("Dto"));

			Assert.That(hbmMapping.Imports[1].@class, Is.EqualTo("Dto"));
			Assert.That(hbmMapping.Imports[1].rename, Is.EqualTo("DtoRenamed"));
		}
	}
}