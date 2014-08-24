using System;

namespace NHibernate.Cfg.Loquacious
{
	internal class MappingsConfiguration : IMappingsConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public MappingsConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		#region Implementation of IMappingsConfiguration

		public IMappingsConfiguration UsingDefaultCatalog(string defaultCatalogName)
		{
			fc.Configuration.SetProperty(Environment.DefaultCatalog, defaultCatalogName);
			return this;
		}

		public IFluentSessionFactoryConfiguration UsingDefaultSchema(string defaultSchemaName)
		{
			fc.Configuration.SetProperty(Environment.DefaultSchema, defaultSchemaName);
			return fc;
		}

		#endregion
	}

	internal class MappingsConfigurationProperties:IMappingsConfigurationProperties
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