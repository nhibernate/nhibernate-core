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
		private bool _isShowSqlEnabled;
		private bool _isOuterJoinFetchEnabled;
		private int maximumFetchDepth;
		private IDictionary _querySubstitutions;
		private Dialect.Dialect _dialect;
		private int batchSize;
		private bool scrollableResultSetsEnabled;
		private bool getGeneratedKeysEnabled;
		private int statementFetchSize;
		private IsolationLevel _isolationLevel;
		private IConnectionProvider _connectionProvider;
		private ITransactionFactory _transactionFactory;
		private string _sessionFactoryName;
		private bool autoCreateSchema;
		private bool autoDropSchema;
		//private bool autoUpdateSchema;
		private ICacheProvider _cacheProvider;
		private string _defaultSchemaName;
		private bool _prepareSql;
		private int _commandTimeout;

		/// <summary></summary>
		public bool IsShowSqlEnabled
		{
			get { return _isShowSqlEnabled; }
			set { _isShowSqlEnabled = value; }
		}

		/// <summary></summary>
		public bool IsOuterJoinFetchEnabled
		{
			get { return _isOuterJoinFetchEnabled; }
			set { _isOuterJoinFetchEnabled = value; }
		}

		/// <summary></summary>
		public bool IsScrollableResultSetsEnabled
		{
			get { return scrollableResultSetsEnabled; }
			set { scrollableResultSetsEnabled = value; }
		}

		/// <summary></summary>
		public bool IsGetGeneratedKeysEnabled
		{
			get { return getGeneratedKeysEnabled; }
			set { getGeneratedKeysEnabled = value; }
		}

		/// <summary></summary>
		public int BatchSize
		{
			get { return batchSize; }
			set { batchSize = value; }
		}

		/// <summary></summary>
		public int MaximumFetchDepth
		{
			get { return maximumFetchDepth; }
			set { maximumFetchDepth = value; }
		}

		/// <summary></summary>
		public bool IsAutoCreateSchema
		{
			get { return autoCreateSchema; }
			set { autoCreateSchema = value; }
		}

		/// <summary></summary>
		public bool IsAutoDropSchema
		{
			get { return autoDropSchema; }
			set { autoDropSchema = value; }
		}

		/// <summary></summary>
		public bool IsAutoUpdateSchema
		{
			get { return autoCreateSchema; }
			set { autoCreateSchema = value; }
		}

		/// <summary></summary>
		public int StatementFetchSize
		{
			get { return statementFetchSize; }
			set { statementFetchSize = value; }
		}

		/// <summary></summary>
		public IDictionary QuerySubstitutions
		{
			get { return _querySubstitutions; }
			set { _querySubstitutions = value; }
		}

		/// <summary></summary>
		public Dialect.Dialect Dialect
		{
			get { return _dialect; }
			set { _dialect = value; }
		}

		/// <summary></summary>
		public string DefaultSchemaName
		{
			get { return _defaultSchemaName; }
			set { _defaultSchemaName = value; }
		}

		/// <summary></summary>
		public IsolationLevel IsolationLevel
		{
			get { return _isolationLevel; }
			set { _isolationLevel = value; }
		}

		/// <summary></summary>
		public IConnectionProvider ConnectionProvider
		{
			get { return _connectionProvider; }
			set { _connectionProvider = value; }
		}

		/// <summary></summary>
		public ITransactionFactory TransactionFactory
		{
			get { return _transactionFactory; }
			set { _transactionFactory = value; }
		}

		/// <summary></summary>
		public string SessionFactoryName
		{
			get { return _sessionFactoryName; }
			set { _sessionFactoryName = value; }
		}

		/// <summary></summary> 
		public ICacheProvider CacheProvider
		{
			get { return _cacheProvider; }
			set { _cacheProvider = value; }
		}

		/// <summary></summary>
		public bool PrepareSql
		{
			get { return _prepareSql; }
			set { _prepareSql = value; }
		}

		/// <summary></summary>
		public int commandTimeout
		{
			get { return _commandTimeout; }
			set { _commandTimeout = value; }
		}
	}
}