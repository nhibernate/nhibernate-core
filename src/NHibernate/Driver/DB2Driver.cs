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
		/// Initializes a new instance of the <see cref="DB2Driver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>IBM.Data.DB2</c> assembly is not and can not be loaded.
		/// </exception>
		public DB2Driver()
		{
			string assemblyName = "IBM.Data.DB2";
			string connectionClassName = "IBM.Data.DB2.DB2Connection";
			string commandClassName = "IBM.Data.DB2.DB2Command";

			// try to get the Types from an already loaded assembly
			connectionType = Util.ReflectHelper.TypeFromAssembly( connectionClassName, assemblyName );
			commandType = Util.ReflectHelper.TypeFromAssembly( commandClassName, assemblyName );

			if( connectionType == null || commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly IBM.Data.DB2 could not be found.  " +
					"Please ensure that the Assemblies needed to communicate with IBM.Data.DB2  " +
					"are in the Global Assembly Cache or in a location that NHibernate " +
					"can use System.Type.GetType(string) to load the types from."
					);
			}
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