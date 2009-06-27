namespace NHibernate.Cfg.Loquacious
{
	public interface IMappingsConfiguration
	{
		IMappingsConfiguration UsingDefaultCatalog(string defaultCatalogName);
		IFluentSessionFactoryConfiguration UsingDefaultSchema(string defaultSchemaName);
	}
}