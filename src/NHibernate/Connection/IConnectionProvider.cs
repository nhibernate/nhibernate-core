using System;
using System.Collections.Generic;
using System.Data.Common;
using NHibernate.Driver;

namespace NHibernate.Connection
{
	/// <summary>
	/// A strategy for obtaining ADO.NET <see cref="DbConnection"/>.
	/// </summary>
	/// <remarks>
	/// The <c>IConnectionProvider</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by NHibernate to obtain <see cref="DbConnection"/>. 
	/// Implementors should provide a public default constructor.
	/// </remarks>
	public partial interface IConnectionProvider : IDisposable
	{
		/// <summary>
		/// Initialize the connection provider from the given properties.
		/// </summary>
		/// <param name="settings">The connection provider settings</param>
		void Configure(IDictionary<string, string> settings);

		/// <summary>
		/// Dispose of a used <see cref="DbConnection"/>
		/// </summary>
		/// <param name="conn">The <see cref="DbConnection"/> to clean up.</param>
		void CloseConnection(DbConnection conn);

		/// <summary>
		/// Gets the <see cref="IDriver"/> this ConnectionProvider should use to 
		/// communicate with the .NET Data Provider
		/// </summary>
		/// <value>
		/// The <see cref="IDriver"/> to communicate with the .NET Data Provider.
		/// </value>
		IDriver Driver { get; }

		/// <summary>
		/// Get an open <see cref="DbConnection"/>.
		/// </summary>
		/// <returns>An open <see cref="DbConnection"/>.</returns>
		DbConnection GetConnection();
	}
}