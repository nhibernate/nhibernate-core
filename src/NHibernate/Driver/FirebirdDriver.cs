namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the FirebirdSql.Data.Firebird DataProvider.
	/// </summary>
	public class FirebirdDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary>
		/// Initializes a new instance of the <see cref="FirebirdDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>FirebirdSql.Data.Firebird</c> assembly is not and can not be loaded.
		/// </exception>
		public FirebirdDriver()
		{
			string assemblyName = "FirebirdSql.Data.Firebird";
			string connectionClassName = "FirebirdSql.Data.Firebird.FbConnection";
			string commandClassName = "FirebirdSql.Data.Firebird.FbCommand";

			// try to get the Types from an already loaded assembly
			connectionType = Util.ReflectHelper.TypeFromAssembly( connectionClassName, assemblyName );
			commandType = Util.ReflectHelper.TypeFromAssembly( commandClassName, assemblyName );

			if( connectionType == null || commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly FirebirdSql.Data could not be found.  " +
						"Please ensure that the Assemblies needed to communicate with Firebird " +
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
			get { return "@"; }
		}
	}
}