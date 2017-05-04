using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Driver
{
	/// <summary>
	/// NHibernate driver for the System.Data.SQLite data provider for .NET 2.0.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this driver you must have the System.Data.SQLite.dll assembly available
	/// for NHibernate to load. This assembly includes the SQLite.dll or SQLite3.dll libraries.
	/// </p>    
	/// <p>
	/// You can get the System.Data.SQLite.dll assembly from http://sourceforge.net/projects/sqlite-dotnet2.
	/// </p>
	/// <p>
	/// Please check <a href="http://www.sqlite.org/">http://www.sqlite.org/</a> for more information regarding SQLite.
	/// </p>
	/// </remarks>
	public partial class SQLite20Driver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SQLite20Driver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>SQLite.NET</c> assembly can not be loaded.
		/// </exception>
		public SQLite20Driver() : base(
			"System.Data.SQLite",
			"System.Data.SQLite",
			"System.Data.SQLite.SQLiteConnection",
			"System.Data.SQLite.SQLiteCommand")
		{
		}

        public override DbConnection CreateConnection()
        {
            var connection = base.CreateConnection();
            connection.StateChange += Connection_StateChange;
            return connection;
        }

        private static void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if ((e.OriginalState == ConnectionState.Broken || e.OriginalState == ConnectionState.Closed || e.OriginalState == ConnectionState.Connecting) &&
                e.CurrentState == ConnectionState.Open)
            {
                var connection = (DbConnection)sender;
                using (var command = connection.CreateCommand())
                {
                    // Activated foreign keys if supported by SQLite.  Unknown pragmas are ignored.
                    command.CommandText = "PRAGMA foreign_keys = ON";
                    command.ExecuteNonQuery();
                }
            }
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

		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool SupportsMultipleQueries
		{
			get { return true; }
		}
	}
}