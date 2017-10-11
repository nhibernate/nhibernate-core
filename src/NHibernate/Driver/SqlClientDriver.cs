using System;
using System.Collections.Generic;
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
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForAnsiClob")]
		public const int MaxSizeForAnsiClob = 2147483647; // int.MaxValue
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForClob")]
		public const int MaxSizeForClob = 1073741823; // int.MaxValue / 2
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForBlob")]
		public const int MaxSizeForBlob = 2147483647; // int.MaxValue

		///<remarks>http://stackoverflow.com/a/7264795/259946</remarks>
		// Since v5.1
		[Obsolete("Use MsSql2005Dialect.MaxSizeForXml")]
		public const int MaxSizeForXml = 2147483647; // int.MaxValue

		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString")]
		public const int MaxSizeForLengthLimitedAnsiString = 8000;
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForLengthLimitedString")]
		public const int MaxSizeForLengthLimitedString = 4000;
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxSizeForLengthLimitedBinary")]
		public const int MaxSizeForLengthLimitedBinary = 8000;
		// Since v5.1
		[Obsolete("This member has no more usages and will be removed in a future version")]
		public const byte MaxPrecision = 28;
		// Since v5.1
		[Obsolete("This member has no more usages and will be removed in a future version")]
		public const byte MaxScale = 5;
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxDateTime2")]
		public const byte MaxDateTime2 = 8;
		// Since v5.1
		[Obsolete("Use MsSql2000Dialect.MaxDateTimeOffset")]
		public const byte MaxDateTimeOffset = 10;

		private Dialect.Dialect _dialect;

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			_dialect = Dialect.Dialect.GetDialect(settings);
		}

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
			
			// Defaults size/precision/scale
			switch (dbParam.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					dbParam.Size = IsAnsiText(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForAnsiClob : MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString;
					break;
				case DbType.Binary:
					dbParam.Size = IsBlob(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForBlob : MsSql2000Dialect.MaxSizeForLengthLimitedBinary;
					break;
				case DbType.Decimal:
					if (_dialect == null)
						throw new InvalidOperationException("Dialect not available, is this driver used without having been configured?");
					dbParam.Precision = _dialect.DefaultCastPrecision;
					dbParam.Scale = _dialect.DefaultCastScale;
					break;
				case DbType.String:
				case DbType.StringFixedLength:
					dbParam.Size = IsText(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForClob : MsSql2000Dialect.MaxSizeForLengthLimitedString;
					break;
				case DbType.DateTime2:
					dbParam.Size = MsSql2000Dialect.MaxDateTime2;
					break;
				case DbType.DateTimeOffset:
					dbParam.Size = MsSql2000Dialect.MaxDateTimeOffset;
					break;
				case DbType.Xml:
					dbParam.Size = MsSql2005Dialect.MaxSizeForXml;
					break;
			}

			// Do not override the default length for string using data from SqlType, since LIKE expressions needs
			// larger columns. https://nhibernate.jira.com/browse/NH-3036

			if (sqlType.PrecisionDefined)
			{
				dbParam.Precision = sqlType.Precision;
				dbParam.Scale = sqlType.Scale;
			}
		}

		// Since v5.1
		[Obsolete("This method has no more usages and will be removed in a future version")]
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
				dbParam.Precision = sqlType.Precision;
				dbParam.Scale = sqlType.Scale;
			}
		}

		// Since v5.1
		[Obsolete("This method has no more usages and will be removed in a future version")]
		protected static void SetDefaultParameterSize(DbParameter dbParam, SqlType sqlType)
		{
			switch (dbParam.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					dbParam.Size = IsAnsiText(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForAnsiClob : MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString;
					break;
				case DbType.Binary:
					dbParam.Size = IsBlob(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForBlob : MsSql2000Dialect.MaxSizeForLengthLimitedBinary;
					break;
				case DbType.Decimal:
					dbParam.Precision = MaxPrecision;
					dbParam.Scale = MaxScale;
					break;
				case DbType.String:
				case DbType.StringFixedLength:
					dbParam.Size = IsText(dbParam, sqlType) ? MsSql2000Dialect.MaxSizeForClob : MsSql2000Dialect.MaxSizeForLengthLimitedString;
					break;
				case DbType.DateTime2:
					dbParam.Size = MsSql2000Dialect.MaxDateTime2;
					break;
				case DbType.DateTimeOffset:
					dbParam.Size = MsSql2000Dialect.MaxDateTimeOffset;
					break;
				case DbType.Xml:
					dbParam.Size = MsSql2005Dialect.MaxSizeForXml;
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
			return ((DbType.AnsiString == dbParam.DbType || DbType.AnsiStringFixedLength == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString));
		}

		/// <summary>
		/// Interprets if a parameter is a Clob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Clob, otherwise False</returns>
		protected static bool IsText(DbParameter dbParam, SqlType sqlType)
		{
			return (sqlType is StringClobSqlType) || ((DbType.String == dbParam.DbType || DbType.StringFixedLength == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedString));
		}

		/// <summary>
		/// Interprets if a parameter is a Blob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Blob, otherwise False</returns>
		protected static bool IsBlob(DbParameter dbParam, SqlType sqlType)
		{
			return (sqlType is BinaryBlobSqlType) || ((DbType.Binary == dbParam.DbType) && sqlType.LengthDefined && (sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedBinary));
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

		/// <summary>
		/// With read committed snapshot or lower, SQL Server may have not actually already committed the transaction
		/// right after the scope disposal.
		/// </summary>
		public override bool HasDelayedDistributedTransactionCompletion => true;

		/// <inheritdoc />
		public override DateTime MinDate => new DateTime(1753, 1, 1);
	}
}
