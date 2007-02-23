using System;
using System.Data;
using NHibernate.Util;

namespace NHibernate.Driver
{
	public abstract class ReflectionBasedDriver : DriverBase
	{
		private System.Type connectionType;
		private System.Type commandType;

		/// <summary>
		/// Initializes a new instance of <see cref="ReflectionBasedDriver" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="driverAssemblyName">Assembly to load the types from.</param>
		/// <param name="connectionTypeName">Connection type name.</param>
		/// <param name="commandTypeName">Command type name.</param>
		public ReflectionBasedDriver(string driverAssemblyName, string connectionTypeName, string commandTypeName)
		{
			// Try to get the types from an already loaded assembly
			connectionType = ReflectHelper.TypeFromAssembly(connectionTypeName, driverAssemblyName, false);
			commandType = ReflectHelper.TypeFromAssembly(commandTypeName, driverAssemblyName, false);

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
		}

		public override IDbConnection CreateConnection()
		{
			return (IDbConnection) Activator.CreateInstance(connectionType);
		}

		public override IDbCommand CreateCommand()
		{
			return (IDbCommand) Activator.CreateInstance(commandType);
		}
	}
}