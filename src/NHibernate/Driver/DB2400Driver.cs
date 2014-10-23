using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the IBM.Data.DB2.iSeries DataProvider.
	/// </summary>
	public class DB2400Driver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DB2Driver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>IBM.Data.DB2.iSeries</c> assembly can not be loaded.
		/// </exception>
		public DB2400Driver() : base(
			"IBM.Data.DB2.iSeries",
			"IBM.Data.DB2.iSeries.iDB2Connection",
			"IBM.Data.DB2.iSeries.iDB2Command")
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