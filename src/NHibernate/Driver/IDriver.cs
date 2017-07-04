using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Engine;
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
	/// to generate and prepare <see cref="DbCommand">DbCommands</see>. Implementors should provide a
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
		/// Creates an uninitialized DbConnection object for the specific Driver
		/// </summary>
		DbConnection CreateConnection();

		/// <summary>
		/// Does this Driver support having more than 1 open DbDataReader with
		/// the same DbConnection.
		/// </summary>
		/// <remarks>
		/// <para>
		/// A value of <see langword="false" /> indicates that an exception would be thrown if NHibernate
		/// attempted to have 2 DbDataReaders open using the same DbConnection.  NHibernate
		/// (since this version is a close to straight port of Hibernate) relies on the 
		/// ability to recursively open 2 DbDataReaders.  If the Driver does not support it
		/// then NHibernate will read the values from the DbDataReader into an <see cref="NDataReader"/>.
		/// </para>
		/// <para>
		/// A value of <see langword="true" /> will result in greater performance because an DbDataReader can be used
		/// instead of the <see cref="NDataReader"/>.  So if the Driver supports it then make sure
		/// it is set to <see langword="true" />.
		/// </para>
		/// </remarks>
		bool SupportsMultipleOpenReaders { get; }

		/// <summary>
		/// Generates an DbCommand from the SqlString according to the requirements of the DataProvider.
		/// </summary>
		/// <param name="type">The <see cref="CommandType"/> of the command to generate.</param>
		/// <param name="sqlString">The SqlString that contains the SQL.</param>
		/// <param name="parameterTypes">The types of the parameters to generate for the command.</param>
		/// <returns>An DbCommand with the CommandText and Parameters fully set.</returns>
		DbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes);

		/// <summary>
		/// Prepare the <paramref name="command" /> by calling <see cref="DbCommand.Prepare()" />.
		/// May be a no-op if the driver does not support preparing commands, or for any other reason.
		/// </summary>
		/// <param name="command">The command.</param>
		void PrepareCommand(DbCommand command);

		/// <summary>
		/// Generates an DbParameter for the DbCommand.  It does not add the DbParameter to the DbCommand's
		/// Parameter collection.
		/// </summary>
		/// <param name="command">The DbCommand to use to create the DbParameter.</param>
		/// <param name="name">The name to set for DbParameter.Name</param>
		/// <param name="sqlType">The SqlType to set for DbParameter.</param>
		/// <returns>An DbParameter ready to be added to an DbCommand.</returns>
		DbParameter GenerateParameter(DbCommand command, string name, SqlType sqlType);

		/// <summary>
		/// Remove 'extra' parameters from the DbCommand
		/// </summary>
		/// <remarks>
		/// We sometimes create more parameters than necessary (see NH-2792 &amp; also comments in SqlStringFormatter.ISqlStringVisitor.Parameter)
		/// </remarks>
		void RemoveUnusedCommandParameters(DbCommand cmd, SqlString sqlString);

		/// <summary>
		/// Expand the parameters of the cmd to have a single parameter for each parameter in the
		/// sql string
		/// </summary>
		/// <remarks>
		/// This is for databases that do not support named parameters.  So, instead of a single parameter
		/// for 'select ... from MyTable t where t.Col1 = @p0 and t.Col2 = @p0' we can issue
		/// 'select ... from MyTable t where t.Col1 = ? and t.Col2 = ?'
		/// </remarks>
		void ExpandQueryParameters(DbCommand cmd, SqlString sqlString);

		IResultSetsCommand GetResultSetsCommand(ISessionImplementor session);
		bool SupportsMultipleQueries { get; }

		/// <summary>
		/// Make any adjustments to each DbCommand object before it is added to the batcher.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <remarks>
		/// This method should be executed before add each single command to the batcher.
		/// If you have to adjust parameters values/type (when the command is full filled) this is a good place where do it.
		/// </remarks>
		void AdjustCommand(DbCommand command);

		/// <summary>
		/// Does this driver mandates <see cref="TimeSpan"/> values for time?
		/// </summary>
		bool RequiresTimeSpanForTime { get; }
	}
}