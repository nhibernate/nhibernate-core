using System.Collections.Generic;
using System.Data;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate driver for Microsoft SQL Server CE data provider
	/// </summary>
	public class SqlServerCeDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerCeDriver"/> class.
		/// </summary>
		public SqlServerCeDriver()
			: base(
				"System.Data.SqlServerCe",
				"System.Data.SqlServerCe.SqlCeConnection",
				"System.Data.SqlServerCe.SqlCeCommand")
		{
		}

		private bool prepareSql;

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);
			prepareSql = PropertiesHelper.GetBoolean(Environment.PrepareSql, settings, false);
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the Parameter.  
		/// </summary>
		/// <remarks>
		/// <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <value>
		/// Sql Server uses <c>"@"</c>.
		/// </value>
		public override string NamedPrefix
		{
			get { return "@"; }
		}

		/// <summary>
		/// The SqlClient driver does NOT support more than 1 open IDataReader
		/// with only 1 IDbConnection.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		/// <remarks>
		/// Ms Sql 2000 (and 7) throws an Exception when multiple DataReaders are 
		/// attempted to be Opened.  When Yukon comes out a new Driver will be 
		/// created for Yukon because it is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			IDbCommand command = base.GenerateCommand(type, sqlString, parameterTypes);
			if (prepareSql)
			{
				SqlClientDriver.SetParameterSizes(command.Parameters, parameterTypes);
			}

			return command;
		}

		public override bool SupportsMultipleQueries
		{
			get { return true; }
		}
	}
}