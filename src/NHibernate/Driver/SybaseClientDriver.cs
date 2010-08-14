using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// The SybaseClientDriver Driver provides a database driver for Sybase.
	/// </summary>
	/// <remarks>
	/// It has been reported to work with the <see cref="Dialect.MsSql2000Dialect"/>.
	/// </remarks>
	public class SybaseClientDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseClientDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Sybase.Data.AseClient assembly can not be loaded.
		/// </exception>
		public SybaseClientDriver() : base(
			"Sybase.Data.AseClient",
			"Sybase.Data.AseClient",
			"Sybase.Data.AseClient.AseConnection",
			"Sybase.Data.AseClient.AseCommand")
		{
		}

		/// <summary>
		/// Sybase.Data.AseClient uses named parameters in the sql.
		/// </summary>
		/// <value><see langword="true" /> - Sybase uses <c>@</c> in the sql.</value>
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