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
		/// <summary></summary>
		public OleDbDriver()
		{
		}

		/// <summary></summary>
		public override System.Type CommandType
		{
			get { return typeof( OleDbCommand ); }
		}

		/// <summary></summary>
		public override System.Type ConnectionType
		{
			get { return typeof( OleDbConnection ); }
		}

		/// <summary></summary>
		public override IDbConnection CreateConnection()
		{
			return new OleDbConnection();
		}

		/// <summary></summary>
		public override IDbCommand CreateCommand()
		{
			return new OleDbCommand();
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