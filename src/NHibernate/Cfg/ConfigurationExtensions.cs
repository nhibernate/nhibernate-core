using System;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	public static class ConfigurationByCodeExtensions
	{
		public static Configuration ByCode(this Configuration configuration, Action<ConfigurationByCode> byCode)
		{
			byCode(new ConfigurationByCode(configuration));
			return configuration;
		}

		public static ConfigurationByCode ByCode(this Configuration configuration)
		{
			return new ConfigurationByCode(configuration);
		}
	}

	//Since 5.3
	[Obsolete]
	public static class ConfigurationExtensions
	{
		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static IFluentSessionFactoryConfiguration SessionFactory(this Configuration configuration)
		{
			return new FluentSessionFactoryConfiguration(configuration);
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration SessionFactoryName(this Configuration configuration, string sessionFactoryName)
		{
			configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration Cache(this Configuration configuration, Action<ICacheConfigurationProperties> cacheProperties)
		{
			cacheProperties(new CacheConfigurationProperties(configuration));
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration CollectionTypeFactory<TCollecionsFactory>(this Configuration configuration)
		{
			configuration.SetProperty(Environment.CollectionTypeFactoryClass,
																	 typeof(TCollecionsFactory).AssemblyQualifiedName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration Proxy(this Configuration configuration, Action<IProxyConfigurationProperties> proxyProperties)
		{
			proxyProperties(new ProxyConfigurationProperties(configuration));
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration HqlQueryTranslator<TQueryTranslator>(this Configuration configuration) where TQueryTranslator : IQueryTranslatorFactory
		{
			configuration.SetProperty(Environment.QueryTranslator, typeof(TQueryTranslator).AssemblyQualifiedName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration LinqQueryProvider<TQueryProvider>(this Configuration configuration) where TQueryProvider : INhQueryProvider
		{
			configuration.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration LinqToHqlGeneratorsRegistry<TLinqToHqlGeneratorsRegistry>(this Configuration configuration) where TLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
		{
			configuration.SetProperty(Environment.LinqToHqlGeneratorsRegistry, typeof(TLinqToHqlGeneratorsRegistry).AssemblyQualifiedName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration CurrentSessionContext<TCurrentSessionContext>(this Configuration configuration) where TCurrentSessionContext : ICurrentSessionContext
		{
			configuration.SetProperty(Environment.CurrentSessionContextClass, typeof(TCurrentSessionContext).AssemblyQualifiedName);
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration Mappings(this Configuration configuration, Action<IMappingsConfigurationProperties> mappingsProperties)
		{
			mappingsProperties(new MappingsConfigurationProperties(configuration));
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration DataBaseIntegration(this Configuration configuration, Action<IDbIntegrationConfigurationProperties> dataBaseIntegration)
		{
			dataBaseIntegration(new DbIntegrationConfigurationProperties(configuration));
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration EntityCache<TEntity>(this Configuration configuration, Action<IEntityCacheConfigurationProperties<TEntity>> entityCacheConfiguration)
			where TEntity : class
		{
			var ecc = new EntityCacheConfigurationProperties<TEntity>();
			entityCacheConfiguration(ecc);
			if (ecc.Strategy.HasValue)
			{
				configuration.SetCacheConcurrencyStrategy(typeof(TEntity).FullName, EntityCacheUsageParser.ToString(ecc.Strategy.Value),
																									ecc.RegionName);
			}
			foreach (var collection in ecc.Collections)
			{
				configuration.SetCollectionCacheConcurrencyStrategy(collection.Key,
																														EntityCacheUsageParser.ToString(collection.Value.Strategy),
																														collection.Value.RegionName);
			}
			return configuration;
		}

		/// <summary>
		/// Add a type-definition for mappings.
		/// </summary>
		/// <typeparam name="TDef">The persistent type.</typeparam>
		/// <param name="configuration">The <see cref="Configuration"/> where add the type-definition.</param>
		/// <param name="typeDefConfiguration">The custom configuration action.</param>
		/// <returns>The <see cref="Configuration"/>.</returns>
		/// <remarks>
		/// <para>
		/// <list type="bullet">
		/// <listheader>
		/// <description>Depending on where you will use the type-definition in the mapping the
		///  <typeparamref name="TDef"/> can be :
		/// </description>
		///</listheader>
		///<item>
		///    <term><see cref="NHibernate.UserTypes.IUserType"/></term>
		///</item>
		///<item>
		///    <term><see cref="NHibernate.UserTypes.IUserCollectionType"/></term>
		///</item>
		///<item>
		///    <term><see cref="NHibernate.UserTypes.IUserVersionType"/></term>
		///</item>
		///<item>
		///    <term><see cref="NHibernate.Id.IPersistentIdentifierGenerator"/> </term>
		///</item>
		///</list>
		/// </para>
		/// </remarks>
		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration TypeDefinition<TDef>(this Configuration configuration, Action<ITypeDefConfigurationProperties> typeDefConfiguration)
			where TDef : class
		{
			if (typeDefConfiguration == null)
			{
				return configuration;
			}
			var tdConfiguration = TypeDefConfigurationProperties.Create<TDef>();
			typeDefConfiguration(tdConfiguration);
			if(string.IsNullOrEmpty(tdConfiguration.Alias))
			{
				return configuration;
			}
			var mappings = configuration.CreateMappings();
			mappings.LazyDialect = new Lazy<Dialect.Dialect>(() => Dialect.Dialect.GetDialect(configuration.Properties));
			mappings.AddTypeDef(tdConfiguration.Alias, typeof(TDef).AssemblyQualifiedName, tdConfiguration.Properties.ToTypeParameters());
			return configuration;
		}

		//Since 5.3
		[Obsolete("Please call this method from configuration.ByCode extension")]
		public static Configuration AddNamedQuery(this Configuration configuration, string queryIdentifier, Action<INamedQueryDefinitionBuilder> namedQueryDefinition)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (queryIdentifier == null)
			{
				throw new ArgumentNullException("queryIdentifier");
			}
			if (namedQueryDefinition == null)
			{
				throw new ArgumentNullException("namedQueryDefinition");
			}
			var builder = new NamedQueryDefinitionBuilder();
			namedQueryDefinition(builder);
			configuration.NamedQueries.Add(queryIdentifier, builder.Build());
			return configuration;
		}
	}
}
