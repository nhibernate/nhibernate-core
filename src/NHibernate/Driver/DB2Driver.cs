using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2 DataProvider.
	/// </summary>
	public class DB2Driver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DB2Driver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>IBM.Data.DB2</c> assembly can not be loaded.
		/// </exception>
		public DB2Driver() : base(
			"IBM.Data.DB2",
			"IBM.Data.DB2",
			"IBM.Data.DB2.DB2Connection",
			"IBM.Data.DB2.DB2Command")
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

		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}
	}
}