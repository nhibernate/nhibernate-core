using System;

namespace NHibernate.Driver 
{

	/// <summary>
	/// The MySqlDataDriver Driver provides a database driver for MySql.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the Assembly <c>MySql.Data.dll</c> available for 
	/// NHibernate to load it.  The Assembly <c>ICSharpCode.SharpZipLib.dll</c> is also required by
	/// the Assembly <c>MySql.Data.dll</c>.
	/// </p>
	/// <p>
	/// Please check the products website 
	/// <a href="http://www.mysql.com/products/connector/net/">http://www.mysql.com/products/connector/net/</a>
	/// for any updates and or documentation.
	/// </p>
	/// </remarks>
	public class MySqlDataDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		public MySqlDataDriver()
		{
			connectionType = System.Type.GetType("MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
			commandType = System.Type.GetType("MySql.Data.MySqlClient.MySqlCommand, MySql.Data");

			if( connectionType==null || commandType==null )
			{
				throw new HibernateException( 
					"The IDbCommand and IDbConnection implementation in the Assembly MySql.Data could not be found.  " +					
					"Please ensure that the Assemblies MySql.Data.dll and ICSharpCode.SharpZipLib.dll " +
					"are in the Global Assembly Cache or in a location that NHibernate " +
					"can use System.Type.GetType(string) to load the types from."
					);
			}
		}

		public override System.Type CommandType
		{
			get	{ return commandType; }
		}

		public override System.Type ConnectionType
		{
			get	{ return connectionType; }
		}

		/// <summary>
		/// MySql.Data uses named parameters in the sql.
		/// </summary>
		/// <value><c>true</c> - MySql uses <c>?</c> in the sql.</value>
		public override bool UseNamedPrefixInSql 
		{
			get {return true;}
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool UseNamedPrefixInParameter 
		{
			get {return true;}
		}

		/// <summary>
		/// MySql.Data use the <c>?</c> to locate parameters in sql.
		/// </summary>
		/// <value><c>?</c> is used to locate parameters in sql.</value>
		public override string NamedPrefix 	
		{
			get {return "?";}
		}

		/// <summary>
		/// The MySql.Data driver does NOT support more than 1 open IDataReader
		/// with only 1 IDbConnection.
		/// </summary>
		/// <value><c>false</c> - it is not supported.</value>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false;	}
		}

		/// <summary>
		/// With the Gamma MySql.Data provider it is throwing an exception with the 
		/// message "Expected End of data packet" when a select command is prepared.
		/// </summary>
		/// <value><c>false</c> - it is not supported.</value>
		public override bool SupportsPreparingCommands
		{
			get { return false; }
		}
	}
}
