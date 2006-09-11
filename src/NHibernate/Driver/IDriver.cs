using System.Data;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A strategy for describing how NHibernate should interact with the different .NET Data
	/// Providers.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The <c>IDriver</c> interface is not intended to be exposed to the application.
	/// Instead it is used internally by NHibernate to obtain connection objects, command objects, and
	/// to generate and prepare <see cref="IDbCommand">IDbCommands</see>. Implementors should provide a
	/// public default constructor.
	/// </para>
	/// <para>
	/// This is the interface to implement, or you can inherit from <see cref="DriverBase"/> 
	/// if you have an ADO.NET data provider that NHibernate does not have built in support for.
	/// To use the driver, NHibernate property <c>hibernate.connection.driver_class</c> should be
	/// set to the assembly-qualified name of the driver class.
	/// </para>
	/// <code>
	/// key="hibernate.connection.driver_class"
	/// value="FullyQualifiedClassName, AssemblyName"
	/// </code>
	/// </remarks>
	public interface IDriver
	{
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

		void PrepareCommand(IDbCommand command, SqlType[] parameterTypes);

		/// <summary>
		/// Generates an IDbCommand from the SqlString according to the requirements of the DataProvider.
		/// </summary>
		/// <param name="type">The <see cref="CommandType"/> of the command to generate.</param>
		/// <param name="sqlString">The SqlString that contains the sql and parameters.</param>
		/// <returns>An IDbCommand with the CommandText and Parameters fully set.</returns>
		IDbCommand GenerateCommand(CommandType type, SqlString sqlString);
	}
}