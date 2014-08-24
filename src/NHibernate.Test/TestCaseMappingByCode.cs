using System.Collections;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Test
{
	public abstract class TestCaseMappingByCode:TestCase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		protected override string MappingsAssembly
		{
			get { return null; }
		}

		protected override void AddMappings(Cfg.Configuration configuration)
		{
			configuration.AddDeserializedMapping(GetMappings(), "TestDomain");
		}

		protected abstract HbmMapping GetMappings();
	}
}