using System;
using System.Collections;
using System.Data;

using NHibernate.Util;

namespace NHibernate.Connection
{
	/// <summary>
	/// The base class for the ConnectionProvider.
	/// </summary>
	public abstract class ConnectionProvider : IConnectionProvider
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionProvider));
		private string connString = null;
		
		public void CloseConnection(IDbConnection conn) 
		{
			log.Debug("Closing connection");
			try 
			{
				conn.Close();
			} 
			catch(Exception e) 
			{
				throw new ADOException("Could not close " + conn.GetType().ToString() + " connection", e);
			}
		}
		public void Configure(IDictionary settings) 
		{
			log.Info("Configuring ConnectionProvider");
			connString = Cfg.Environment.Properties[ Cfg.Environment.ConnectionString ] as string;
			if (connString==null) throw new HibernateException("Could not find connection string setting");
		}

		protected string ConnectionString 
		{
			get { return connString;}
		}

		/// <summary>
		/// Returns a specific ConnectionProvider
		/// </summary>
		/// <returns></returns>
		public abstract IDbConnection GetConnection(); 

		public abstract bool IsStatementCache {get;}

		public abstract bool UseNamedPrefixInSql {get;}

		public abstract bool UseNamedPrefixInParameter {get;}

		public abstract string NamedPrefix 	{get;}

		public string FormatNameForSql(string parameterName) 
		{
			return UseNamedPrefixInSql ? (NamedPrefix + parameterName): StringHelper.SqlParameter;
		}

		public string FormatNameForSql(string tableAlias, string parameterName) 
		{
			
			if(!UseNamedPrefixInSql) return StringHelper.SqlParameter;

			
			if(tableAlias!=null && tableAlias!=String.Empty) {
				return NamedPrefix + tableAlias + parameterName;
			}
			else {
				return NamedPrefix + parameterName;
			}
		}


		public string FormatNameForParameter(string parameterName)
		{
			return UseNamedPrefixInParameter ? (NamedPrefix + parameterName) : parameterName;
		}

		public string FormatNameForParameter(string tableAlias, string parameterName) 
		{
			
			if(!UseNamedPrefixInParameter) return parameterName;

			
			if(tableAlias!=null && tableAlias!=String.Empty) 
			{
				return NamedPrefix + tableAlias + parameterName;
			}
			else 
			{
				return NamedPrefix + parameterName;
			}
		}

		public abstract IDbCommand CreateCommand();
	}
}
