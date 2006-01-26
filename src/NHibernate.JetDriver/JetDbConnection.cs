using System.Data;
using System.Data.OleDb;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// Wrapper class for OleDbConnection to support MS Access dialect in NHibernate.
	/// 
	/// <p>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </p>
	/// </summary>
	public sealed class JetDbConnection : IDbConnection
	{
		private OleDbConnection _connection;

		internal OleDbConnection Connection
		{
			get { return _connection; }
		}

		public JetDbConnection()
		{
			_connection = new OleDbConnection();
		}

		public JetDbConnection( string connectionString )
		{
			_connection = new OleDbConnection( connectionString );
		}

		public JetDbConnection( OleDbConnection connection )
		{
			_connection = connection;
		}

		#region IDbConnection Members

		public void ChangeDatabase( string databaseName )
		{
			Connection.ChangeDatabase( databaseName );
		}

		public IDbTransaction BeginTransaction( IsolationLevel il )
		{
			return new JetDbTransaction( this, Connection.BeginTransaction( il ) );
		}

		IDbTransaction IDbConnection.BeginTransaction()
		{
			return new JetDbTransaction( this, Connection.BeginTransaction() );
		}

		public ConnectionState State
		{
			get { return Connection.State; }
		}

		public string ConnectionString
		{
			get { return Connection.ConnectionString; }
			set { Connection.ConnectionString = value; }
		}

		public IDbCommand CreateCommand()
		{
			return new JetDbCommand( Connection.CreateCommand() );
		}

		public void Open()
		{
			Connection.Open();
		}

		public void Close()
		{
			Connection.Close();
		}

		public string Database
		{
			get { return Connection.Database; }
		}

		public int ConnectionTimeout
		{
			get { return Connection.ConnectionTimeout; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Connection.Dispose();
		}

		#endregion
	}
}