using System;
using System.Collections;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Test
{
	public abstract class TestCaseMappingByCode:TestCase
	{
		protected override string[] Mappings
		{
			get { return Array.Empty<string>(); }
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
