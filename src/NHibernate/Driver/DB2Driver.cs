using System;
using NHibernate.Engine;

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

		/// <summary>
		/// Gets a value indicating whether the driver [supports multiple queries].
		/// </summary>
		/// <value>
		/// <c>true</c> if [supports multiple queries]; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsMultipleQueries
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the result sets command.
		/// </summary>
		/// <param name="session">The implementor of the session.</param>
		/// <returns></returns>
		public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}
	}
}
