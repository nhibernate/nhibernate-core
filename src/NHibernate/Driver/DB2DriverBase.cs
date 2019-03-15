using NHibernate.Engine;

namespace NHibernate.Driver
{
	/// <summary>
	/// A base for NHibernate Driver for using the IBM.Data.DB2 or IBM.Data.DB2.Core DataProvider.
	/// </summary>
	public abstract class DB2DriverBase : ReflectionBasedDriver
	{
		/// <param name="assemblyName"></param>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>assemblyName</c> assembly can not be loaded.
		/// </exception>
		protected DB2DriverBase(string assemblyName) :
			base(assemblyName, assemblyName, assemblyName + ".DB2Connection", assemblyName + ".DB2Command")
		{
		}

		public override bool UseNamedPrefixInSql => false;

		public override bool UseNamedPrefixInParameter => false;

		public override string NamedPrefix => string.Empty;

		public override bool SupportsMultipleOpenReaders => false;

		/// <summary>
		/// Gets a value indicating whether the driver [supports multiple queries].
		/// </summary>
		/// <value>
		/// <c>true</c> if [supports multiple queries]; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsMultipleQueries => true;

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
