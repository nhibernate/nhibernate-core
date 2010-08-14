using System.Data;
using NHibernate.Util;

namespace NHibernate.Driver
{
	public abstract class ReflectionBasedDriver : DriverBase
	{
		private readonly IDriveConnectionCommandProvider connectionCommandProvider;

		/// <summary>
		/// Initializes a new instance of <see cref="ReflectionBasedDriver" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="driverAssemblyName">Assembly to load the types from.</param>
		/// <param name="connectionTypeName">Connection type name.</param>
		/// <param name="commandTypeName">Command type name.</param>
		protected ReflectionBasedDriver(string driverAssemblyName, string connectionTypeName, string commandTypeName)
		{
			// Try to get the types from an already loaded assembly
			var connectionType = ReflectHelper.TypeFromAssembly(connectionTypeName, driverAssemblyName, false);
			var commandType = ReflectHelper.TypeFromAssembly(commandTypeName, driverAssemblyName, false);

			if (connectionType == null || commandType == null)
			{
				throw new HibernateException(
					string.Format(
						"The IDbCommand and IDbConnection implementation in the assembly {0} could not be found. "
						+ "Ensure that the assembly {0} is located in the application directory or in the Global "
						+ "Assembly Cache. If the assembly is in the GAC, use <qualifyAssembly/> element in the "
						+ "application configuration file to specify the full name of the assembly.",
						driverAssemblyName));
			}
			connectionCommandProvider = new ReflectionDriveConnectionCommandProvider(connectionType, commandType);
		}

		public override IDbConnection CreateConnection()
		{
			return connectionCommandProvider.CreateConnection();
		}

		public override IDbCommand CreateCommand()
		{
			return connectionCommandProvider.CreateCommand();
		}
	}
}