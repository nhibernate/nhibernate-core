using NUnit.Framework;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg;

namespace NHibernate.Test.NHSpecificTest.NH3016
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ShouldAllowMappingComponentAsIdWithNestedClass()
		{
			var cfg = new Configuration();
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc => rc.ComponentAsId(entity => entity.Id, cid => cid.Property(p => p.Id)));

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddDeserializedMapping(mapping, "TestDomain");
		}
	}
}