using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.AdoNet;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Driver
{
	/// <summary>
	///     A NHibernate Driver for using the SqlClient DataProvider
	/// </summary>
	public class MicrosoftDataSqlClientDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider, IParameterAdjuster
	{
		const byte MaxTime = 5;

		private static readonly Action<object, SqlDbType> SetSqlDbType = DelegateHelper.BuildPropertySetter<SqlDbType>(System.Type.GetType("Microsoft.Data.SqlClient.SqlParameter, Microsoft.Data.SqlClient", true), "SqlDbType");

		private Dialect.Dialect _dialect;

		public MicrosoftDataSqlClientDriver()
			: base(
				"Microsoft.Data.SqlClient",
				"Microsoft.Data.SqlClient.SqlConnection",
				"Microsoft.Data.SqlClient.SqlCommand")
		{
		}

		/// <summary>
		///     MsSql requires the use of a Named Prefix in the SQL statement.
		/// </summary>
		/// <remarks>
		///     <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInSql => true;

		/// <summary>
		///     MsSql requires the use of a Named Prefix in the Parameter.
		/// </summary>
		/// <remarks>
		///     <see langword="true" /> because MsSql uses "<c>@</c>".
		/// </remarks>
		public override bool UseNamedPrefixInParameter => true;

		/// <summary>
		///     The Named Prefix for parameters.
		/// </summary>
		/// <value>
		///     Sql Server uses <c>"@"</c>.
		/// </value>
		public override string NamedPrefix => "@";

		/// <inheritdoc/>
		public override bool SupportsMultipleOpenReaders => false;

		/// <inheritdoc />
		public override bool SupportsMultipleQueries => true;

		/// <summary>
		///     With read committed snapshot or lower, SQL Server may have not actually already committed the transaction
		///     right after the scope disposal.
		/// </summary>
		public override bool HasDelayedDistributedTransactionCompletion => true;

		public override bool RequiresTimeSpanForTime => true;

		/// <inheritdoc />
		public override DateTime MinDate => DateTime.MinValue;

		System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass => typeof(GenericBatchingBatcherFactory);

		/// <inheritdoc />
		public virtual void AdjustParameterForValue(DbParameter parameter, SqlType sqlType, object value)
		{
			if (value is string stringVal)
				switch (parameter.DbType)
				{
					case DbType.AnsiString:
					case DbType.AnsiStringFixedLength:
						parameter.Size = IsAnsiText(parameter, sqlType)
							? MsSql2000Dialect.MaxSizeForAnsiClob
							: Math.Max(stringVal.Length, sqlType.LengthDefined ? sqlType.Length : parameter.Size);
						break;
					case DbType.String:
					case DbType.StringFixedLength:
						parameter.Size = IsText(parameter, sqlType)
							? MsSql2000Dialect.MaxSizeForClob
							: Math.Max(stringVal.Length, sqlType.LengthDefined ? sqlType.Length : parameter.Size);
						break;
				}
		}

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			_dialect = Dialect.Dialect.GetDialect(settings);
		}

		/// <inheritdoc />
		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);

			// Defaults size/precision/scale
			switch (dbParam.DbType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
					dbParam.Size = IsAnsiText(dbParam, sqlType)
						? MsSql2000Dialect.MaxSizeForAnsiClob
						: MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString;
					break;
				case DbType.Binary:
					dbParam.Size = IsBlob(dbParam, sqlType)
						? MsSql2000Dialect.MaxSizeForBlob
						: MsSql2000Dialect.MaxSizeForLengthLimitedBinary;
					break;
				case DbType.Decimal:
					if (_dialect == null)
						throw new InvalidOperationException(
							"Dialect not available, is this driver used without having been configured?");
					dbParam.Precision = _dialect.DefaultCastPrecision;
					dbParam.Scale = _dialect.DefaultCastScale;
					break;
				case DbType.String:
				case DbType.StringFixedLength:
					dbParam.Size = IsText(dbParam, sqlType)
						? MsSql2000Dialect.MaxSizeForClob
						: MsSql2000Dialect.MaxSizeForLengthLimitedString;
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
			switch (sqlType.DbType)
			{
				case DbType.Time:
					SetSqlDbType(dbParam, SqlDbType.Time);
					dbParam.Size = MaxTime;
					break;
				case DbType.Date:
					SetSqlDbType(dbParam, SqlDbType.Date);
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

		/// <summary>
		///     Interprets if a parameter is a Clob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Clob, otherwise False</returns>
		protected static bool IsAnsiText(DbParameter dbParam, SqlType sqlType)
		{
			return (DbType.AnsiString == dbParam.DbType || DbType.AnsiStringFixedLength == dbParam.DbType) &&
			       sqlType.LengthDefined && sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedAnsiString;
		}

		/// <summary>
		///     Interprets if a parameter is a Clob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Clob, otherwise False</returns>
		protected static bool IsText(DbParameter dbParam, SqlType sqlType)
		{
			return sqlType is StringClobSqlType ||
			       (DbType.String == dbParam.DbType || DbType.StringFixedLength == dbParam.DbType) &&
			       sqlType.LengthDefined && sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedString;
		}

		/// <summary>
		///     Interprets if a parameter is a Blob (for the purposes of setting its default size)
		/// </summary>
		/// <param name="dbParam">The parameter</param>
		/// <param name="sqlType">The <see cref="SqlType" /> of the parameter</param>
		/// <returns>True, if the parameter should be interpreted as a Blob, otherwise False</returns>
		protected static bool IsBlob(DbParameter dbParam, SqlType sqlType)
		{
			return sqlType is BinaryBlobSqlType || DbType.Binary == dbParam.DbType && sqlType.LengthDefined &&
			       sqlType.Length > MsSql2000Dialect.MaxSizeForLengthLimitedBinary;
		}

		/// <inheritdoc />
		public override IResultSetsCommand GetResultSetsCommand(ISessionImplementor session)
		{
			return new BasicResultSetsCommand(session);
		}
	}
}
