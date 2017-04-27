using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3992
{
    [TestFixture]
    public class MappingByCodeTest : BugTestCase
    {
        //no xml mappings here, since we use MappingByCode
        protected override System.Collections.IList Mappings
        {
            get { return new string[0]; }
        }

        [Test]
        public void CompiledMappings_ShouldNotDependOnAddedOrdering_AddedBy_AddMapping()
        {
            var mapper = new ModelMapper();
	        mapper.AddMapping(typeof(BaseEntityMapping));
	        mapper.AddMapping(typeof(MappedEntityMapping));

	        var config = TestConfigurationHelper.GetDefaultConfiguration();
			config.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
		}
    }
}
