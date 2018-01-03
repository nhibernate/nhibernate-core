#if !NETSTANDARD2_0 || DRIVER_PACKAGE
using System;
using System.Data;
using System.Data.Common;

namespace NHibernate.Driver
{
	/// <summary>
	/// NHibernate driver for the System.Data.SQLite data provider for .NET.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to use this driver you must have the System.Data.SQLite.dll assembly available
	/// for NHibernate to load. This assembly includes the SQLite.dll or SQLite3.dll libraries.
	/// </para>
	/// <para>
	/// You can get the System.Data.SQLite.dll assembly from
	/// <a href="https://system.data.sqlite.org/">https://system.data.sqlite.org/</a>
	/// </para>
	/// <para>
	/// Please check <a href="https://www.sqlite.org/">https://www.sqlite.org/</a> for more information regarding SQLite.
	/// </para>
	/// </remarks>
#if DRIVER_PACKAGE
	public class SQLiteDriver : DriverBase
#else
	[Obsolete("Use NHibernate.Driver.SQlLite NuGet package and SQLiteDriver."
		+ "  There are also Loquacious configuration points: .Connection.BySQLiteDriver() and .DataBaseIntegration(x => x.SQLiteDriver()).")]
	public class SQLite20Driver : ReflectionBasedDriver
#endif
	{
#if DRIVER_PACKAGE
		public SQLiteDriver()
		{}
#else
		/// <summary>
		/// Initializes a new instance of <see cref="SQLite20Driver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>SQLite.NET</c> assembly can not be loaded.
		/// </exception>
		public SQLite20Driver() 
			: base(
				"System.Data.SQLite",
				"System.Data.SQLite",
				"System.Data.SQLite.SQLiteConnection",
				"System.Data.SQLite.SQLiteCommand")
		{
		}
#endif

#if DRIVER_PACKAGE
		public override DbConnection CreateConnection()
		{
			var connection = new System.Data.SQLite.SQLiteConnection();
			connection.StateChange += Connection_StateChange;
			return connection;
		}

		public override DbCommand CreateCommand()
		{
			return new System.Data.SQLite.SQLiteCommand();
		}
#else
		public override DbConnection CreateConnection()
		{
			var connection = base.CreateConnection();
			connection.StateChange += Connection_StateChange;
			return connection;
		}
#endif

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

		public override IResultSetsCommand GetResultSetsCommand(Engine.ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool UseNamedPrefixInSql => true;

		public override bool UseNamedPrefixInParameter => true;

		public override string NamedPrefix => "@";

		public override bool SupportsMultipleOpenReaders => false;

		public override bool SupportsMultipleQueries => true;

		public override bool SupportsNullEnlistment => false;

		public override bool HasDelayedDistributedTransactionCompletion => true;
	}
}
#endif
