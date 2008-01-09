using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Transaction;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Settings that affect the behavior of NHibernate at runtime.
	/// </summary>
	public sealed class Settings
	{
		private bool isShowSqlEnabled;
		private bool isFormatSqlEnabled;
		private int maximumFetchDepth;
		private IDictionary<string, string> querySubstitutions;
		private Dialect.Dialect dialect;
		private int adoBatchSize;
		private int defaultBatchFetchSize;
		private bool isScrollableResultSetsEnabled;
		private bool isGetGeneratedKeysEnabled;
		private string defaultSchemaName;
		private string defaultCatalogName;
		private string sessionFactoryName;
		private bool isAutoCreateSchema;
		private bool isAutoDropSchema;
		private bool isAutoUpdateSchema;
		private bool isAutoValidateSchema;
		private bool isQueryCacheEnabled;
		private bool isStructuredCacheEntriesEnabled;
		private bool isSecondLevelCacheEnabled;
		private string cacheRegionPrefix;
		private bool isMinimalPutsEnabled;
		private bool isCommentsEnabled;
		private bool isStatisticsEnabled;
		private bool isIdentifierRollbackEnabled;
		private bool isFlushBeforeCompletionEnabled;
		private bool isAutoCloseSessionEnabled;
		private ConnectionReleaseMode connectionReleaseMode;
		private ICacheProvider cacheProvider;
		private IQueryCacheFactory queryCacheFactory;
		private IConnectionProvider connectionProvider;
		private ITransactionFactory transactionFactory;
		// not ported - private TransactionManagerLookup transactionManagerLookup;
		private IBatcherFactory batcherFactory;
		private IQueryTranslatorFactory queryTranslatorFactory;
		private ISQLExceptionConverter sqlExceptionConverter;
		private bool isWrapResultSetsEnabled;
		private bool isOrderUpdatesEnabled;
		private bool isOrderInsertsEnabled;
		private EntityMode defaultEntityMode;
		private bool isDataDefinitionImplicitCommit;
		private bool isDataDefinitionInTransactionSupported;
		// not ported - private bool strictJPAQLCompliance;
		private bool isNamedQueryStartupCheckingEnabled;

		#region JDBC Specific (Not Ported)
		//private int jdbcFetchSize;
		//private bool isJdbcBatchVersionedData;
		#endregion

		#region NH specific

		private IsolationLevel isolationLevel;
		private bool isOuterJoinFetchEnabled;

		#endregion

		public bool IsShowSqlEnabled
		{
			get { return isShowSqlEnabled; }
			internal set { isShowSqlEnabled = value; }
		}

		public bool IsFormatSqlEnabled
		{
			get { return isFormatSqlEnabled; }
			internal set { isFormatSqlEnabled = value; }
		}

		public int MaximumFetchDepth
		{
			get { return maximumFetchDepth; }
			internal set { maximumFetchDepth = value; }
		}

		public IDictionary<string,string> QuerySubstitutions
		{
			get { return querySubstitutions; }
			internal set { querySubstitutions = value; }
		}

		public Dialect.Dialect Dialect
		{
			get { return dialect; }
			internal set { dialect = value; }
		}

		public int AdoBatchSize
		{
			get { return adoBatchSize; }
			internal set { adoBatchSize = value; }
		}

		public int DefaultBatchFetchSize
		{
			get { return defaultBatchFetchSize; }
			internal set { defaultBatchFetchSize = value; }
		}

		public bool IsScrollableResultSetsEnabled
		{
			get { return isScrollableResultSetsEnabled; }
			internal set { isScrollableResultSetsEnabled = value; }
		}

		public bool IsGetGeneratedKeysEnabled
		{
			get { return isGetGeneratedKeysEnabled; }
			internal set { isGetGeneratedKeysEnabled = value; }
		}

		public string DefaultSchemaName
		{
			get { return defaultSchemaName; }
			set { defaultSchemaName = value; }
		}

		public string DefaultCatalogName
		{
			get { return defaultCatalogName; }
			internal set { defaultCatalogName = value; }
		}

		public string SessionFactoryName
		{
			get { return sessionFactoryName; }
			internal set { sessionFactoryName = value; }
		}

		public bool IsAutoCreateSchema
		{
			get { return isAutoCreateSchema; }
			internal set { isAutoCreateSchema = value; }
		}

		public bool IsAutoDropSchema
		{
			get { return isAutoDropSchema; }
			internal set { isAutoDropSchema = value; }
		}

		public bool IsAutoUpdateSchema
		{
			get { return isAutoUpdateSchema; }
			internal set { isAutoUpdateSchema = value; }
		}

		public bool IsAutoValidateSchema
		{
			get { return isAutoValidateSchema; }
			internal set { isAutoValidateSchema = value; }
		}

		public bool IsQueryCacheEnabled
		{
			get { return isQueryCacheEnabled; }
			internal set { isQueryCacheEnabled = value; }
		}

		public bool IsStructuredCacheEntriesEnabled
		{
			get { return isStructuredCacheEntriesEnabled; }
			internal set { isStructuredCacheEntriesEnabled = value; }
		}

		public bool IsSecondLevelCacheEnabled
		{
			get { return isSecondLevelCacheEnabled; }
			internal set { isSecondLevelCacheEnabled = value; }
		}

		public string CacheRegionPrefix
		{
			get { return cacheRegionPrefix; }
			internal set { cacheRegionPrefix = value; }
		}

		public bool IsMinimalPutsEnabled
		{
			get { return isMinimalPutsEnabled; }
			internal set { isMinimalPutsEnabled = value; }
		}

		public bool IsCommentsEnabled
		{
			get { return isCommentsEnabled; }
			internal set { isCommentsEnabled = value; }
		}

		public bool IsStatisticsEnabled
		{
			get { return isStatisticsEnabled; }
			internal set { isStatisticsEnabled = value; }
		}

		public bool IsIdentifierRollbackEnabled
		{
			get { return isIdentifierRollbackEnabled; }
			internal set { isIdentifierRollbackEnabled = value; }
		}

		public bool IsFlushBeforeCompletionEnabled
		{
			get { return isFlushBeforeCompletionEnabled; }
			internal set { isFlushBeforeCompletionEnabled = value; }
		}

		public bool IsAutoCloseSessionEnabled
		{
			get { return isAutoCloseSessionEnabled; }
			internal set { isAutoCloseSessionEnabled = value; }
		}

		public ConnectionReleaseMode ConnectionReleaseMode
		{
			get { return connectionReleaseMode; }
			internal set { connectionReleaseMode = value; }
		}

		public ICacheProvider CacheProvider
		{
			get { return cacheProvider; }
			internal set { cacheProvider = value; }
		}

		public IQueryCacheFactory QueryCacheFactory
		{
			get { return queryCacheFactory; }
			internal set { queryCacheFactory = value; }
		}

		public IConnectionProvider ConnectionProvider
		{
			get { return connectionProvider; }
			internal set { connectionProvider = value; }
		}

		public ITransactionFactory TransactionFactory
		{
			get { return transactionFactory; }
			internal set { transactionFactory = value; }
		}

		public IBatcherFactory BatcherFactory
		{
			get { return batcherFactory; }
			internal set { batcherFactory = value; }
		}

		public IQueryTranslatorFactory QueryTranslatorFactory
		{
			get { return queryTranslatorFactory; }
			internal set { queryTranslatorFactory = value; }
		}

		public ISQLExceptionConverter SqlExceptionConverter
		{
			get { return sqlExceptionConverter; }
			internal set { sqlExceptionConverter = value; }
		}

		public bool IsWrapResultSetsEnabled
		{
			get { return isWrapResultSetsEnabled; }
			internal set { isWrapResultSetsEnabled = value; }
		}

		public bool IsOrderUpdatesEnabled
		{
			get { return isOrderUpdatesEnabled; }
			internal set { isOrderUpdatesEnabled = value; }
		}

		public bool IsOrderInsertsEnabled
		{
			get { return isOrderInsertsEnabled; }
			internal set { isOrderInsertsEnabled = value; }
		}

		public EntityMode DefaultEntityMode
		{
			get { return defaultEntityMode; }
			internal set { defaultEntityMode = value; }
		}

		public bool IsDataDefinitionImplicitCommit
		{
			get { return isDataDefinitionImplicitCommit; }
			internal set { isDataDefinitionImplicitCommit = value; }
		}

		public bool IsDataDefinitionInTransactionSupported
		{
			get { return isDataDefinitionInTransactionSupported; }
			internal set { isDataDefinitionInTransactionSupported = value; }
		}

		public bool IsNamedQueryStartupCheckingEnabled
		{
			get { return isNamedQueryStartupCheckingEnabled; }
			internal set { isNamedQueryStartupCheckingEnabled = value; }
		}

		#region NH specific

		public IsolationLevel IsolationLevel
		{
			get { return isolationLevel; }
			internal set { isolationLevel = value; }
		}

		public bool IsOuterJoinFetchEnabled
		{
			get { return isOuterJoinFetchEnabled; }
			internal set { isOuterJoinFetchEnabled = value; }
		}

		#endregion
	}
}