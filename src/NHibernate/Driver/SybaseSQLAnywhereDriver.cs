namespace NHibernate.Driver
{
	/// <summary>
	/// The SybaseSQLAnywhereDriver Driver provides a database driver for Sybase SQL Anywhere 10 and above
	/// </summary>
	public class SybaseSQLAnywhereDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SybaseSQLAnywhereDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the iAnywhere.Data.SQLAnywhere assembly is not and can not be loaded.
		/// </exception>
		public SybaseSQLAnywhereDriver()
			: base("iAnywhere.Data.SQLAnywhere", "iAnywhere.Data.SQLAnywhere.SAConnection", "iAnywhere.Data.SQLAnywhere.SACommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return ":"; }
		}
	}
}