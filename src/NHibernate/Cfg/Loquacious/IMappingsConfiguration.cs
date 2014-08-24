namespace NHibernate.Cfg.Loquacious
{
	public interface IMappingsConfiguration
	{
		IMappingsConfiguration UsingDefaultCatalog(string defaultCatalogName);
		IFluentSessionFactoryConfiguration UsingDefaultSchema(string defaultSchemaName);
	}

	public interface IMappingsConfigurationProperties
	{
		string DefaultCatalog { set; }
		string DefaultSchema { set; }
	}
}