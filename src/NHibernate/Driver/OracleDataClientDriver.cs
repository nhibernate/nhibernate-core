using System;

namespace NHibernate.Driver 
{
	/// <summary>
	/// A NHibernate Driver for using the Oracle.DataAccess DataProvider
	/// </summary>
	/// <remarks>
	/// Code was contributed by <a href="http://sourceforge.net/users/jemcalgary/">James Mills</a>
	/// on the NHibernate forums in this 
	/// <a href="http://sourceforge.net/forum/message.php?msg_id=2952662">post</a>.
	/// </remarks>
	public class OracleDataClientDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary>
		/// Initializes a new instance of <see cref="OracleDataClientDriver"/>.
		/// </summary>
		public OracleDataClientDriver()
		{
			connectionType = System.Type.GetType( "Oracle.DataAccess.Client.OracleConnection, Oracle.DataAccess" );
			commandType =System. Type.GetType( "Oracle.DataAccess.Client.OracleCommand, Oracle.DataAccess" );
			
			if( connectionType == null || commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly Oracle.DataAccess could not be found.  " +
					"Please ensure that the Assemblies Oracle.DataAccess.dll " +
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
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string NamedPrefix
		{
			get { return ":"; }
		}
	}
}
	
