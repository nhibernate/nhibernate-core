namespace NHibernate.Cfg.Loquacious
{
	public interface IMappingsConfiguration
	{
		IMappingsConfiguration UsingDefaultSchema(string defaultSchemaName);
		IMappingsConfiguration UsingDefaultCatalog(string defaultCatalogName);
	}
}