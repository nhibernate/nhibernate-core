using System;
using System.Data;
using System.Collections;

namespace NHibernate.Connection 
{
	/// <summary>
	/// A strategy for obtaining ADO.NET connections.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>IConnectionProvider</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by Hibernate to obtain connections. Implementors should provide
	/// a public default constructor.
	/// </para>
	/// <para>
	/// This is the interface to implement, or you can inherit from <see cref="ConnectionProvider"/> 
	/// if you have a .NET DataProvider that NHibernate does not have built in support for.  
	/// For example, there is an Assembly for the ByteFX.Data MySql DataProvider.  
	/// It is part of the <c>NHibernate.Connection</c> namespace and that is where your
	/// DataProvider should be placed.  The assembly should start with the name <c>NHibernate.Connectioin</c>,
	/// however it does not have to.  The MySql DataProvider is in an assembly called <c>NHibernate.Connection.ByteFX.dll</c>.
	/// For someone to use it all that needs to be done is in the configuration file for NHibernate this should be
	/// there.
	/// </para>
	/// <code>
	/// key="hibernate.connection.provider"          
	/// value="FullyQualifiedClassName, AssemblyName" 
	/// </code>
	/// <para>
	/// This is the standard .NET way to load a class from an external assembly.
	/// </para>
	/// </remarks>
	public interface IConnectionProvider 
	{

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

		/// <summary>
		/// Does this ConnectionProvider require the use of a Named Prefix in the SQL 
		/// statement.  For example, SqlClient requires select * from simple where simple_id = @simple_id
		/// If this is false, like with the OleDb provider, then it is assumed that  
		/// the ? can be a placeholder for the parameter in the SQL statement.
		/// </summary>
		bool UseNamedPrefixInSql {get;}

		/// <summary>
		/// Does this ConnectionProvider require the use of the Named Prefix when trying
		/// to reference the Parameter in the Command's Parameter collection.  This is
		/// really only useful when the UseNamedPrefixInSql = true.  When this is true the
		/// code will look like IDbParameter param = cmd.Parameters["@paramName"], if this
		/// is false the code will be IDbParameter param = cmd.Parameters["paramName"].
		/// </summary>
		bool UseNamedPrefixInParameter {get;}

		/// <summary>
		/// The Named Prefix for parameters.  Sql Server uses "@" and Oracle uses ":".
		/// </summary>
		string NamedPrefix  {get;}

		/// <summary>
		/// Change the parameterName into the correct format IDbCommand.CommandText
		/// for the ConnectionProvider
		/// </summary>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
		string FormatNameForSql(string parameterName);

		/// <summary>
		/// Change the parameterName into the correct format IDbCommand.CommandText
		/// for the ConnectionProvider
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
		string FormatNameForSql(string tableAlias, string parameterName);


		/// <summary>
		/// Changes the parameterName into the correct format for an IDbParameter
		/// for the ConnectionProvider.
		/// </summary>
		/// <remarks>
		/// For SqlServerConnectionProvider it will change "id" to "@id"
		/// </remarks>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbParameter.</returns>
		string FormatNameForParameter(string parameterName);

		/// <summary>
		/// Changes the parameterName into the correct format for an IDbParameter
		/// for the ConnectionProvider.
		/// </summary>
		/// <remarks>
		/// For SqlServerConnectionProvider it will change "id" to "@id"
		/// </remarks>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbParameter.</returns>
		string FormatNameForParameter(string tableAlias, string parameterName);
		
		/// <summary>
		/// Creates an empty IDbCommand object for the specific provider
		/// </summary>
		/// <remarks>
		/// The reason for having this method is that Interfaces in ADO.NET require 
		/// the use of a IDbCommand.CreateCommand - when we are making the IDbCommand
		/// objects we might not have a particular connection to create the commands
		/// from.
		/// </remarks>
		IDbCommand CreateCommand();
	}
}
