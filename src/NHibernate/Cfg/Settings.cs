using System;
using System.Collections;
using System.Data;

using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Dialect;
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
		
		public bool IsShowSqlEnabled
		{
			get { return _isShowSqlEnabled; }
			set { _isShowSqlEnabled = value; }
		}

		public bool IsOuterJoinFetchEnabled
		{
			get { return _isOuterJoinFetchEnabled; }
			set { _isOuterJoinFetchEnabled = value; }
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

		public bool PrepareSql
		{
			get { return _prepareSql; }
			set { _prepareSql = value; }
		}
	}
}
