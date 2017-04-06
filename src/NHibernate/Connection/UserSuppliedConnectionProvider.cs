using System;
using System.Collections.Generic;
using System.Data.Common;


namespace NHibernate.Connection
{
	/// <summary>
	/// An implementation of the <c>IConnectionProvider</c> that simply throws an exception when
	/// a connection is requested.
	/// </summary>
	/// <remarks>
	/// This implementation indicates that the user is expected to supply an ADO.NET connection
	/// </remarks>
	public class UserSuppliedConnectionProvider : ConnectionProvider
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(UserSuppliedConnectionProvider));

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> if this method is called
		/// because the user is responsible for closing <see cref="DbConnection"/>s.
		/// </summary>
		/// <param name="conn">The <see cref="DbConnection"/> to clean up.</param>
		/// <exception cref="InvalidOperationException">
		/// Thrown when this method is called.  User is responsible for closing
		/// <see cref="DbConnection"/>s.
		/// </exception>
		public override void CloseConnection(DbConnection conn)
		{
			throw new InvalidOperationException("The User is responsible for closing ADO.NET connection - not NHibernate.");
		}

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> if this method is called
		/// because the user is responsible for creating <see cref="DbConnection"/>s.
		/// </summary>
		/// <returns>
		/// No value is returned because an <see cref="InvalidOperationException"/> is thrown.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when this method is called.  User is responsible for creating
		/// <see cref="DbConnection"/>s.
		/// </exception>
		public override DbConnection GetConnection()
		{
			throw new InvalidOperationException("The user must provide an ADO.NET connection - NHibernate is not creating it.");
		}

		/// <summary>
		/// Configures the ConnectionProvider with only the Driver class.
		/// </summary>
		/// <param name="settings"></param>
		/// <remarks>
		/// All other settings of the Connection are the responsibility of the User since they configured
		/// NHibernate to use a Connection supplied by the User.
		/// </remarks>
		public override void Configure(IDictionary<string, string> settings)
		{
			log.Warn("No connection properties specified - the user must supply an ADO.NET connection");

			ConfigureDriver(settings);
		}
	}
}