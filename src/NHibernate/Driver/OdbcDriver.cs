using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Odbc DataProvider
	/// </summary>
	/// <remarks>
	/// Always look for a native .NET DataProvider before using the Odbc DataProvider.
	/// </remarks>
	public class OdbcDriver 
#if NETFX
		: DriverBase
#else
		: ReflectionBasedDriver
#endif
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(OdbcDriver));

		private byte? _dbDateTimeScale;

		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			// Explicit scale for DbType.DateTime. Seems required for at least MS SQL Server 2008+.
			_dbDateTimeScale = PropertiesHelper.GetByte(Environment.OdbcDateTimeScale, settings, null);
			if (_dbDateTimeScale != null && Log.IsInfoEnabled())
			{
				Log.Info("Will use scale {0} for DbType.DateTime parameters.", _dbDateTimeScale);
			}
		}

#if !NETFX
		public OdbcDriver() 
			: base("System.Data.Odbc", "System.Data.Odbc.OdbcConnection", "System.Data.Odbc.OdbcCommand")
		{
		}
#else
		public override DbConnection CreateConnection()
		{
			return new System.Data.Odbc.OdbcConnection();
		}

		public override DbCommand CreateCommand()
		{
			return new System.Data.Odbc.OdbcCommand();
		}
#endif

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return String.Empty; }
		}

		private void SetVariableLengthParameterSize(DbParameter dbParam, SqlType sqlType)
		{
			if (sqlType is DateTimeSqlType && _dbDateTimeScale != null)
				dbParam.Scale = _dbDateTimeScale.Value;

			if (sqlType.LengthDefined)
			{
				switch (dbParam.DbType)
				{
					case DbType.StringFixedLength:
					case DbType.AnsiStringFixedLength:
						// For types that are using one character (CharType, AnsiCharType, TrueFalseType, YesNoType and EnumCharType),
						// we have to specify the length otherwise sql function like charindex won't work as expected.
						if (sqlType.Length == 1)
						{
							dbParam.Size = sqlType.Length;
						}

						break;
					case DbType.String:
					case DbType.AnsiString:
						// NH-4083: do not limit to column length if above 2000. Setting size may trigger conversion from
						// nvarchar to ntext when size is superior or equal to 2000, causing some queries to fail:
						// https://stackoverflow.com/q/8569844/1178314
						// So we cannot do as the SqlServerClientDriver which set max default length instead.
						// This may also cause NH-3895, forbidding like comparisons which may need
						// some more length.
						// Moreover specifying size is a SQL Server optimization for query
						// plan cache, but we have no knowledge here if the target database will be SQL-Server.
						break;
					default:
						dbParam.Size = sqlType.Length;
						break;
				}
			}

			if (sqlType.PrecisionDefined)
			{
				dbParam.Precision = sqlType.Precision;
				dbParam.Scale = sqlType.Scale;
			}
		}

		protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
		{
			base.InitializeParameter(dbParam, name, sqlType);
			SetVariableLengthParameterSize(dbParam, sqlType);
		}

		public override bool RequiresTimeSpanForTime => true;

		/// <summary>
		/// Depends on target DB in the Odbc case. This in facts depends on both the driver and the database.
		/// </summary>
		public override bool HasDelayedDistributedTransactionCompletion => true;

		/// <inheritdoc />
		public override DateTime MinDate => new DateTime(1753, 1, 1);
	}
}
