using System;
using System.Data;
using System.Reflection;

namespace NHibernate.Driver
{
	/// <summary>
	/// The ByteFX.Data Driver provides a database driver for MySql.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the ByteFX.Data.dll Assembly available for 
	/// NHibernate to load it.  The ICSharpCode.SharpZipLib.dll Assembly is also required by
	/// the ByteFX.Data Assembly.
	/// </p>
	/// <p>
	/// Please check the products website <a href="http://www.bytefx.com/">http://www.bytefx.com/</a>
	/// for any updates and or documentation.
	/// </p>
	/// <p>
	/// The sourceforge project for this .NET DataProvider is: 
	/// <a href="http://sourceforge.net/projects/mysqlnet/">http://sourceforge.net/projects/mysqlnet/</a>. 
	/// </p>
	/// </remarks>
	public class ByteFXDataDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		public ByteFXDataDriver()
		{
			connectionType = System.Type.GetType("ByteFX.Data.MySqlClient.MySqlConnection, ByteFX.Data");
			commandType = System.Type.GetType("ByteFX.Data.MySqlClient.MySqlCommand, ByteFX.Data");
		}

		public override IDbConnection CreateConnection()
		{
			return (IDbConnection) Activator.CreateInstance(connectionType);
		}

		public override IDbCommand CreateCommand() 
		{
			return (IDbCommand) Activator.CreateInstance(commandType);
		}

		public override bool UseNamedPrefixInSql 
		{
			get {return true;}
		}

		public override bool UseNamedPrefixInParameter 
		{
			get {return true;}
		}

		public override string NamedPrefix 	
		{
			get {return "@";}
		}
	}
}

