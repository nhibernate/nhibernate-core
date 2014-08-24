using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// The SybaseAsaClientDriver driver provides a database driver for Adaptive Server Anywhere 9.0.
	/// </summary>
	public class SybaseAsaClientDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseAsaClientDriver" /> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the iAnywhere.Data.AsaClient assembly is not and can not be loaded.
		/// </exception>
		public SybaseAsaClientDriver()
			: base("iAnywhere.Data.AsaClient", "iAnywhere.Data.AsaClient.AsaConnection", "iAnywhere.Data.AsaClient.AsaCommand")
		{
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
	}
}