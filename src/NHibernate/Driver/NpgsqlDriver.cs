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
	public class NpgsqlDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NpgsqlDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>Npgsql</c> assembly can not be loaded.
		/// </exception>
		public NpgsqlDriver() : base(
			"Npgsql",
			"Npgsql.NpgsqlConnection",
			"Npgsql.NpgsqlCommand" )
		{
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
			get { return true; }
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