using System;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	// "Loquacious" part of Configuration
	public partial class Configuration
	{
		public FluentSessionFactoryConfiguration SessionFactory()
		{
			return new FluentSessionFactoryConfiguration(this);
		}

		public Configuration SessionFactory(Action<FluentSessionFactoryConfiguration> configure)
		{
			configure(SessionFactory());
			return this;
		}

		public Configuration SessionFactoryName(string sessionFactoryName)
		{
			this.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		public Configuration Cache(Action<CacheConfigurationProperties> cacheProperties)
		{
			cacheProperties(new CacheConfigurationProperties(this));
			return this;
		}

		public Configuration CollectionTypeFactory<TCollectionsFactory>()
		{
			this.SetProperty(
				Environment.CollectionTypeFactoryClass,
				typeof(TCollectionsFactory).AssemblyQualifiedName);
			return this;
		}

		public Configuration Proxy(Action<ProxyConfigurationProperties> proxyProperties)
		{
			proxyProperties(new ProxyConfigurationProperties(this));
			return this;
		}

		public Configuration HqlQueryTranslator<TQueryTranslator>() where TQueryTranslator : IQueryTranslatorFactory
		{
			this.SetProperty(Environment.QueryTranslator, typeof(TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		public Configuration LinqQueryProvider<TQueryProvider>() where TQueryProvider : INhQueryProvider
		{
			this.SetProperty(Environment.QueryLinqProvider, typeof(TQueryProvider).AssemblyQualifiedName);
			return this;
		}

		public Configuration LinqToHqlGeneratorsRegistry<TLinqToHqlGeneratorsRegistry>() where TLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
		{
			this.SetProperty(Environment.LinqToHqlGeneratorsRegistry, typeof(TLinqToHqlGeneratorsRegistry).AssemblyQualifiedName);
			return this;
		}

		public Configuration CurrentSessionContext<TCurrentSessionContext>() where TCurrentSessionContext : ICurrentSessionContext
		{
			this.SetProperty(Environment.CurrentSessionContextClass, typeof(TCurrentSessionContext).AssemblyQualifiedName);
			return this;
		}

		public Configuration Mappings(Action<MappingsConfigurationProperties> mappingsProperties)
		{
			mappingsProperties(new MappingsConfigurationProperties(this));
			return this;
		}

		public Configuration DataBaseIntegration(Action<DbIntegrationConfigurationProperties> dataBaseIntegration)
		{
			dataBaseIntegration(new DbIntegrationConfigurationProperties(this));
			return this;
		}

		public Configuration EntityCache<TEntity>(Action<EntityCacheConfigurationProperties<TEntity>> entityCacheConfiguration)
			where TEntity : class
		{
			var ecc = new EntityCacheConfigurationProperties<TEntity>();
			entityCacheConfiguration(ecc);
			if (ecc.Strategy.HasValue)
			{
				this.SetCacheConcurrencyStrategy(
					typeof(TEntity).FullName,
					EntityCacheUsageParser.ToString(ecc.Strategy.Value),
					ecc.RegionName);
			}

			foreach (var collection in ecc.Collections)
			{
				this.SetCollectionCacheConcurrencyStrategy(
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
		public Configuration TypeDefinition<TDef>(Action<TypeDefConfigurationProperties> typeDefConfiguration)
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

			var mappings = this.CreateMappings();
			mappings.LazyDialect = new Lazy<Dialect.Dialect>(() => Dialect.Dialect.GetDialect(this.Properties));
			mappings.AddTypeDef(tdConfiguration.Alias, typeof(TDef).AssemblyQualifiedName, tdConfiguration.Properties.ToTypeParameters());
			return this;
		}

		public Configuration AddNamedQuery(string queryIdentifier, Action<NamedQueryDefinitionBuilder> namedQueryDefinition)
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
			this.NamedQueries.Add(queryIdentifier, builder.Build());
			return this;
		}
	}
}
