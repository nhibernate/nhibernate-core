using System;
using System.Data;
using System.Reflection;

namespace NHibernate.Driver
{
	/// <summary>
	/// The PostgreSQL data provider provides a database driver for PostgreSQL.
	/// <p>
	/// Author: <a href="mailto:oliver@weichhold.com">Oliver Weichhold</a>
	/// </p>
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the Npgsql.dll Assembly available for 
	/// NHibernate to load it.
	/// </p>
	/// <p>
	/// Please check the products website <a href="http://www.postgresql.org/">http://www.postgresql.org/</a>
	/// for any updates and or documentation.
	/// </p>
	/// <p>
	/// The homepage for the .NET DataProvider is: 
	/// <a href="http://gborg.postgresql.org/project/npgsql/projdisplay.php">http://gborg.postgresql.org/project/npgsql/projdisplay.php</a>. 
	/// </p>
	/// </remarks>
	public class NpgsqlDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		public NpgsqlDriver()
		{
			connectionType = System.Type.GetType("Npgsql.NpgsqlConnection, Npgsql");
			commandType = System.Type.GetType("Npgsql.NpgsqlCommand, Npgsql");
		}

		public override System.Type CommandType
		{
			get { return commandType; }
		}

		public override System.Type ConnectionType
		{
			get { return connectionType; }
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
			get { return ":"; }
		}

		public override bool SupportsMultipleOpenReaders
		{
			get { return true;	}
		}

		public override bool SupportsPreparingCommands
		{
			// NOTE: Npgsql actually supports this feature but there a bug that results in 
			// NotSupportedException("Backend sent unrecognized response type") being thrown on insert statements
			// this should be fixed with Npgsql post 0.7beta1 (GBorg Bug ID 952) so we need to re-evaluate this override then

			get { return false; }
		}
	}
}

