using System;
using System.Data;
using System.Collections;

using NHibernate.Driver;

namespace NHibernate.Connection 
{
	/// <summary>
	/// A strategy for obtaining ADO.NET connections.
	/// </summary>
	/// <remarks>
	/// The <c>IConnectionProvider</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by Hibernate to obtain connections. Implementors should provide
	/// a public default constructor.
	/// </remarks>
	public interface IConnectionProvider 
	{

		/// <summary>
		/// The Driver this ConnectionProvider should use to communicate with the .NET Data Provider
		/// </summary>
		/// <value></value>
		/// <remarks></remarks>
		IDriver Driver {get;}

		/// <summary>
		/// Initialize the connection provider from the given properties.
		/// </summary>
		/// <param name="settings">The connection provider settings</param>
		void Configure(IDictionary settings); 

		/// <summary>
		/// Grab a connection 
		/// </summary>
		/// <returns>An ADO.NET connection</returns>
		IDbConnection GetConnection();

		/// <summary>
		/// Dispose of a used connection
		/// </summary>
		/// <param name="conn">An ADO.NET connection</param>
		void CloseConnection(IDbConnection conn);

		/// <summary>
		/// Does this ConnectionProvider implement a <c>PreparedStatemnt</c> cache?.
		/// </summary>
		/// <remarks>
		/// If so, Hibernate will not use its own cache
		/// </remarks>
		bool IsStatementCache { get; }

	}
}
