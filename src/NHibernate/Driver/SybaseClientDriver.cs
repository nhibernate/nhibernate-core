using System;
using System.Data;

namespace NHibernate.Driver
{
	/// <summary>
	/// The SybaseClientDriver Driver provides a database driver for Sybase.
	/// </summary>
	/// <remarks>
	/// It has been reported to work with the <see cref="Dialect.MsSql2000Dialect"/>.
	/// </remarks>
	public class SybaseClientDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseClientDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Sybase.Data.AseClient assembly is not and can not be loaded.
		/// </exception>
		public SybaseClientDriver()
		{

			string assemblyName = "Sybase.Data.AseClient";
			string connectionClassName = "Sybase.Data.AseClient.AseConnection";
			string commandClassName = "Sybase.Data.AseClient.AseCommand";

			// try to get the Types from an already loaded assembly
			connectionType = Util.ReflectHelper.TypeFromAssembly( connectionClassName, assemblyName );
			commandType = Util.ReflectHelper.TypeFromAssembly( commandClassName, assemblyName );

			if( connectionType == null || commandType == null )
			{
				throw new HibernateException(
					"The IDbCommand and IDbConnection implementation in the Assembly Sybase.Data.AseClient.dll could not be found.  " +
					"Please ensure that the Assemblies needed to communicate with Sybase " +
					"are in the Global Assembly Cache or in a location that NHibernate " +
					"can use System.Type.GetType(string) to load the types from." 
					);
			}

			

		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> from the Sybase.Data.AseClient assembly
		/// that implements <see cref="IDbCommand"/>
		/// </summary>
		/// <value>The <c>Sybase.Data.AseClient.AseCommand</c>c> type.</value>
		public override System.Type CommandType
		{
			get { return commandType; }
		}

		/// <summary>
		/// Gets the <see cref="System.Type"/> from the Sybase.Data.AseClient assembly
		/// that implements <see cref="IDbCommand"/>
		/// </summary>
		/// <value>The <c>Sybase.Data.AseClient.AseConnection</c>c> type.</value>
		public override System.Type ConnectionType
		{
			get { return connectionType; }
		}

		/// <summary>
		/// Sybase.Data.AseClient uses named parameters in the sql.
		/// </summary>
		/// <value><c>true</c> - Sybase uses <c>@</c> in the sql.</value>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary>
		/// Sybase.Data.AseClient use the <c>@</c> to locate parameters in sql.
		/// </summary>
		/// <value><c>@</c> is used to locate parameters in sql.</value>
		public override string NamedPrefix
		{
			get { return "@"; }
		}

	}
}
