using System.Collections;
using System.Data;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Transaction;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Settings that affect the behavior of NHibernate at runtime.
	/// </summary>
	public sealed class Settings
	{
		private bool _showSql;
		private bool _outerJoinFetchEnabled;
		private int maximumFetchDepth;
		private IDictionary _querySubstitutions;
		private Dialect.Dialect _dialect;
		private int batchSize;
		private bool scrollableResultSetsEnabled;
		private bool getGeneratedKeysEnabled;
		private string _defaultSchemaName;
		private int statementFetchSize;
		private IConnectionProvider _connectionProvider;
		private ITransactionFactory _transactionFactory;
		// not ported - TransactionManagerLookup
		private string _sessionFactoryName;
		private bool autoCreateSchema;
		private bool autoDropSchema;
		//private bool autoUpdateSchema;
		private ICacheProvider _cacheProvider;
		private bool _queryCacheEnabled;
		private IQueryCacheFactory _queryCacheFactory;
		private bool _minimalPutsEnabled;
		// not ported - private bool _jdbcBatchVersionedData
		// TODO: private bool _sqlExceptionConverter;
		// TODO: private bool _wrapDataReadersEnabled;
		
		// New in NH:
		private IsolationLevel _isolationLevel;
		private bool _prepareSql;
		private int _commandTimeout;

		public bool IsShowSqlEnabled
		{
			get { return _showSql; }
			set { _showSql = value; }
		}

		public bool IsOuterJoinFetchEnabled
		{
			get { return _outerJoinFetchEnabled; }
			set { _outerJoinFetchEnabled = value; }
		}

		public bool IsScrollableResultSetsEnabled
		{
			get { return scrollableResultSetsEnabled; }
			set { scrollableResultSetsEnabled = value; }
		}

		public bool IsGetGeneratedKeysEnabled
		{
			get { return getGeneratedKeysEnabled; }
			set { getGeneratedKeysEnabled = value; }
		}

		public bool IsMinimalPutsEnabled
		{
			get { return _minimalPutsEnabled; }
			set { _minimalPutsEnabled = value; }
		}

		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		public int MaximumFetchDepth
		{
			get { return maximumFetchDepth; }
			set { maximumFetchDepth = value; }
		}

		public bool IsAutoCreateSchema
		{
			get { return autoCreateSchema; }
			set { autoCreateSchema = value; }
		}

		public bool IsAutoDropSchema
		{
			get { return autoDropSchema; }
			set { autoDropSchema = value; }
		}

		public bool IsAutoUpdateSchema
		{
			get { return autoCreateSchema; }
			set { autoCreateSchema = value; }
		}

		public int StatementFetchSize
		{
			get { return statementFetchSize; }
			set { statementFetchSize = value; }
		}

		public IDictionary QuerySubstitutions
		{
			get { return _querySubstitutions; }
			set { _querySubstitutions = value; }
		}

		public Dialect.Dialect Dialect
		{
			get { return _dialect; }
			set { _dialect = value; }
		}

		public string DefaultSchemaName
		{
			get { return _defaultSchemaName; }
			set { _defaultSchemaName = value; }
		}

		public IsolationLevel IsolationLevel
		{
			get { return _isolationLevel; }
			set { _isolationLevel = value; }
		}

		public IConnectionProvider ConnectionProvider
		{
			get { return _connectionProvider; }
			set { _connectionProvider = value; }
		}

		public ITransactionFactory TransactionFactory
		{
			get { return _transactionFactory; }
			set { _transactionFactory = value; }
		}

		public string SessionFactoryName
		{
			get { return _sessionFactoryName; }
			set { _sessionFactoryName = value; }
		}

		public ICacheProvider CacheProvider
		{
			get { return _cacheProvider; }
			set { _cacheProvider = value; }
		}

		public bool IsQueryCacheEnabled
		{
			get { return _queryCacheEnabled; }
			set { _queryCacheEnabled = value; }
		}

		public IQueryCacheFactory QueryCacheFactory
		{
			get { return _queryCacheFactory; }
			set { _queryCacheFactory = value; }
		}
	}
}