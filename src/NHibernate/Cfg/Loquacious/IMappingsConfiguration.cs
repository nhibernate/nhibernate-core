using System;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IMappingsConfiguration
	{
		IMappingsConfiguration UsingDefaultCatalog(string defaultCatalogName);
		IFluentSessionFactoryConfiguration UsingDefaultSchema(string defaultSchemaName);
	}

	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IMappingsConfigurationProperties
	{
		string DefaultCatalog { set; }
		string DefaultSchema { set; }
	}
}
