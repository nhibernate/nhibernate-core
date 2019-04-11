using System;
using NHibernate.Context;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace NHibernate.Cfg.Loquacious
{
	/// <summary>
	/// Class for "Loquacious" NHibernate configuration
	/// </summary>
	public class ConfigurationByCode
	{
		private readonly Configuration _configuration;

		internal ConfigurationByCode(Configuration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		public FluentSessionFactoryConfiguration SessionFactory()
		{
			return new FluentSessionFactoryConfiguration(_configuration);
		}

		public ConfigurationByCode SessionFactory(Action<FluentSessionFactoryConfiguration> configure)
		{
			configure(SessionFactory());
			return this;
		}

		public ConfigurationByCode SessionFactoryName(string sessionFactoryName)
		{
			_configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		public ConfigurationByCode Cache(Action<CacheConfigurationProperties> cacheProperties)
		{
			cacheProperties(new CacheConfigurationProperties(_configuration));
			return this;
		}

		public ConfigurationByCode CollectionTypeFactory<TCollectionsFactory>()
		{
			_configuration.SetProperty(
				Environment.CollectionTypeFactoryClass,
				typeof(TCollectionsFactory).AssemblyQualifiedName);
			return this;
		}

		public ConfigurationByCode Proxy(Action<ProxyConfigurationProperties> proxyProperties)
		{
			proxyProperties(new ProxyConfigurationProperties(_configuration));
			return this;
		}

		public ConfigurationByCode HqlQueryTranslator<TQueryTranslator>() where TQueryTranslator : IQueryTranslatorFactory
		{
			_configuration.SetProperty(Environment.QueryTranslator, typeof(TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		public ConfigurationByCode LinqQueryProvider<TQueryProvider>() where TQueryProvider : INhQueryProvider
		{
			_configuration.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return this;
		}

		public ConfigurationByCode LinqToHqlGeneratorsRegistry<TLinqToHqlGeneratorsRegistry>() where TLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
		{
			_configuration.SetProperty(Environment.LinqToHqlGeneratorsRegistry, typeof(TLinqToHqlGeneratorsRegistry).AssemblyQualifiedName);
			return this;
		}

		public ConfigurationByCode CurrentSessionContext<TCurrentSessionContext>() where TCurrentSessionContext : ICurrentSessionContext
		{
			_configuration.SetProperty(Environment.CurrentSessionContextClass, typeof(TCurrentSessionContext).AssemblyQualifiedName);
			return this;
		}

		public ConfigurationByCode Mappings(Action<MappingsConfigurationProperties> mappingsProperties)
		{
			mappingsProperties(new MappingsConfigurationProperties(_configuration));
			return this;
		}

		public ConfigurationByCode DataBaseIntegration(Action<DbIntegrationConfigurationProperties> dataBaseIntegration)
		{
			dataBaseIntegration(new DbIntegrationConfigurationProperties(_configuration));
			return this;
		}

		public ConfigurationByCode EntityCache<TEntity>(Action<EntityCacheConfigurationProperties<TEntity>> entityCacheConfiguration)
			where TEntity : class
		{
			var ecc = new EntityCacheConfigurationProperties<TEntity>();
			entityCacheConfiguration(ecc);
			if (ecc.Strategy.HasValue)
			{
				_configuration.SetCacheConcurrencyStrategy(
					typeof(TEntity).FullName,
					EntityCacheUsageParser.ToString(ecc.Strategy.Value),
					ecc.RegionName);
			}

			foreach (var collection in ecc.Collections)
			{
				_configuration.SetCollectionCacheConcurrencyStrategy(
					collection.Key,
					EntityCacheUsageParser.ToString(collection.Value.Strategy),
					collection.Value.RegionName);
			}

			return this;
		}

		/// <summary>
		/// Add a type-definition for mappings.
		/// </summary>
		/// <typeparam name="TDef">The persistent type.</typeparam>
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
		public ConfigurationByCode TypeDefinition<TDef>(Action<TypeDefConfigurationProperties> typeDefConfiguration)
			where TDef : class
		{
			if (typeDefConfiguration == null)
			{
				return this;
			}

			var tdConfiguration = TypeDefConfigurationProperties.Create<TDef>();
			typeDefConfiguration(tdConfiguration);
			if (string.IsNullOrEmpty(tdConfiguration.Alias))
			{
				return this;
			}

			var mappings = _configuration.CreateMappings();
			mappings.LazyDialect = new Lazy<Dialect.Dialect>(() => Dialect.Dialect.GetDialect(_configuration.Properties));
			mappings.AddTypeDef(tdConfiguration.Alias, typeof(TDef).AssemblyQualifiedName, tdConfiguration.Properties.ToTypeParameters());
			return this;
		}

		public ConfigurationByCode AddNamedQuery(string queryIdentifier, Action<NamedQueryDefinitionBuilder> namedQueryDefinition)
		{
			if (queryIdentifier == null)
			{
				throw new ArgumentNullException(nameof(queryIdentifier));
			}

			if (namedQueryDefinition == null)
			{
				throw new ArgumentNullException(nameof(namedQueryDefinition));
			}

			var builder = new NamedQueryDefinitionBuilder();
			namedQueryDefinition(builder);
			_configuration.NamedQueries.Add(queryIdentifier, builder.Build());
			return this;
		}
	}
}
