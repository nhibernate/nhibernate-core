using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Driver;

namespace NHibernate.Connection
{
	/// <summary>
	/// A strategy for obtaining ADO.NET <see cref="IDbConnection"/>.
	/// </summary>
	/// <remarks>
	/// The <c>IConnectionProvider</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by NHibernate to obtain <see cref="IDbConnection"/>. 
	/// Implementors should provide a public default constructor.
	/// </remarks>
	public interface IConnectionProvider : IDisposable
	{
		/// <summary>
		/// Initialize the connection provider from the given properties.
		/// </summary>
		/// <param name="settings">The connection provider settings</param>
		void Configure(IDictionary<string, string> settings);

		/// <summary>
		/// Dispose of a used <see cref="IDbConnection"/>
		/// </summary>
		/// <param name="conn">The <see cref="IDbConnection"/> to clean up.</param>
		void CloseConnection(IDbConnection conn);

		/// <summary>
		/// Gets the <see cref="IDriver"/> this ConnectionProvider should use to 
		/// communicate with the .NET Data Provider
		/// </summary>
		/// <value>
		/// The <see cref="IDriver"/> to communicate with the .NET Data Provider.
		/// </value>
		IDriver Driver { get; }

		/// <summary>
		/// Get an open <see cref="IDbConnection"/>.
		/// </summary>
		/// <returns>An open <see cref="IDbConnection"/>.</returns>
		IDbConnection GetConnection();
	}
}