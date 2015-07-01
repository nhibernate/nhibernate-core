namespace NHibernate.Driver
{
	/// <summary>
	/// This provides a driver base for Sybase ASE 15 using the ADO.NET driver. (Also known as SAP
	/// Adaptive Server Enterprise.)
	/// </summary>
	/// <remarks>
	/// ASE was formerly Sybase SQL Server, not to be confused with SQL Anywhere / ASA.
	/// </remarks>
	public abstract class SybaseAseClientDriverBase : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SybaseAseClientDriverBase" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="assemblyName">Assembly to load the types from.</param>
		protected SybaseAseClientDriverBase(string assemblyName) : base(
			"Sybase.Data.AseClient",
			assemblyName,
			"Sybase.Data.AseClient.AseConnection",
			"Sybase.Data.AseClient.AseCommand")
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="SybaseAseClientDriverBase" /> with
		/// type names that are loaded from the specified assembly.
		/// </summary>
		/// <param name="providerInvariantName">The Invariant name of a provider.</param>
		/// <param name="assemblyName">Assembly to load the types from.</param>
		/// <param name="connectionTypeName">Connection type name.</param>
		/// <param name="commandTypeName">Command type name.</param>
		protected SybaseAseClientDriverBase(
			string providerInvariantName,
			string assemblyName,
			string connectionTypeName,
			string commandTypeName) : base(
			providerInvariantName,
			assemblyName,
			connectionTypeName,
			commandTypeName)
		{
		}

		/// <inheritdoc />
		public override string NamedPrefix => "@";

		/// <inheritdoc />
		public override bool UseNamedPrefixInParameter => true;

		/// <inheritdoc />
		public override bool UseNamedPrefixInSql => true;
	}
}
