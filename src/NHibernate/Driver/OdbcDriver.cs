using System;
using System.Data;
using System.Data.Odbc;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Odbc DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the Odbc DataProvider.
	/// </remarks>
	public class OdbcDriver : DriverBase
	{
		/// <summary></summary>
		public OdbcDriver()
		{
		}

		/// <summary></summary>
		public override System.Type CommandType
		{
			get { return typeof( OdbcCommand ); }
		}

		/// <summary></summary>
		public override System.Type ConnectionType
		{
			get { return typeof( OdbcConnection ); }
		}

		/// <summary></summary>
		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection();
		}

		/// <summary></summary>
		public override IDbCommand CreateCommand()
		{
			return new OdbcCommand();
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return String.Empty; }
		}
	}
}