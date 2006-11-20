using System;
using System.Data;
using System.Data.OleDb;

namespace NHibernate.JetDriver
{
	/// <summary>
	/// JetDbCommand is just a wrapper class for OleDbCommand with special handling of datatypes needed when storing
	/// data into the Access database.
	/// These type conversion are performed in command parameters:
	/// 1) DateTime, Time and Date parameters are converted to string using 'dd-MMM-yyyy HH:mm:ss' format.
	/// 2) Int64 parameter is converted to Int32, possibly throwing an exception.
	/// 
	/// Because of the diference between the way how NHibernate defines identity columns and how Access does, I have to
	/// incorporate another dirty hack here. Because NHibernate does not always use its driver to generate commands 
	/// (ie. in Schema creation classes), this functionality has to be moved "down" to the IDbCommand object. 
	/// IMO, everything in NHibernate should call db queries using its drivers, but at the present time it is not true.
	/// If it was, we could move the replacing functionality up to the driver class, where it's more appropriate, although it
	/// is still a dirty hack :)
	/// 
	/// <p>
	/// Author: <a href="mailto:lukask@welldatatech.com">Lukas Krejci</a>
	/// </p>
	/// </summary>
	public sealed class JetDbCommand : IDbCommand
	{
		private OleDbCommand _command;
		private JetDbConnection _connection;
		private JetDbTransaction _transaction;

		public static string IdentitySpecPlaceHolder
		{
			get { return "!!! REPLACE THE PRECEEDING 'INT' AND THIS PLACEHOLDER WITH 'COUNTER' !!!"; }
		}

		internal OleDbCommand Command
		{
			get { return _command; }
		}

		public JetDbCommand()
		{
			_command = new OleDbCommand();
		}

		public JetDbCommand( string cmdText, OleDbConnection connection, OleDbTransaction transaction )
		{
			_command = new OleDbCommand( cmdText, connection, transaction );
		}

		public JetDbCommand( string cmdText, OleDbConnection connection )
		{
			_command = new OleDbCommand( cmdText, connection );
		}

		public JetDbCommand( string cmdText )
		{
			_command = new OleDbCommand( cmdText );
		}

		internal JetDbCommand( OleDbCommand command )
		{
			_command = command;
		}

		/// <summary>
		/// So far, the only data type I know about that causes Access to fail everytime is DateTime.
		/// The solution to the problem is to convert the date to a string representing it.
		/// </summary>
		private void CheckParameters()
		{
			if( Command.Parameters == null || Command.Parameters.Count == 0 ) return;

			foreach( IDataParameter p in Command.Parameters )
			{
				object newVal;
				switch( p.DbType )
				{
					case DbType.DateTime:
					case DbType.Time:
					case DbType.Date:
						p.DbType = DbType.String;

						if( p.Value != DBNull.Value )
						{
							newVal = ( ( DateTime ) p.Value ).ToString( "dd-MMM-yyyy HH:mm:ss" );
							p.Value = newVal;
						}
						break;
					case DbType.Int64:
						p.DbType = DbType.Int32;
						if( p.Value != DBNull.Value )
						{
							newVal = Convert.ToInt32( ( long ) p.Value );
							p.Value = newVal;
						}
						break;
				}
			}
		}

		#region IDbCommand Members

		public void Cancel()
		{
			Command.Cancel();
		}

		public void Prepare()
		{
			Command.Prepare();
		}

		public CommandType CommandType
		{
			get { return Command.CommandType; }
			set { Command.CommandType = value; }
		}

		public IDataReader ExecuteReader( CommandBehavior behavior )
		{
			CheckParameters();
			return Command.ExecuteReader( behavior );
		}

		IDataReader IDbCommand.ExecuteReader()
		{
			CheckParameters();
			return Command.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			CheckParameters();
			return Command.ExecuteScalar();
		}

		public int ExecuteNonQuery()
		{
			CheckParameters();
			Command.CommandText = Command.CommandText.Replace( "INT " + IdentitySpecPlaceHolder, "COUNTER" );
			return Command.ExecuteNonQuery();
		}

		public int CommandTimeout
		{
			get { return Command.CommandTimeout; }
			set { Command.CommandTimeout = value; }
		}

		public IDbDataParameter CreateParameter()
		{
			return Command.CreateParameter();
		}

		public IDbConnection Connection
		{
			get { return _connection; }
			set
			{
				_connection = ( JetDbConnection ) value;
				Command.Connection = _connection.Connection;
			}
		}

		public UpdateRowSource UpdatedRowSource
		{
			get { return Command.UpdatedRowSource; }
			set { Command.UpdatedRowSource = value; }
		}

		public string CommandText
		{
			get { return Command.CommandText; }
			set { Command.CommandText = value; }
		}

		public IDataParameterCollection Parameters
		{
			get { return Command.Parameters; }
		}

		public IDbTransaction Transaction
		{
			get { return _transaction; }
			set
			{
				if (value == null)
				{
					_transaction = null;
					Command.Transaction = null;
				}
				else
				{
					_transaction = ( JetDbTransaction ) value;
					Command.Transaction = _transaction.Transaction;
				}
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Command.Dispose();
		}

		#endregion
	}
}