using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Bytecode;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	internal class FluentSessionFactoryConfiguration : IFluentSessionFactoryConfiguration
	{
		private readonly Configuration configuration;

		public FluentSessionFactoryConfiguration(Configuration configuration)
		{
			this.configuration = configuration;
			Integrate = new DbIntegrationConfiguration(configuration);
			Caching = new CacheConfiguration(this);
			Proxy = new ProxyConfiguration(this);
			GeneratingCollections = new CollectionFactoryConfiguration(this);
			Mapping = new MappingsConfiguration(this);
		}

		internal Configuration Configuration
		{
			get { return configuration; }
		}

		#region Implementation of IFluentSessionFactoryConfiguration

		public IFluentSessionFactoryConfiguration Named(string sessionFactoryName)
		{
			configuration.SetProperty(Environment.SessionFactoryName, sessionFactoryName);
			return this;
		}

		public IDbIntegrationConfiguration Integrate { get; private set; }

		public ICacheConfiguration Caching { get; private set; }

		public IFluentSessionFactoryConfiguration GenerateStatistics()
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			return this;
		}

		public IFluentSessionFactoryConfiguration Using(EntityMode entityMode)
		{
			configuration.SetProperty(Environment.DefaultEntityMode, EntityModeHelper.ToString(entityMode));
			return this;
		}

		public IFluentSessionFactoryConfiguration ParsingHqlThrough<TQueryTranslator>()
			where TQueryTranslator : IQueryTranslatorFactory
		{
			configuration.SetProperty(Environment.QueryTranslator, typeof (TQueryTranslator).AssemblyQualifiedName);
			return this;
		}

		public IProxyConfiguration Proxy { get; private set; }
		public ICollectionFactoryConfiguration GeneratingCollections { get; private set; }
		public IMappingsConfiguration Mapping { get; private set; }

		#endregion
	}

	internal class DbIntegrationConfiguration : IDbIntegrationConfiguration
	{
		private readonly Configuration configuration;

		public DbIntegrationConfiguration(Configuration configuration)
		{
			this.configuration = configuration;
			Connected = new ConnectionConfiguration(this);
			BatchingQueries = new BatcherConfiguration(this);
			Transactions = new TransactionConfiguration(this);
			CreateCommands = new CommandsConfiguration(this);
			Schema = new DbSchemaIntegrationConfiguration(this);
		}

		public Configuration Configuration
		{
			get { return configuration; }
		}

		#region Implementation of IDbIntegrationConfiguration

		public IDbIntegrationConfiguration Using<TDialect>() where TDialect : Dialect.Dialect
		{
			configuration.SetProperty(Environment.Dialect, typeof (TDialect).AssemblyQualifiedName);
			return this;
		}

		public IDbIntegrationConfiguration DisableKeywordsAutoImport()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "none");
			return this;
		}

		public IDbIntegrationConfiguration AutoQuoteKeywords()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			return this;
		}

		public IDbIntegrationConfiguration LogSqlInConsole()
		{
			configuration.SetProperty(Environment.ShowSql, "true");
			return this;
		}

		public IDbIntegrationConfiguration DisableLogFormatedSql()
		{
			configuration.SetProperty(Environment.FormatSql, "false");
			return this;
		}

		public IConnectionConfiguration Connected { get; private set; }
		public IBatcherConfiguration BatchingQueries { get; private set; }
		public ITransactionConfiguration Transactions { get; private set; }

		public ICommandsConfiguration CreateCommands { get; private set; }

		public IDbSchemaIntegrationConfiguration Schema { get; private set; }

		#endregion
	}

	internal class DbSchemaIntegrationConfiguration : IDbSchemaIntegrationConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public DbSchemaIntegrationConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IDbSchemaIntegrationConfiguration

		public IDbIntegrationConfiguration Recreating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, "create-drop");
			return dbc;
		}

		public IDbIntegrationConfiguration Creating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, "create");
			return dbc;
		}

		public IDbIntegrationConfiguration Updating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, "update");
			return dbc;
		}

		public IDbIntegrationConfiguration Validating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, "validate");
			return dbc;
		}

		#endregion
	}

	internal class CommandsConfiguration : ICommandsConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public CommandsConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of ICommandsConfiguration

		public ICommandsConfiguration Preparing()
		{
			dbc.Configuration.SetProperty(Environment.PrepareSql, "true");
			return this;
		}

		public ICommandsConfiguration WithTimeout(int seconds)
		{
			dbc.Configuration.SetProperty(Environment.CommandTimeout, seconds.ToString());
			return this;
		}

		public ICommandsConfiguration ConvertingExceptionsThrough<TExceptionConverter>()
			where TExceptionConverter : ISQLExceptionConverter
		{
			dbc.Configuration.SetProperty(Environment.SqlExceptionConverter, typeof (TExceptionConverter).AssemblyQualifiedName);
			return this;
		}

		public ICommandsConfiguration AutoCommentingSql()
		{
			dbc.Configuration.SetProperty(Environment.UseSqlComments, "true");
			return this;
		}

		public IDbIntegrationConfiguration WithHqlToSqlSubstitutions(string csvQuerySubstitutions)
		{
			dbc.Configuration.SetProperty(Environment.QuerySubstitutions, csvQuerySubstitutions);
			return dbc;
		}

		public IDbIntegrationConfiguration WithDefaultHqlToSqlSubstitutions()
		{
			return dbc;
		}

		public ICommandsConfiguration WithMaximumDepthOfOuterJoinFetching(byte maxFetchDepth)
		{
			dbc.Configuration.SetProperty(Environment.MaxFetchDepth, maxFetchDepth.ToString());
			return this;
		}

		#endregion
	}

	internal class TransactionConfiguration : ITransactionConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public TransactionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of ITransactionConfiguration

		public IDbIntegrationConfiguration Through<TFactory>() where TFactory : ITransactionFactory
		{
			dbc.Configuration.SetProperty(Environment.TransactionStrategy, typeof (TFactory).AssemblyQualifiedName);
			return dbc;
		}

		#endregion
	}

	internal class BatcherConfiguration : IBatcherConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public BatcherConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IBatcherConfiguration

		public IBatcherConfiguration Through<TBatcher>() where TBatcher : IBatcherFactory
		{
			dbc.Configuration.SetProperty(Environment.BatchStrategy, typeof (TBatcher).AssemblyQualifiedName);
			return this;
		}

		public IDbIntegrationConfiguration Each(short batchSize)
		{
			dbc.Configuration.SetProperty(Environment.BatchSize, batchSize.ToString());
			return dbc;
		}

		#endregion
	}

	internal class ConnectionConfiguration : IConnectionConfiguration
	{
		private readonly DbIntegrationConfiguration dbc;

		public ConnectionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		#region Implementation of IConnectionConfiguration

		public IConnectionConfiguration Through<TProvider>() where TProvider : IConnectionProvider
		{
			dbc.Configuration.SetProperty(Environment.ConnectionProvider, typeof (TProvider).AssemblyQualifiedName);
			return this;
		}

		public IConnectionConfiguration By<TDriver>() where TDriver : IDriver
		{
			dbc.Configuration.SetProperty(Environment.ConnectionDriver, typeof (TDriver).AssemblyQualifiedName);
			return this;
		}

		public IConnectionConfiguration With(IsolationLevel level)
		{
			dbc.Configuration.SetProperty(Environment.Isolation, level.ToString());
			return this;
		}

		public IConnectionConfiguration Releasing(ConnectionReleaseMode releaseMode)
		{
			dbc.Configuration.SetProperty(Environment.ReleaseConnections, ConnectionReleaseModeParser.ToString(releaseMode));
			return this;
		}

		public IDbIntegrationConfiguration Using(string connectionString)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionString);
			return dbc;
		}

		public IDbIntegrationConfiguration Using(DbConnectionStringBuilder connectionStringBuilder)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionStringBuilder.ConnectionString);
			return dbc;
		}

		public IDbIntegrationConfiguration ByAppConfing(string connectionStringName)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionStringName, connectionStringName);
			return dbc;
		}

		#endregion
	}

	internal class CacheConfiguration : ICacheConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public CacheConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
			Queries = new QueryCacheConfiguration(this);
		}

		internal Configuration Configuration
		{
			get { return fc.Configuration; }
		}

		#region Implementation of ICacheConfiguration

		public ICacheConfiguration Through<TProvider>() where TProvider : ICacheProvider
		{
			fc.Configuration.SetProperty(Environment.CacheProvider, typeof (TProvider).AssemblyQualifiedName);
			return this;
		}

		public ICacheConfiguration PrefixingRegionsWith(string regionPrefix)
		{
			fc.Configuration.SetProperty(Environment.CacheRegionPrefix, regionPrefix);
			return this;
		}

		public ICacheConfiguration UsingMinimalPuts()
		{
			fc.Configuration.SetProperty(Environment.UseMinimalPuts, "true");
			return this;
		}

		public IFluentSessionFactoryConfiguration WithDefaultExpiration(byte seconds)
		{
			fc.Configuration.SetProperty(Environment.CacheDefaultExpiration, seconds.ToString());
			return fc;
		}

		public IQueryCacheConfiguration Queries { get; private set; }

		#endregion
	}

	internal class QueryCacheConfiguration : IQueryCacheConfiguration
	{
		private readonly CacheConfiguration cc;

		public QueryCacheConfiguration(CacheConfiguration cc)
		{
			this.cc = cc;
		}

		#region Implementation of IQueryCacheConfiguration

		public ICacheConfiguration Through<TFactory>() where TFactory : IQueryCache
		{
			cc.Configuration.SetProperty(Environment.QueryCacheFactory, typeof (TFactory).AssemblyQualifiedName);
			return cc;
		}

		#endregion
	}

	internal class ProxyConfiguration : IProxyConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public ProxyConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		#region Implementation of IProxyConfiguration

		public IProxyConfiguration DisableValidation()
		{
			fc.Configuration.SetProperty(Environment.UseProxyValidator, "false");
			return this;
		}

		public IFluentSessionFactoryConfiguration Through<TProxyFactoryFactory>()
			where TProxyFactoryFactory : IProxyFactoryFactory
		{
			fc.Configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
			                             typeof (TProxyFactoryFactory).AssemblyQualifiedName);
			return fc;
		}

		#endregion
	}

	internal class CollectionFactoryConfiguration : ICollectionFactoryConfiguration
	{
		private readonly FluentSessionFactoryConfiguration fc;

		public CollectionFactoryConfiguration(FluentSessionFactoryConfiguration parent)
		{
			fc = parent;
		}

		#region Implementation of ICollectionFactoryConfiguration

		public IFluentSessionFactoryConfiguration Through<TCollecionsFactory>()
			where TCollecionsFactory : ICollectionTypeFactory
		{
			fc.Configuration.SetProperty(Environment.CollectionTypeFactoryClass,
			                             typeof (TCollecionsFactory).AssemblyQualifiedName);
			return fc;
		}

		#endregion
	}

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
}