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

		public DB2Driver()
		{
			connectionType = System.Type.GetType("IBM.Data.DB2.DB2Connection, IBM.Data.DB2");
			commandType = System.Type.GetType("IBM.Data.DB2.DB2Command, IBM.Data.DB2");
		}

		public override System.Type CommandType
		{
			get { return commandType; }
		}

		public override System.Type ConnectionType
		{
			get { return connectionType; }
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

		public override bool SupportsMultipleOpenReaders
		{
			get { return false;	}
		}
	}
}
