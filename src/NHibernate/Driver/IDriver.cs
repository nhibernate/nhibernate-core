using System.Data;
using NHibernate.SqlCommand;

namespace NHibernate.Driver
{
	/// <summary>
	/// A strategy for describing how NHibernate should interact with the different .NET Data
	/// Providers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>IDriver</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by Hibernate to obtain connection objects, command objects, and
	/// to format an IDbCommand's CommandText. Implementors should provide a public default constructor.
	/// </para>
	/// <para>
	/// This is the interface to implement, or you can inherit from <see cref="DriverBase"/> 
	/// if you have a .NET DataProvider that NHibernate does not have built in support for.
	/// </para>
	/// <para>
	/// For example, there is an Assembly for the ByteFX.Data MySql DataProvider.  
	/// It is part of the <c>NHibernate.Driver</c> namespace and that is where your
	/// DataProvider should be placed.  The assembly should start with the name <c>NHibernate.Driver</c>,
	/// however it does not have to.  The MySql DataProvider is in an assembly called <c>NHibernate.Driver.ByteFX.dll</c>.
	/// For someone to use it all that needs to be done is in the configuration file for NHibernate this should be
	/// there.
	/// </para>
	/// <code>
	/// key="hibernate.connection.driver_class"          
	/// value="FullyQualifiedClassName, AssemblyName" 
	/// </code>
	/// <para>
	/// This is the standard .NET way to load a class from an external assembly.
	/// </para>
	/// </remarks>
	public interface IDriver
	{
		/// <summary>
		/// The Type used to create an IDbConnection
		/// </summary>
		System.Type ConnectionType { get; }

		/// <summary>
		/// The Type used to create an IDbCommand
		/// </summary>
		System.Type CommandType { get; }

		/// <summary>
		/// Creates an uninitialized IDbConnection object for the specific Driver
		/// </summary>
		IDbConnection CreateConnection();

		/// <summary>
		/// Creates an empty IDbCommand object for the specific Driver
		/// </summary>
		/// <remarks>
		/// The reason for having this method is that Interfaces in ADO.NET require 
		/// the use of a IDbCommand.CreateCommand - when we are making the IDbCommand
		/// objects we might not have a particular connection to create the commands
		/// from.
		/// </remarks>
		IDbCommand CreateCommand();

		/// <summary>
		/// Does this Driver require the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// For example, SqlClient requires <c>select * from simple where simple_id = @simple_id</c>
		/// If this is false, like with the OleDb provider, then it is assumed that  
		/// the <c>?</c> can be a placeholder for the parameter in the SQL statement.
		/// </remarks>
		bool UseNamedPrefixInSql { get; }

		/// <summary>
		/// Does this Driver require the use of the Named Prefix when trying
		/// to reference the Parameter in the Command's Parameter collection.  
		/// </summary>
		/// <remarks>
		/// This is really only useful when the UseNamedPrefixInSql == true.  When this is true the
		/// code will look like:
		/// <code>IDbParameter param = cmd.Parameters["@paramName"]</code>
		/// if this is false the code will be 
		/// <code>IDbParameter param = cmd.Parameters["paramName"]</code>.
		/// </remarks>
		bool UseNamedPrefixInParameter { get; }

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <remarks>
		/// Sql Server uses <c>"@"</c> and Oracle uses <c>":"</c>.
		/// </remarks>
		string NamedPrefix { get; }

		/// <summary>
		/// Change the parameterName into the correct format IDbCommand.CommandText
		/// for the ConnectionProvider
		/// </summary>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
		string FormatNameForSql( string parameterName );

		/// <summary>
		/// Change the parameterName into the correct format IDbCommand.CommandText
		/// for the ConnectionProvider
		/// </summary>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
		string FormatNameForSql( string tableAlias, string parameterName );


		/// <summary>
		/// Changes the parameterName into the correct format for an IDbParameter
		/// for the Driver.
		/// </summary>
		/// <remarks>
		/// For SqlServerConnectionProvider it will change <c>id</c> to <c>@id</c>
		/// </remarks>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbParameter.</returns>
		string FormatNameForParameter( string parameterName );

		/// <summary>
		/// Changes the parameterName into the correct format for an IDbParameter
		/// for the Driver.
		/// </summary>
		/// <remarks>
		/// For SqlServerConnectionProvider it will change <c>id</c> to <c>@id</c>
		/// </remarks>
		/// <param name="tableAlias">The Alias for the Table.</param>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbParameter.</returns>
		string FormatNameForParameter( string tableAlias, string parameterName );

		/// <summary>
		/// Does this Driver support having more than 1 open IDataReader with
		/// the same IDbConnection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A value of <c>false</c> indicates that an exception would be thrown if NHibernate
		/// attempted to have 2 IDataReaders open using the same IDbConnection.  NHibernate
		/// (since this version is a close to straight port of Hibernate) relies on the 
		/// ability to recursively open 2 IDataReaders.  If the Driver does not support it
		/// then NHibernate will read the values from the IDataReader into an <see cref="NDataReader"/>.
		/// </para>
		/// <para>
		/// A value of <c>true</c> will result in greater performance because an IDataReader can be used
		/// instead of the <see cref="NDataReader"/>.  So if the Driver supports it then make sure
		/// it is set to <c>true</c>.
		/// </para>
		/// </remarks>
		bool SupportsMultipleOpenReaders { get; }

		/// <summary>
		/// Does this Driver support IDbCommand.Prepare().
		/// </summary>
		/// <remarks>
		/// <para>
		/// A value of <c>false</c> indicates that an exception would be thrown or the 
		/// company that produces the Driver we are wrapping does not recommend using
		/// IDbCommand.Prepare().
		/// </para>
		/// <para>
		/// A value of <c>true</c> indicates that calling IDbCommand.Prepare() will function
		/// fine on this Driver.
		/// </para>
		/// </remarks>
		bool SupportsPreparingCommands { get; }


		/// <summary>
		/// Generates an IDbCommand from the SqlString according to the requirements of the DataProvider.
		/// </summary>
		/// <param name="dialect">The Dialect to help build the IDbCommand</param>
		/// <param name="sqlString">The SqlString that contains the sql and parameters.</param>
		/// <returns>An IDbCommand with the CommandText and Parameters fully set.</returns>
		IDbCommand GenerateCommand( Dialect.Dialect dialect, SqlString sqlString );

		/// <summary>
		/// Generates an IDbCommand from the string containing sql according to the requirements 
		/// of the DataProvider.
		/// </summary>
		/// <param name="dialect">The Dialect to help build the IDbCommand</param>
		/// <param name="sqlString">The string that contains the sql that has NO parameters.</param>
		/// <returns>An IDbCommand with the CommandText fully set.</returns>
		/// <remarks>This can not be used to build an IDbCommand that needs to contain Parameters.</remarks>
		IDbCommand GenerateCommand( Dialect.Dialect dialect, string sqlString );


	}
}