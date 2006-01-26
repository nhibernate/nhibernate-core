using System;
using System.Data;
using System.Data.OleDb;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// Summary description for JetDbTransaction.
	/// 
	/// <p>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </p>
	/// </summary>
	public sealed class JetDbTransaction : IDbTransaction
	{
		private OleDbTransaction _transaction;
		private JetDbConnection _connection;

		internal OleDbTransaction Transaction
		{
			get { return _transaction; }
		}

		internal JetDbTransaction( JetDbConnection connection, OleDbTransaction transaction )
		{
			_connection = connection;
			_transaction = transaction;
		}

		#region IDbTransaction Members

		public void Rollback()
		{
			Transaction.Rollback();
		}

		public void Commit()
		{
			Transaction.Commit();
		}

		public IDbConnection Connection
		{
			get { return _connection; }
		}

		public IsolationLevel IsolationLevel
		{
			get { return Transaction.IsolationLevel; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			( _transaction as IDisposable ).Dispose();
		}

		#endregion
	}
}