using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Linq.Functions;
using NHibernate.Transaction;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Settings that affect the behavior of NHibernate at runtime.
	/// </summary>
	public sealed class Settings
	{
		public Settings()
		{
			MaximumFetchDepth = -1;
		}

		// not ported - private TransactionManagerLookup transactionManagerLookup;
		// not ported - private bool strictJPAQLCompliance;

		#region JDBC Specific (Not Ported)

		//private int jdbcFetchSize;
		//private bool isJdbcBatchVersionedData;

		#endregion
		public SqlStatementLogger SqlStatementLogger { get; internal set; }

		public int MaximumFetchDepth { get; internal set; }

		public IDictionary<string, string> QuerySubstitutions { get; internal set; }

		public Dialect.Dialect Dialect { get; internal set; }

		public int AdoBatchSize { get; internal set; }

		public int DefaultBatchFetchSize { get; internal set; }

		public bool IsScrollableResultSetsEnabled { get; internal set; }

		public bool IsGetGeneratedKeysEnabled { get; internal set; }

		public string DefaultSchemaName { get; set; }

		public string DefaultCatalogName { get; internal set; }

		public string SessionFactoryName { get; internal set; }

		public bool IsAutoCreateSchema { get; internal set; }

		public bool IsAutoDropSchema { get; internal set; }

		public bool IsAutoUpdateSchema { get; internal set; }

		public bool IsAutoValidateSchema { get; internal set; }

		public bool IsAutoQuoteEnabled { get; internal set; }

		public bool IsKeywordsImportEnabled { get; internal set; }

		public bool IsQueryCacheEnabled { get; internal set; }

		public bool IsStructuredCacheEntriesEnabled { get; internal set; }

		public bool IsSecondLevelCacheEnabled { get; internal set; }

		public string CacheRegionPrefix { get; internal set; }

		public bool IsMinimalPutsEnabled { get; internal set; }

		public bool IsCommentsEnabled { get; internal set; }

		public bool IsStatisticsEnabled { get; internal set; }

		public bool IsIdentifierRollbackEnabled { get; internal set; }

		public bool IsFlushBeforeCompletionEnabled { get; internal set; }

		public bool IsAutoCloseSessionEnabled { get; internal set; }

		public ConnectionReleaseMode ConnectionReleaseMode { get; internal set; }

		public ICacheProvider CacheProvider { get; internal set; }

		public IQueryCacheFactory QueryCacheFactory { get; internal set; }

		public IConnectionProvider ConnectionProvider { get; internal set; }

		public ITransactionFactory TransactionFactory { get; internal set; }

		public IBatcherFactory BatcherFactory { get; internal set; }

		public IQueryTranslatorFactory QueryTranslatorFactory { get; internal set; }

		public ISQLExceptionConverter SqlExceptionConverter { get; internal set; }

		public bool IsWrapResultSetsEnabled { get; internal set; }

		public bool IsOrderUpdatesEnabled { get; internal set; }

		public bool IsOrderInsertsEnabled { get; internal set; }

		public EntityMode DefaultEntityMode { get; internal set; }

		public bool IsDataDefinitionImplicitCommit { get; internal set; }

		public bool IsDataDefinitionInTransactionSupported { get; internal set; }

		public bool IsNamedQueryStartupCheckingEnabled { get; internal set; }

		#region NH specific

		public IsolationLevel IsolationLevel { get; internal set; }

		public bool IsOuterJoinFetchEnabled { get; internal set; }

		/// <summary>
		/// Get the registry to provide Hql-Generators for known properties/methods.
		/// </summary>
		public ILinqToHqlGeneratorsRegistry LinqToHqlGeneratorsRegistry { get; internal set; }

		[Obsolete("This setting is likely to be removed in a future version of NHibernate. The workaround is to catch all exceptions in the IInterceptor implementation.")]
		public bool IsInterceptorsBeforeTransactionCompletionIgnoreExceptionsEnabled { get; internal set; }

		#endregion
	}
}