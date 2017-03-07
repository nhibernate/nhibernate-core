using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using NHibernate.AdoNet;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class SqlClientDriver : DriverBase, IEmbeddedBatcherFactoryProvider
	{
		public const int MaxSizeForAnsiClob = 2147483647; // int.MaxValue
		public const int MaxSizeForClob = 1073741823; // int.MaxValue / 2
		public const int MaxSizeForBlob = 2147483647; // int.MaxValue
		//http://stackoverflow.com/a/7264795/259946
		public const int MaxSizeForXml = 2147483647; // int.MaxValue
		public const int MaxSizeForLengthLimitedAnsiString = 8000;
		public const int MaxSizeForLengthLimitedString = 4000;
		public const int MaxSizeForLengthLimitedBinary = 8000;
		public const byte MaxPrecision = 28;
		public const byte MaxScale = 5;
		public const byte MaxDateTime2 = 8;
		public const byte MaxDateTimeOffset = 10;

		/// <summary>
		/// Creates an uninitialized <see cref="DbConnection" /> object for
		/// the SqlClientDriver.
		/// </summary>
		/// <value>An unitialized <see cref="System.Data.SqlClient.SqlConnection"/> object.</value>
		public override DbConnection CreateConnection()
		{
			return new SqlConnection();
		}

		/// <summary>
		/// Creates an uninitialized <see cref="DbCommand" /> object for
		/// the SqlClientDriver.
		/// </summary>
		/// <value>An unitialized <see cref="System.Data.SqlClient.SqlCommand"/> object.</value>
		public override DbCommand CreateCommand()
		{
			return new System.Data.SqlClient.SqlCommand();
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
		/// The SqlClient driver does NOT support more than 1 open DbDataReader
		/// with only 1 DbConnection.
		/// </summary>
		/// <value><see langword="false" /> - it is not supported.</value>
		/// <remarks>
		/// MS SQL Server 2000 (and 7) throws an exception when multiple DbDataReaders are
		/// attempted to be opened.  When SQL Server 2005 comes out a new driver will be
		/// created for it because SQL Server 2005 is supposed to support it.
		/// </remarks>
		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);
			SetVariableLengthParameterSize(dbParam, sqlType);
		}

		// Used from SqlServerCeDriver as well
		public static void SetVariableLengthParameterSize(DbParameter dbParam, SqlType sqlType)
		{
			SetDefaultParameterSize(dbParam, sqlType);

			// no longer override the defaults using data from SqlType, since LIKE expressions needs larger columns
			// https://nhibernate.jira.com/browse/NH-3036
			//if (sqlType.LengthDefined && !IsText(dbParam, sqlType) && !IsBlob(dbParam, sqlType))
			//{
			//	dbParam.Size = sqlType.Length;
			//}

			if (sqlType.PrecisionDefined)
			{
				((IDbDataParameter) dbParam).Precision = sqlType.Precision;
				((IDbDataParameter) dbParam).Scale = sqlType.Scale;
			}
		}

		protected static void SetDefaultParameterSize(DbParameter dbParam, SqlType sqlType)
		{
			switch (dbParam.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
                    dbParam.Size = IsAnsiText(dbParam, sqlType) ? MaxSizeForAnsiClob : MaxSizeForLengthLimitedAnsiString;
					break;
				case DbType.Binary:
					dbParam.Size = IsBlob(dbParam, sqlType) ? MaxSizeForBlob : MaxSizeForLengthLimitedBinary;
					break;
				case DbType.Decimal:
					((IDbDataParameter) dbParam).Precision = MaxPrecision;
					((IDbDataParameter) dbParam).Scale = MaxScale;
					break;
				case DbType.String:
				case DbType.StringFixedLength:
					dbParam.Size = IsText(dbParam, sqlType) ? MaxSizeForClob : MaxSizeForLengthLimitedString;
					break;
				case DbType.DateTime2:
					dbParam.Size = MaxDateTime2;
					break;
				case DbType.DateTimeOffset:
					dbParam.Size = MaxDateTimeOffset;
					break;
				case DbType.Xml:
					dbParam.Size = MaxSizeForXml;
					break;
			}
		}

        /// <summary>
        /// Interprets if a parameter is a Clob (for the purposes of setting its default size)
        /// </summary>
        /// <param name="dbParam">The parameter</param>
        /// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
        /// <returns>True, if the parameter should be interpreted as a Clob, otherwise False</returns>
        protected static bool IsAnsiText(DbParameter dbParam, SqlType sqlType)
        {
            return ((DbType.AnsiString == dbParam.DbType || DbType.AnsiStringFixedLength == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MaxSizeForLengthLimitedAnsiString));
        }

		/// <summary>
		/// Interprets if a parameter is a Clob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Clob, otherwise False</returns>
		protected static bool IsText(DbParameter dbParam, SqlType sqlType)
		{
			return (sqlType is StringClobSqlType) || ((DbType.String == dbParam.DbType || DbType.StringFixedLength == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MaxSizeForLengthLimitedString));
		}

		/// <summary>
		/// Interprets if a parameter is a Blob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Blob, otherwise False</returns>
		protected static bool IsBlob(DbParameter dbParam, SqlType sqlType)
		{
			return (sqlType is BinaryBlobSqlType) || ((DbType.Binary == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MaxSizeForLengthLimitedBinary));
		}

		#region IEmbeddedBatcherFactoryProvider Members

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass
		{
			get { return typeof(SqlClientBatchingBatcherFactory); }
		}

		#endregion

		public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}

		public override bool SupportsMultipleQueries
		{
			get { return true; }
		}
	}
}
