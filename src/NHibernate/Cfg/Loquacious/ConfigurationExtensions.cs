using System;
using NHibernate.Hql;
namespace NHibernate.Cfg.Loquacious
{
	public static class ConfigurationExtensions
	{
		public static IFluentSessionFactoryConfiguration SessionFactory(this Configuration configuration)
		{
			return new FluentSessionFactoryConfiguration(configuration);
		}

		public static Configuration SessionFactoryName(this Configuration configuration, string sessionFactoryName)
		{
			configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return configuration;
		}

		public static Configuration Cache(this Configuration configuration, Action<ICacheConfigurationProperties> cacheProperties)
		{
			cacheProperties(new CacheConfigurationProperties(configuration));
			return configuration;
		}

		public static Configuration CollectionTypeFactory<TCollecionsFactory>(this Configuration configuration)
		{
			configuration.SetProperty(Environment.CollectionTypeFactoryClass,
																	 typeof(TCollecionsFactory).AssemblyQualifiedName);
			return configuration;
		}

		public static Configuration Proxy(this Configuration configuration, Action<IProxyConfigurationProperties> proxyProperties)
		{
			proxyProperties(new ProxyConfigurationProperties(configuration));
			return configuration;
		}

		public static Configuration HqlQueryTranslator<TQueryTranslator>(this Configuration configuration) where TQueryTranslator : IQueryTranslatorFactory
		{
			configuration.SetProperty(Environment.QueryTranslator, typeof(TQueryTranslator).AssemblyQualifiedName);
			return configuration;
		}

		public static Configuration Mappings(this Configuration configuration, Action<IMappingsConfigurationProperties> mappingsProperties)
		{
			mappingsProperties(new MappingsConfigurationProperties(configuration));
			return configuration;
		}

		public static Configuration DataBaseIntegration(this Configuration configuration, Action<IDbIntegrationConfigurationProperties> dataBaseIntegration)
		{
			dataBaseIntegration(new DbIntegrationConfigurationProperties(configuration));
			return configuration;
		}
	}
}