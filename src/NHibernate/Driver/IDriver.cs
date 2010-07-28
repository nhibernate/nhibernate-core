using System.Collections.Generic;
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
	/// To use the driver, NHibernate property <c>connection.driver_class</c> should be
	/// set to the assembly-qualified name of the driver class.
	/// </para>
	/// <code>
	/// key="connection.driver_class"
	/// value="FullyQualifiedClassName, AssemblyName"
	/// </code>
	/// </remarks>
	public interface IDriver
	{
		/// <summary>
		/// Configure the driver using <paramref name="settings"/>.
		/// </summary>
		void Configure(IDictionary<string, string> settings);

		/// <summary>
		/// Creates an uninitialized IDbConnection object for the specific Driver
		/// </summary>
		IDbConnection CreateConnection();

		/// <summary>
		/// Does this Driver support having more than 1 open IDataReader with
		/// the same IDbConnection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A value of <see langword="false" /> indicates that an exception would be thrown if NHibernate
		/// attempted to have 2 IDataReaders open using the same IDbConnection.  NHibernate
		/// (since this version is a close to straight port of Hibernate) relies on the 
		/// ability to recursively open 2 IDataReaders.  If the Driver does not support it
		/// then NHibernate will read the values from the IDataReader into an <see cref="NDataReader"/>.
		/// </para>
		/// <para>
		/// A value of <see langword="true" /> will result in greater performance because an IDataReader can be used
		/// instead of the <see cref="NDataReader"/>.  So if the Driver supports it then make sure
		/// it is set to <see langword="true" />.
		/// </para>
		/// </remarks>
		bool SupportsMultipleOpenReaders { get; }

		/// <summary>
		/// Can we issue several select queries in a single query, and get
		/// several result sets back?
		/// </summary>
		bool SupportsMultipleQueries { get; }

		/// <summary>
		/// How we separate the queries when we use multiply queries.
		/// </summary>
		string MultipleQueriesSeparator { get; }

		/// <summary>
		/// Generates an IDbCommand from the SqlString according to the requirements of the DataProvider.
		/// </summary>
		/// <param name="type">The <see cref="CommandType"/> of the command to generate.</param>
		/// <param name="sqlString">The SqlString that contains the SQL.</param>
		/// <param name="parameterTypes">The types of the parameters to generate for the command.</param>
		/// <returns>An IDbCommand with the CommandText and Parameters fully set.</returns>
		IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes);

		/// <summary>
		/// Prepare the <paramref name="command" /> by calling <see cref="IDbCommand.Prepare()" />.
		/// May be a no-op if the driver does not support preparing commands, or for any other reason.
		/// </summary>
		/// <param name="command"></param>
		void PrepareCommand(IDbCommand command);

		/// <summary>
		/// Generates an IDbDataParameter for the IDbCommand.  It does not add the IDbDataParameter to the IDbCommand's
		/// Parameter collection.
		/// </summary>
		/// <param name="command">The IDbCommand to use to create the IDbDataParameter.</param>
		/// <param name="name">The name to set for IDbDataParameter.Name</param>
		/// <param name="sqlType">The SqlType to set for IDbDataParameter.</param>
		/// <returns>An IDbDataParameter ready to be added to an IDbCommand.</returns>
		IDbDataParameter GenerateParameter(IDbCommand command, string name, SqlType sqlType);

		/// <summary>
		/// Expand the parameters of the cmd to have a single parameter for each parameter in the
		/// sql string
		/// </summary>
		/// <remarks>
		/// This is for databases that do not support named parameters.  So, instead of a single parameter
		/// for 'select ... from MyTable t where t.Col1 = @p0 and t.Col2 = @p0' we can issue
		/// 'select ... from MyTable t where t.Col1 = ? and t.Col2 = ?'
		/// </remarks>
		void ExpandQueryParameters(IDbCommand cmd, SqlString sqlString);
	}
}