using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// NHibernate driver for the SQLite.NET data provider.
	/// <p>
	/// Author: <a href="mailto:ib@stalker.ro"> Ioan Bizau </a>
	/// </p>
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the SQLite.NET.dll Assembly available for NHibernate to load it.
	/// You must also have the SQLite.dll and SQLite3.dll libraries.
	/// </p>
	/// <p>
	/// Please check <a href="http://www.sqlite.org/"> http://www.sqlite.org/ </a> for more information regarding SQLite.
	/// </p>
	/// </remarks>
	public class SQLiteDriver : DriverBase
	{
		System.Type _connectionType;
		System.Type _commandType;

		public SQLiteDriver()
		{
			_connectionType = System.Type.GetType("Finisar.SQLite.SQLiteConnection, SQLite.NET");
			_commandType = System.Type.GetType("Finisar.SQLite.SQLiteCommand, SQLite.NET");
			

			if( _connectionType == null || _commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly SQLite.dll could not be found.  " +
					"Please ensure that the Assembly SQLite.dll is " +
					"in the Global Assembly Cache or in a location that NHibernate " +
					"can use System.Type.GetType(string) to load the types from."
					);
			}
		}

		public override System.Type CommandType
		{
			get { return _commandType; }
		}

		public override System.Type ConnectionType
		{
			get { return _connectionType; }
		}

		public override bool UseNamedPrefixInSql 
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter 
		{
			get { return true; }
		}

		public override string NamedPrefix 	
		{
			get { return "@"; }
		}
	}
}
