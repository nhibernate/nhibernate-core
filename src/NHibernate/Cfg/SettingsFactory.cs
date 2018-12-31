using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Transaction;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Reads configuration properties and configures a <see cref="Settings"/> instance. 
	/// </summary>
	[Serializable]
	public sealed class SettingsFactory
	{
		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(SettingsFactory));
		private static readonly System.Type DefaultCacheProvider = typeof(NoCacheProvider);

		public Settings BuildSettings(IDictionary<string, string> properties)
		{
			Settings settings = new Settings();

			Dialect.Dialect dialect;
			try
			{
				dialect = Dialect.Dialect.GetDialect(properties);
				Dictionary<string, string> temp = new Dictionary<string, string>();

				foreach (KeyValuePair<string, string> de in dialect.DefaultProperties)
				{
					temp[de.Key] = de.Value;
				}
				foreach (KeyValuePair<string, string> de in properties)
				{
					temp[de.Key] = de.Value;
				}
				properties = temp;
			}
			catch (HibernateException he)
			{
				log.Warn(he, "No dialect set - using GenericDialect: {0}", he.Message);
				dialect = new GenericDialect();
			}
			settings.Dialect = dialect;

			settings.LinqToHqlGeneratorsRegistry = LinqToHqlGeneratorsRegistryFactory.CreateGeneratorsRegistry(properties);

			#region SQL Exception converter

			ISQLExceptionConverter sqlExceptionConverter;
			try
			{
				sqlExceptionConverter = SQLExceptionConverterFactory.BuildSQLExceptionConverter(dialect, properties);
			}
			catch (HibernateException he)
			{
				log.Warn(he, "Error building SQLExceptionConverter; using minimal converter");
				sqlExceptionConverter = SQLExceptionConverterFactory.BuildMinimalSQLExceptionConverter();
			}
			settings.SqlExceptionConverter = sqlExceptionConverter;

			#endregion

			bool comments = PropertiesHelper.GetBoolean(Environment.UseSqlComments, properties);
			log.Info("Generate SQL with comments: {0}", EnabledDisabled(comments));
			settings.IsCommentsEnabled = comments;

			int maxFetchDepth = PropertiesHelper.GetInt32(Environment.MaxFetchDepth, properties, -1);
			if (maxFetchDepth != -1)
			{
				log.Info("Maximum outer join fetch depth: {0}", maxFetchDepth);
			}

			IConnectionProvider connectionProvider = ConnectionProviderFactory.NewConnectionProvider(properties);
			ITransactionFactory transactionFactory = CreateTransactionFactory(properties);
			// TransactionManagerLookup transactionManagerLookup = TransactionManagerLookupFactory.GetTransactionManagerLookup( properties );

			// Not ported: useGetGeneratedKeys, useScrollableResultSets

			bool useMinimalPuts = PropertiesHelper.GetBoolean(Environment.UseMinimalPuts, properties, false);
			log.Info("Optimize cache for minimal puts: {0}", useMinimalPuts);

			string releaseModeName = PropertiesHelper.GetString(Environment.ReleaseConnections, properties, "auto");
			log.Info("Connection release mode: {0}", releaseModeName);
			ConnectionReleaseMode releaseMode;
			if ("auto".Equals(releaseModeName))
			{
				releaseMode = ConnectionReleaseMode.AfterTransaction; //transactionFactory.DefaultReleaseMode;
			}
			else
			{
				releaseMode = ConnectionReleaseModeParser.Convert(releaseModeName);
			}
			settings.ConnectionReleaseMode = releaseMode;

			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);
			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			if (defaultSchema != null)
				log.Info("Default schema: {0}", defaultSchema);
			if (defaultCatalog != null)
				log.Info("Default catalog: {0}", defaultCatalog);
			settings.DefaultSchemaName = defaultSchema;
			settings.DefaultCatalogName = defaultCatalog;

			int batchFetchSize = PropertiesHelper.GetInt32(Environment.DefaultBatchFetchSize, properties, 1);
			log.Info("Default batch fetch size: {0}", batchFetchSize);
			settings.DefaultBatchFetchSize = batchFetchSize;

			//Statistics and logging:

			bool showSql = PropertiesHelper.GetBoolean(Environment.ShowSql, properties, false);
			if (showSql)
			{
				log.Info("echoing all SQL to stdout");
			}
			bool formatSql = PropertiesHelper.GetBoolean(Environment.FormatSql, properties);

			bool useStatistics = PropertiesHelper.GetBoolean(Environment.GenerateStatistics, properties);
			log.Info("Statistics: {0}", EnabledDisabled(useStatistics));
			settings.IsStatisticsEnabled = useStatistics;

			bool useIdentifierRollback = PropertiesHelper.GetBoolean(Environment.UseIdentifierRollBack, properties);
			log.Info("Deleted entity synthetic identifier rollback: {0}", EnabledDisabled(useIdentifierRollback));
			settings.IsIdentifierRollbackEnabled = useIdentifierRollback;

			// queries:

			settings.QueryTranslatorFactory = CreateQueryTranslatorFactory(properties);

			settings.LinqQueryProviderType = CreateLinqQueryProviderType(properties);

			IDictionary<string, string> querySubstitutions = PropertiesHelper.ToDictionary(Environment.QuerySubstitutions,
			                                                                               " ,=;:\n\t\r\f", properties);
			if (log.IsInfoEnabled())
			{
				log.Info("Query language substitutions: {0}", CollectionPrinter.ToString((IDictionary) querySubstitutions));
			}

			#region Hbm2DDL
			string autoSchemaExport = PropertiesHelper.GetString(Environment.Hbm2ddlAuto, properties, null);
			if (SchemaAutoAction.Update == autoSchemaExport)
			{
				settings.IsAutoUpdateSchema = true;
			}
			else if (SchemaAutoAction.Create == autoSchemaExport)
			{
				settings.IsAutoCreateSchema = true;
			}
			else if (SchemaAutoAction.Recreate == autoSchemaExport)
			{
				settings.IsAutoCreateSchema = true;
				settings.IsAutoDropSchema = true;
			}
			else if (SchemaAutoAction.Validate == autoSchemaExport)
			{
				settings.IsAutoValidateSchema = true;
			}

			string autoKeyWordsImport = PropertiesHelper.GetString(Environment.Hbm2ddlKeyWords, properties, "not-defined");
			if (autoKeyWordsImport == Hbm2DDLKeyWords.None)
			{
				settings.IsKeywordsImportEnabled = false;
				settings.IsAutoQuoteEnabled = false;
			}
			else if (autoKeyWordsImport == Hbm2DDLKeyWords.Keywords)
			{
				settings.IsKeywordsImportEnabled = true;
			}
			else if (autoKeyWordsImport == Hbm2DDLKeyWords.AutoQuote)
			{
				settings.IsKeywordsImportEnabled = true;
				settings.IsAutoQuoteEnabled = true;
			}
			else if (autoKeyWordsImport == "not-defined")
			{
				settings.IsKeywordsImportEnabled = true;
				settings.IsAutoQuoteEnabled = false;
			}

			#endregion

			bool useSecondLevelCache = PropertiesHelper.GetBoolean(Environment.UseSecondLevelCache, properties, true);
			bool useQueryCache = PropertiesHelper.GetBoolean(Environment.UseQueryCache, properties);

			if (useSecondLevelCache || useQueryCache)
			{
				// The cache provider is needed when we either have second-level cache enabled
				// or query cache enabled.  Note that useSecondLevelCache is enabled by default
				settings.CacheProvider = CreateCacheProvider(properties);
			}
			else
			{
				settings.CacheProvider = new NoCacheProvider();
			}

			string cacheRegionPrefix = PropertiesHelper.GetString(Environment.CacheRegionPrefix, properties, null);
			if (string.IsNullOrEmpty(cacheRegionPrefix)) cacheRegionPrefix = null;
			if (cacheRegionPrefix != null) log.Info("Cache region prefix: {0}", cacheRegionPrefix);


			if (useQueryCache)
			{
				settings.QueryCacheFactory = PropertiesHelper.GetInstance<IQueryCacheFactory>(
					Environment.QueryCacheFactory,
					properties,
					typeof(StandardQueryCacheFactory));
				log.Info("query cache factory: {0}", settings.QueryCacheFactory.GetType().AssemblyQualifiedName);
			}

			string sessionFactoryName = PropertiesHelper.GetString(Environment.SessionFactoryName, properties, null);

			//ADO.NET and connection settings:

			settings.AdoBatchSize = PropertiesHelper.GetInt32(Environment.BatchSize, properties, 0);
			bool orderInserts = PropertiesHelper.GetBoolean(Environment.OrderInserts, properties, (settings.AdoBatchSize > 0));
			log.Info("Order SQL inserts for batching: {0}", EnabledDisabled(orderInserts));
			settings.IsOrderInsertsEnabled = orderInserts;

			bool orderUpdates = PropertiesHelper.GetBoolean(Environment.OrderUpdates, properties, false);
			log.Info("Order SQL updates for batching: {0}", EnabledDisabled(orderUpdates));
			settings.IsOrderUpdatesEnabled = orderUpdates;

			bool wrapResultSets = PropertiesHelper.GetBoolean(Environment.WrapResultSets, properties, false);
			log.Debug("Wrap result sets: {0}", EnabledDisabled(wrapResultSets));
			settings.IsWrapResultSetsEnabled = wrapResultSets;

			bool batchVersionedData = PropertiesHelper.GetBoolean(Environment.BatchVersionedData, properties, false);
			log.Debug("Batch versioned data: {0}", EnabledDisabled(batchVersionedData));
			settings.IsBatchVersionedDataEnabled = batchVersionedData;

			settings.BatcherFactory = CreateBatcherFactory(properties, settings.AdoBatchSize, connectionProvider);

			string isolationString = PropertiesHelper.GetString(Environment.Isolation, properties, String.Empty);
			IsolationLevel isolation = IsolationLevel.Unspecified;
			if (isolationString.Length > 0)
			{
				try
				{
					isolation = (IsolationLevel) Enum.Parse(typeof (IsolationLevel), isolationString);
					log.Info("Using Isolation Level: {0}", isolation);
				}
				catch (ArgumentException ae)
				{
					log.Error(ae, "error configuring IsolationLevel {0}", isolationString);
					throw new HibernateException(
						"The isolation level of " + isolationString + " is not a valid IsolationLevel.  Please "
						+ "use one of the Member Names from the IsolationLevel.", ae);
				}
			}

			//NH-3619
			FlushMode defaultFlushMode = (FlushMode) Enum.Parse(typeof(FlushMode), PropertiesHelper.GetString(Environment.DefaultFlushMode, properties, FlushMode.Auto.ToString()), false);
			log.Info("Default flush mode: {0}", defaultFlushMode);
			settings.DefaultFlushMode = defaultFlushMode;

#pragma warning disable CS0618 // Type or member is obsolete
			var defaultEntityMode = PropertiesHelper.GetString(Environment.DefaultEntityMode, properties, null);
			if (!string.IsNullOrEmpty(defaultEntityMode))
				log.Warn("Default entity-mode setting is deprecated.");
#pragma warning restore CS0618 // Type or member is obsolete

			bool namedQueryChecking = PropertiesHelper.GetBoolean(Environment.QueryStartupChecking, properties, true);
			log.Info("Named query checking : {0}", EnabledDisabled(namedQueryChecking));
			settings.IsNamedQueryStartupCheckingEnabled = namedQueryChecking;
			
			// Not ported - settings.StatementFetchSize = statementFetchSize;
			// Not ported - ScrollableResultSetsEnabled
			// Not ported - GetGeneratedKeysEnabled
			settings.SqlStatementLogger = new SqlStatementLogger(showSql, formatSql);

			settings.ConnectionProvider = connectionProvider;
			settings.QuerySubstitutions = querySubstitutions;
			settings.TransactionFactory = transactionFactory;
			// Not ported - TransactionManagerLookup
			settings.SessionFactoryName = sessionFactoryName;
			settings.MaximumFetchDepth = maxFetchDepth;
			settings.IsQueryCacheEnabled = useQueryCache;
			settings.IsSecondLevelCacheEnabled = useSecondLevelCache;
			settings.CacheRegionPrefix = cacheRegionPrefix;
			settings.IsMinimalPutsEnabled = useMinimalPuts;
			// Not ported - JdbcBatchVersionedData

			settings.QueryModelRewriterFactory = CreateQueryModelRewriterFactory(properties);
			
			// NHibernate-specific:
			settings.IsolationLevel = isolation;
			
			bool trackSessionId = PropertiesHelper.GetBoolean(Environment.TrackSessionId, properties, true);
			log.Debug("Track session id: " + EnabledDisabled(trackSessionId));
			settings.TrackSessionId = trackSessionId;

			return settings;
		}

		private static IBatcherFactory CreateBatcherFactory(IDictionary<string, string> properties, int batchSize, IConnectionProvider connectionProvider)
		{
			System.Type tBatcher = null;
			var batcherClass = PropertiesHelper.GetString(Environment.BatchStrategy, properties, null);
			if (string.IsNullOrEmpty(batcherClass))
			{
				if (batchSize > 0)
				{
					// try to get the BatcherFactory from the Drive if not available use NonBatchingBatcherFactory
					if (connectionProvider.Driver is IEmbeddedBatcherFactoryProvider ebfp && ebfp.BatcherFactoryClass != null)
					{
						tBatcher = ebfp.BatcherFactoryClass;
					}
				}
			}
			else
			{
				tBatcher = ReflectHelper.ClassForName(batcherClass);
			}

			IBatcherFactory instance = null;
			try
			{
				if (tBatcher == null)
				{
					instance = (IBatcherFactory) Environment.ServiceProvider.GetService(typeof(IBatcherFactory));
				}
				if (instance == null)
				{
					instance = (IBatcherFactory) Environment.ServiceProvider.GetMandatoryService(tBatcher ?? typeof(NonBatchingBatcherFactory));
				}
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("Could not instantiate BatcherFactory: " + batcherClass, cnfe);
			}
			log.Info("Batcher factory: {0}", instance.GetType().AssemblyQualifiedName);
			return instance;
		}

		private static string EnabledDisabled(bool value)
		{
			return value ? "enabled" : "disabled";
		}

		private static ICacheProvider CreateCacheProvider(IDictionary<string, string> properties)
		{
			var instance = PropertiesHelper.GetInstance<ICacheProvider>(
				Environment.CacheProvider,
				properties,
				DefaultCacheProvider);
			log.Info("cache provider: {0}", instance.GetType().AssemblyQualifiedName);
			return instance;
		}

		// visibility changed and static modifier added until complete H3.2 porting of SettingsFactory
		private static IQueryTranslatorFactory CreateQueryTranslatorFactory(IDictionary<string, string> properties)
		{
			var instance = PropertiesHelper.GetInstance<IQueryTranslatorFactory>(
				Environment.QueryTranslator,
				properties,
				typeof(Hql.Ast.ANTLR.ASTQueryTranslatorFactory));
			log.Info("Query translator: {0}", instance.GetType().AssemblyQualifiedName);
			return instance;
		}

		private static System.Type CreateLinqQueryProviderType(IDictionary<string, string> properties)
		{
			string className = PropertiesHelper.GetString(
				Environment.QueryLinqProvider, properties, typeof(DefaultQueryProvider).FullName);
			log.Info("Query provider: {0}", className);
			try
			{
				return System.Type.GetType(className, true);
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("could not find query provider class: " + className, cnfe);
			}
		}

		private static ITransactionFactory CreateTransactionFactory(IDictionary<string, string> properties)
		{
			var instance = PropertiesHelper.GetInstance<ITransactionFactory>(
				Environment.TransactionStrategy,
				properties,
				typeof(AdoNetWithSystemTransactionFactory));
			instance.Configure(properties);
			log.Info("Transaction factory: {0}", instance.GetType().AssemblyQualifiedName);
			return instance;
		}

		private static IQueryModelRewriterFactory CreateQueryModelRewriterFactory(IDictionary<string, string> properties)
		{
			var instance = PropertiesHelper.GetInstance<IQueryModelRewriterFactory>(
				Environment.QueryModelRewriterFactory,
				properties,
				null);
			log.Info("Query model rewriter factory factory: '{0}'", instance?.GetType().AssemblyQualifiedName ?? "none");
			return instance;
		}
	}
}
