using System;
using System.Data.Common;
using NHibernate.Util;

namespace NHibernate.Driver
{
	public abstract class ReflectionBasedDriver : DriverBase
	{
		protected const string ReflectionTypedProviderExceptionMessageTemplate = "The DbCommand and DbConnection implementation in the assembly {0} could not be found. "
		                                                                       + "Ensure that the assembly {0} is located in the application directory or in the Global "
		                                                                       + "Assembly Cache. If the assembly is in the GAC, use <qualifyAssembly/> element in the "
		                                                                       + "application configuration file to specify the full name of the assembly.";

		private readonly IDriveConnectionCommandProvider connectionCommandProvider;

		/// <summary>
		/// If the driver use a third party driver (not a .Net Framework DbProvider), its assembly version.
		/// </summary>
		protected Version DriverVersion { get; } 

		/// <summary>
		/// Initializes a new instance of <see cref="ReflectionBasedDriver" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="driverAssemblyName">Assembly to load the types from.</param>
		/// <param name="connectionTypeName">Connection type name.</param>
		/// <param name="commandTypeName">Command type name.</param>
		protected ReflectionBasedDriver(string driverAssemblyName, string connectionTypeName, string commandTypeName)
			: this(null, driverAssemblyName, connectionTypeName, commandTypeName)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ReflectionBasedDriver" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="providerInvariantName">The Invariant name of a provider.</param>
		/// <param name="driverAssemblyName">Assembly to load the types from.</param>
		/// <param name="connectionTypeName">Connection type name.</param>
		/// <param name="commandTypeName">Command type name.</param>
		/// <seealso cref="DbProviderFactories.GetFactory(string)"/>
		protected ReflectionBasedDriver(string providerInvariantName, string driverAssemblyName, string connectionTypeName, string commandTypeName)
		{
			// Try to get the types from an already loaded assembly
			var connectionType = ReflectHelper.TypeFromAssembly(connectionTypeName, driverAssemblyName, false);
			var commandType = ReflectHelper.TypeFromAssembly(commandTypeName, driverAssemblyName, false);

			if (connectionType == null || commandType == null)
			{
				if (string.IsNullOrEmpty(providerInvariantName))
				{
					throw new HibernateException(string.Format(ReflectionTypedProviderExceptionMessageTemplate, driverAssemblyName));
				}
				var factory = DbProviderFactories.GetFactory(providerInvariantName);
				connectionCommandProvider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			}
			else
			{
				connectionCommandProvider = new ReflectionDriveConnectionCommandProvider(connectionType, commandType);
				DriverVersion = connectionType.Assembly.GetName().Version;
			}
		}

		public override DbConnection CreateConnection()
		{
			return connectionCommandProvider.CreateConnection();
		}

		public override DbCommand CreateCommand()
		{
			return connectionCommandProvider.CreateCommand();
		}
	}
}