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
		private IDictionary _querySubstitutions;
		private Dialect.Dialect _dialect;
		private IsolationLevel _isolationLevel;
		private IConnectionProvider _connectionProvider;
		private ITransactionFactory _transactionFactory;
		private string _sessionFactoryName;
		private ICacheProvider _cacheProvider;
		private string _defaultSchemaName;
		private bool _prepareSql;

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
	}
}