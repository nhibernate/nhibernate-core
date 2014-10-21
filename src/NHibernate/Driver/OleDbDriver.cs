using System;
using System.Data;
using System.Data.OleDb;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the OleDb DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the OleDb DataProvider.
	/// </remarks>
	public class OleDbDriver : DriverBase
	{
		public OleDbDriver()
		{
		}

		public override void AddNotificationHandler(IDbConnection con, Delegate handler)
		{
			//NH-3724
			(con as OleDbConnection).InfoMessage += (OleDbInfoMessageEventHandler)handler;

			base.AddNotificationHandler(con, handler);
		}

		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		public override IDbCommand CreateCommand()
		{
			return new OleDbCommand();
		}

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