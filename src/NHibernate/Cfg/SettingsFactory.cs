using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Transaction;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Reads configuration properties and configures a <see cref="Settings"/> instance. 
	/// </summary>
	public sealed class SettingsFactory
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SettingsFactory));
		private static readonly string DefaultCacheProvider = typeof(NoCacheProvider).AssemblyQualifiedName;

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
				log.Warn("No dialect set - using GenericDialect: " + he.Message);
				dialect = new GenericDialect();
			}
			settings.Dialect = dialect;

			#region SQL Exception converter

			ISQLExceptionConverter sqlExceptionConverter;
			try
			{
				sqlExceptionConverter = SQLExceptionConverterFactory.BuildSQLExceptionConverter(dialect, properties);
			}
			catch (HibernateException)
			{
				log.Warn("Error building SQLExceptionConverter; using minimal converter");
				sqlExceptionConverter = SQLExceptionConverterFactory.BuildMinimalSQLExceptionConverter();
			}
			settings.SqlExceptionConverter = sqlExceptionConverter;

			#endregion

			int maxFetchDepth = PropertiesHelper.GetInt32(Environment.MaxFetchDepth, properties, -1);
			if (maxFetchDepth != -1)
			{
				log.Info("Maximum outer join fetch depth: " + maxFetchDepth);
			}

			IConnectionProvider connectionProvider = ConnectionProviderFactory.NewConnectionProvider(properties);
			ITransactionFactory transactionFactory = CreateTransactionFactory(properties);
			// TransactionManagerLookup transactionManagerLookup = TransactionManagerLookupFactory.GetTransactionManagerLookup( properties );

			// Not ported: useGetGeneratedKeys, useScrollableResultSets

			bool useMinimalPuts = PropertiesHelper.GetBoolean(Environment.UseMinimalPuts, properties, false);
			log.Info("Optimize cache for minimal puts: " + useMinimalPuts);

			string releaseModeName = PropertiesHelper.GetString(Environment.ReleaseConnections, properties, "auto");
			log.Info("Connection release mode: " + releaseModeName);
			ConnectionReleaseMode releaseMode;
			if ("auto".Equals(releaseModeName))
			{
				releaseMode = ConnectionReleaseMode.AfterTransaction; //transactionFactory.DefaultReleaseMode;
			}
			else
			{
				releaseMode = ParseConnectionReleaseMode(releaseModeName);
			}
			settings.ConnectionReleaseMode = releaseMode;

			string defaultSchema = PropertiesHelper.GetString(Environment.DefaultSchema, properties, null);
			string defaultCatalog = PropertiesHelper.GetString(Environment.DefaultCatalog, properties, null);
			if (defaultSchema != null)
				log.Info("Default schema: " + defaultSchema);
			if (defaultCatalog != null)
				log.Info("Default catalog: " + defaultCatalog);
			settings.DefaultSchemaName=defaultSchema;
			settings.DefaultCatalogName=defaultCatalog;

			int batchFetchSize = PropertiesHelper.GetInt32(Environment.DefaultBatchFetchSize, properties, 1);
			log.Info("Default batch fetch size: " + batchFetchSize);
			settings.DefaultBatchFetchSize= batchFetchSize;

			//Statistics and logging:

			bool showSql = PropertiesHelper.GetBoolean(Environment.ShowSql, properties, false);
			if (showSql)
			{
				log.Info("echoing all SQL to stdout");
			}

			bool useStatistics = PropertiesHelper.GetBoolean(Environment.GenerateStatistics, properties);
			log.Info("Statistics: " + EnabledDisabled(useStatistics));
			settings.IsStatisticsEnabled = useStatistics;

			// queries:

			settings.QueryTranslatorFactory = CreateQueryTranslatorFactory(properties);

			IDictionary<string, string> querySubstitutions =
				PropertiesHelper.ToDictionary(Environment.QuerySubstitutions, " ,=;:\n\t\r\f", properties);
			if (log.IsInfoEnabled)
			{
				log.Info("Query language substitutions: " + CollectionPrinter.ToString((IDictionary)querySubstitutions));
			}

			string autoSchemaExport = PropertiesHelper.GetString(Environment.Hbm2ddlAuto, properties, null);
			if ("update" == autoSchemaExport)
			{
				settings.IsAutoUpdateSchema = true;
			}
			if ("create" == autoSchemaExport)
			{
				settings.IsAutoCreateSchema = true;
			}
			if ("create-drop" == autoSchemaExport)
			{
				settings.IsAutoCreateSchema = true;
				settings.IsAutoDropSchema = true;
			}

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
			if (cacheRegionPrefix != null) log.Info("Cache region prefix: " + cacheRegionPrefix);


			if (useQueryCache)
			{
				string queryCacheFactoryClassName =
					PropertiesHelper.GetString(Environment.QueryCacheFactory, properties, typeof(StandardQueryCacheFactory).FullName);
				log.Info("query cache factory: " + queryCacheFactoryClassName);
				try
				{
					settings.QueryCacheFactory = (IQueryCacheFactory) Activator.CreateInstance(
					                                                  	ReflectHelper.ClassForName(queryCacheFactoryClassName));
				}
				catch (Exception cnfe)
				{
					throw new HibernateException("could not instantiate IQueryCacheFactory: " + queryCacheFactoryClassName, cnfe);
				}
			}

			string sessionFactoryName = PropertiesHelper.GetString(Environment.SessionFactoryName, properties, null);

			//ADO.NET and connection settings:

			// TODO: Environment.BatchVersionedData
			settings.AdoBatchSize = PropertiesHelper.GetInt32(Environment.BatchSize, properties, 0);
			bool wrapResultSets = PropertiesHelper.GetBoolean(Environment.WrapResultSets, properties, false);
			log.Debug("Wrap result sets: " + EnabledDisabled(wrapResultSets));
			settings.IsWrapResultSetsEnabled = wrapResultSets;
			settings.BatcherFactory = CreateBatcherFactory(properties, settings.AdoBatchSize, connectionProvider);

			string isolationString = PropertiesHelper.GetString(Environment.Isolation, properties, String.Empty);
			IsolationLevel isolation = IsolationLevel.Unspecified;
			if (isolationString.Length > 0)
			{
				try
				{
					isolation = (IsolationLevel) Enum.Parse(typeof(IsolationLevel), isolationString);
					log.Info("Using Isolation Level: " + isolation);
				}
				catch (ArgumentException ae)
				{
					log.Error("error configuring IsolationLevel " + isolationString, ae);
					throw new HibernateException(
						"The isolation level of " + isolationString + " is not a valid IsolationLevel.  Please " +
						"use one of the Member Names from the IsolationLevel.", ae);
				}
			}

			bool namedQueryChecking = PropertiesHelper.GetBoolean(Environment.QueryStartupChecking, properties, true);
			log.Info("Named query checking : " + EnabledDisabled(namedQueryChecking));
			settings.IsNamedQueryStartupCheckingEnabled = namedQueryChecking;

			// Not ported - settings.StatementFetchSize = statementFetchSize;
			// Not ported - ScrollableResultSetsEnabled
			// Not ported - GetGeneratedKeysEnabled
			settings.IsShowSqlEnabled = showSql;
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

			// NHibernate-specific:
			settings.IsolationLevel = isolation;

			return settings;
		}

		private static IBatcherFactory CreateBatcherFactory(IDictionary<string, string> properties, int batchSize, IConnectionProvider connectionProvider)
		{
			System.Type tBatcher = typeof (NonBatchingBatcherFactory);
			string batcherClass = PropertiesHelper.GetString(Environment.BatchStrategy, properties, null);
			if (string.IsNullOrEmpty(batcherClass))
			{
				if (batchSize > 0)
				{
					// try to get the BatcherFactory from the Drive if not available use NonBatchingBatcherFactory
					IEmbeddedBatcherFactoryProvider ebfp = connectionProvider.Driver as IEmbeddedBatcherFactoryProvider;
					if (ebfp != null && ebfp.BatcherFactoryClass != null)
						tBatcher = ebfp.BatcherFactoryClass;
				}
			}
			else
			{
				tBatcher = ReflectHelper.ClassForName(batcherClass);
			}
			log.Info("Batcher factory: " + tBatcher.AssemblyQualifiedName);
			try
			{
				return (IBatcherFactory) Activator.CreateInstance(tBatcher);
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("Could not instantiate BatcherFactory: " + batcherClass, cnfe);
			}
		}

		private static string EnabledDisabled(bool value)
		{
			return value ? "enabled" : "disabled";
		}

		private static ICacheProvider CreateCacheProvider(IDictionary<string, string> properties)
		{
			string cacheClassName = PropertiesHelper.GetString(Environment.CacheProvider, properties, DefaultCacheProvider);
			log.Info("cache provider: " + cacheClassName);
			try
			{
				return (ICacheProvider) Activator.CreateInstance(ReflectHelper.ClassForName(cacheClassName));
			}
			catch (Exception e)
			{
				throw new HibernateException("could not instantiate CacheProvider: " + cacheClassName, e);
			}
		}

		internal SettingsFactory()
		{
			//should not be publically creatable
		}

		private static ConnectionReleaseMode ParseConnectionReleaseMode(string name)
		{
			switch (name)
			{
				case "after_statement":
					throw new HibernateException("aggressive connection release (after_statement) not supported by NHibernate");
				case "after_transaction":
					return ConnectionReleaseMode.AfterTransaction;
				case "on_close":
					return ConnectionReleaseMode.OnClose;
				default:
					throw new HibernateException("could not determine appropriate connection release mode [" + name + "]");
			}
		}

		// visibility changed and static modifier added until complete H3.2 porting of SettingsFactory
		private static IQueryTranslatorFactory CreateQueryTranslatorFactory(IDictionary<string, string> properties)
		{
			string className = PropertiesHelper.GetString(
				Environment.QueryTranslator, properties, typeof(Hql.Classic.ClassicQueryTranslatorFactory).FullName);
			log.Info("Query translator: " + className);
			try
			{
				return (IQueryTranslatorFactory) Activator.CreateInstance(ReflectHelper.ClassForName(className));
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("could not instantiate QueryTranslatorFactory: " + className, cnfe);
			}
		}

		private static ITransactionFactory CreateTransactionFactory(IDictionary<string, string> properties)
		{
			string className = PropertiesHelper.GetString(
				Environment.TransactionStrategy, properties, typeof(AdoNetTransactionFactory).FullName);
			log.Info("Transaction factory: " + className);

			try
			{
				return (ITransactionFactory)Activator.CreateInstance(ReflectHelper.ClassForName(className));
			}
			catch (Exception cnfe)
			{
				throw new HibernateException("could not instantiate TransactionFactory: " + className, cnfe);
			}
		}
	}
}
