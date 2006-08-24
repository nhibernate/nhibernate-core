using System.Data;
using System.Data.SqlClient;

using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class SqlClientDriver : DriverBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlClientDriver"/> class.
		/// </summary>
		public SqlClientDriver()
		{
		}

		/// <summary>
		/// Creates an uninitialized <see cref="IDbConnection" /> object for 
		/// the SqlClientDriver.
		/// </summary>
		/// <value>An unitialized <see cref="System.Data.SqlClient.SqlConnection"/> object.</value>
		public override IDbConnection CreateConnection()
		{
			return new SqlConnection();
		}

		/// <summary>
		/// Creates an uninitialized <see cref="IDbCommand" /> object for 
		/// the SqlClientDriver.
		/// </summary>
		/// <value>An unitialized <see cref="System.Data.SqlClient.SqlCommand"/> object.</value>
		public override IDbCommand CreateCommand()
		{
			return new System.Data.SqlClient.SqlCommand();
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// <c>true</c> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		/// <summary>
		/// MsSql requires the use of a Named Prefix in the Parameter.  
		/// </summary>
		/// <remarks>
		/// <c>true</c> because MsSql uses "<c>@</c>".
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
		/// <value><c>false</c> - it is not supported.</value>
		/// <remarks>
		/// MS SQL Server 2000 (and 7) throws an exception when multiple IDataReaders are 
		/// attempted to be opened.  When SQL Server 2005 comes out a new driver will be 
		/// created for it because SQL Server 2005 is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		/// <remarks>
		/// In order to prepare an IDbCommand against an MsSql database all variable length values need
		/// to be set.
		/// </remarks>
		public override void PrepareCommand(IDbCommand command, SqlType[] parameterTypes)
		{
			SetParameterSizes(command.Parameters, parameterTypes);
			base.PrepareCommand(command, parameterTypes);
		}

		// Used from SqlServerCeDriver as well
		public static void SetParameterSizes(IDataParameterCollection parameters, SqlType[] parameterTypes)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				SetVariableLengthParameterSize((IDbDataParameter) parameters[i], parameterTypes[i]);
			}
		}

		private const int MaxAnsiStringSize = 8000;
		private const int MaxBinarySize = MaxAnsiStringSize;
		private const int MaxStringSize = MaxAnsiStringSize / 2;
		private const int MaxBinaryBlobSize = int.MaxValue;
		private const int MaxStringClobSize = MaxBinaryBlobSize / 2;

		private static void SetDefaultParameterSize(IDbDataParameter dbParam, SqlType sqlType)
		{
			switch (dbParam.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					dbParam.Size = MaxAnsiStringSize;
					break;

				case DbType.Binary:
					if (sqlType is BinaryBlobSqlType)
					{
						dbParam.Size = MaxBinaryBlobSize;
					}
					else
					{
						dbParam.Size = MaxBinarySize;
					}
					break;

				case DbType.String:
				case DbType.StringFixedLength:
					if (sqlType is StringClobSqlType)
					{
						dbParam.Size = MaxStringClobSize;
					}
					else
					{
						dbParam.Size = MaxStringSize;
					}
					break;
			}
		}

		private static void SetVariableLengthParameterSize(IDbDataParameter dbParam, SqlType sqlType)
		{
			SetDefaultParameterSize(dbParam, sqlType);

			// Override the defaults using data from SqlType.
			if (sqlType.LengthDefined)
			{
				dbParam.Size = sqlType.Length;
			}

			if (sqlType.PrecisionDefined)
			{
				dbParam.Precision = sqlType.Precision;
				dbParam.Scale = sqlType.Scale;
			}
		}
	}
}