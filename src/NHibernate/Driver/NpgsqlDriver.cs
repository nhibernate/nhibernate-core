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
	/// Please check the products website 
	/// <a href="http://www.postgresql.org/">http://www.postgresql.org/</a>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="NpgsqlDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Npgsql</c> assembly is not and can not be loaded.
		/// </exception>
		public NpgsqlDriver()
		{
			string assemblyName = "Npgsql";
			string connectionClassName = "Npgsql.NpgsqlConnection";
			string commandClassName = "Npgsql.NpgsqlCommand";

			// try to get the Types from an already loaded assembly
			connectionType = Util.ReflectHelper.TypeFromAssembly( connectionClassName, assemblyName );
			commandType = Util.ReflectHelper.TypeFromAssembly( commandClassName, assemblyName );

			if( connectionType == null || commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly Npgsql.dll could not be found.  " +
						"Please ensure that the Assemblies needed to communicate with PostgreSQL " +
						"are in the Global Assembly Cache or in a location that NHibernate " +
						"can use System.Type.GetType(string) to load the types from."
					);
			}
		}

		/// <summary></summary>
		public override System.Type CommandType
		{
			get { return commandType; }
		}

		/// <summary></summary>
		public override System.Type ConnectionType
		{
			get { return connectionType; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}

		/// <summary></summary>
		public override bool SupportsMultipleOpenReaders
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsPreparingCommands
		{
			// NOTE: Npgsql actually supports this feature but there a bug that results in 
			// NotSupportedException("Backend sent unrecognized response type") being thrown on insert statements
			// this should be fixed with Npgsql post 0.7beta1 (GBorg Bug ID 952) so we need to re-evaluate this override then

			get { return false; }
		}
	}
}