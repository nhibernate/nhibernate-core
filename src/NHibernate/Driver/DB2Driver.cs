using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2 DataProvider.
	/// </summary>
	public class DB2Driver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary>
		/// 
		/// </summary>
		public DB2Driver()
		{
			connectionType = System.Type.GetType( "IBM.Data.DB2.DB2Connection, IBM.Data.DB2" );
			commandType = System.Type.GetType( "IBM.Data.DB2.DB2Command, IBM.Data.DB2" );
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

		/// <summary></summary>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}
	}
}