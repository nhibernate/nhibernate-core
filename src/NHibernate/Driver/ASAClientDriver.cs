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
		/// Thrown when the iAnywhere.Data.SQLAnywhere assembly is not and can not be loaded.
		/// </exception>
		public ASAClientDriver()
			: base("iAnywhere.Data.SQLAnywhere", "iAnywhere.Data.SQLAnywhere.SAConnection", "iAnywhere.Data.SQLAnywhere.SACommand")
		{
		}

		/// <summary>
		/// iAnywhere.Data.SQLAnywhere uses named parameters in the sql.
		/// </summary>
		/// <value><see langword="true" /> - Sybase uses <c>String.Empty</c> in the sql.</value>
		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		/// <summary>
		/// iAnywhere.Data.SQLAnywhere use the <c>string.Empty</c> to locate parameters in sql.
		/// </summary>
		public override string NamedPrefix
		{
			get { return string.Empty; }
		}
	}
}
