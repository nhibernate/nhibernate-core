namespace NHibernate.Cfg.Loquacious
{
	public class MappingsConfiguration 
#pragma warning disable 618
		: IMappingsConfiguration
#pragma warning restore 618
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public MappingsConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		public MappingsConfiguration UsingDefaultCatalog(string defaultCatalogName)
		{
			fc.Configuration.SetProperty(Environment.DefaultCatalog, defaultCatalogName);
			return this;
		}

		public FluentSessionFactoryConfiguration UsingDefaultSchema(string defaultSchemaName)
		{
			fc.Configuration.SetProperty(Environment.DefaultSchema, defaultSchemaName);
			return fc;
		}

		#region Implementation of IMappingsConfiguration
#pragma warning disable 618

		IMappingsConfiguration IMappingsConfiguration.UsingDefaultCatalog(string defaultCatalogName)
		{
			return UsingDefaultCatalog(defaultCatalogName);
		}

		IFluentSessionFactoryConfiguration IMappingsConfiguration.UsingDefaultSchema(string defaultSchemaName)
		{
			return UsingDefaultSchema(defaultSchemaName);
		}

#pragma warning restore 618
		#endregion
	}

	public class MappingsConfigurationProperties
#pragma warning disable 618
		:IMappingsConfigurationProperties
#pragma warning restore 618
	{
		private readonly Configuration configuration;

		public MappingsConfigurationProperties(Configuration configuration)
		{
			this.configuration = configuration;
		}

		#region Implementation of IMappingsConfigurationProperties

		public string DefaultCatalog
		{
			set { configuration.SetProperty(Environment.DefaultCatalog, value); }
		}

		public string DefaultSchema
		{
			set { configuration.SetProperty(Environment.DefaultSchema, value); }
		}

		#endregion
	}
}
