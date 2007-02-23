using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// The ASAClientDriver Driver provides a database driver for Adaptive Server Anywhere 9.0.
	/// </summary>
	public class ASAClientDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ASAClientDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the ASA.Data.AseClient assembly is not and can not be loaded.
		/// </exception>
		public ASAClientDriver()
			: base("iAnywhere.Data.AsaClient", "iAnywhere.Data.AsaClient.AsaConnection", "iAnywhere.Data.AsaClient.AsaCommand")
		{
		}

		/// <summary>
		/// iAnywhere.Data.AsaClient uses named parameters in the sql.
		/// </summary>
		/// <value><c>true</c> - Sybase uses <c>String.Empty</c> in the sql.</value>
		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		/// <summary>
		/// iAnywhere.Data.AsaClient use the <c>string.Empty</c> to locate parameters in sql.
		/// </summary>
		public override string NamedPrefix
		{
			get { return string.Empty; }
		}
	}
}