using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// The MySqlDataDriver Driver provides a database driver for MySql.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the assembly <c>MySql.Data.dll</c> available for 
	/// NHibernate to load it.  The assembly <c>ICSharpCode.SharpZipLib.dll</c> is also required by
	/// the assembly <c>MySql.Data.dll</c>.
	/// </p>
	/// <p>
	/// Please check the products website 
	/// <a href="http://www.mysql.com/products/connector/net/">http://www.mysql.com/products/connector/net/</a>
	/// for any updates and or documentation.
	/// </p>
	/// </remarks>
	public class MySqlDataDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlDataDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>MySql.Data assembly</c> can not be loaded.
		/// </exception>
		public MySqlDataDriver() : base(
			"MySql.Data",
			"MySql.Data.MySqlClient.MySqlConnection",
			"MySql.Data.MySqlClient.MySqlCommand")
		{
		}

		/// <summary>
		/// MySql.Data uses named parameters in the sql.
		/// </summary>
		/// <value><see langword="true" /> - MySql uses <c>?</c> in the sql.</value>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary>
		/// MySql.Data use the <c>?</c> to locate parameters in sql.
		/// </summary>
		/// <value><c>?</c> is used to locate parameters in sql.</value>
		public override string NamedPrefix
		{
			get { return "?"; }
		}

		/// <summary>
		/// The MySql.Data driver does NOT support more than 1 open IDataReader
		/// with only 1 IDbConnection.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		/// <summary>
		/// MySql.Data does not support preparing of commands.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		/// <remarks>
		/// With the Gamma MySql.Data provider it is throwing an exception with the 
		/// message "Expected End of data packet" when a select command is prepared.
		/// </remarks>
		protected override bool SupportsPreparingCommands
		{
			get { return false; }
		}
	}
}