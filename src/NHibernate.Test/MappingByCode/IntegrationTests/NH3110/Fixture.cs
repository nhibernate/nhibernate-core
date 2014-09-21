using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3110
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CanSetPolymorphism()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Polymorphism(PolymorphismType.Explicit);
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

			var entity = mapping.RootClasses[0];
			Assert.AreEqual(entity.polymorphism, HbmPolymorphismType.Explicit);
		}
	}
}