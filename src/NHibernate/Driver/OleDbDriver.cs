using System;
using System.Data.Common;
using System.Data.OleDb;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the OleDb DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the OleDb DataProvider.
	/// </remarks>
	public class OleDbDriver 
#if NETFX
		: DriverBase
#else
		: ReflectionBasedDriver
#endif
	{

#if !NETFX
		public OleDbDriver()
			: base ("System.Data.OleDb", "System.Data.OleDb.OleDbConnection", "System.Data.OleDb.OleDbCommand")
		{
		}
#else
		public override DbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		public override DbCommand CreateCommand()
		{
			return new OleDbCommand();
		}
#endif

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return String.Empty; }
		}

		/// <summary>
		/// OLE DB provider does not support multiple open data readers
		/// </summary>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}
	}
}
