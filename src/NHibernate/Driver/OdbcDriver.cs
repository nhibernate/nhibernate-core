using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
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
	public class OdbcDriver : DriverBase
	{
		private static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(OdbcDriver));

		private byte? _dbDateTimeScale;


		public override void Configure(IDictionary<string, string> settings)
		{
			base.Configure(settings);

			// Explicit scale for DbType.DateTime. Seems required for at least MS SQL Server 2008+.
			_dbDateTimeScale = PropertiesHelper.GetByte(Environment.OdbcDateTimeScale, settings, null);
			if (_dbDateTimeScale != null && Log.IsInfoEnabled)
			{
				Log.Info(string.Format("Will use scale {0} for DbType.DateTime parameters.", _dbDateTimeScale));
			}
		}

		public override DbConnection CreateConnection()
		{
			return new OdbcConnection();
		}

		public override DbCommand CreateCommand()
		{
			return new OdbcCommand();
		}

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
			if (Equals(sqlType, SqlTypeFactory.DateTime) && _dbDateTimeScale != null)
				dbParam.Scale = _dbDateTimeScale.Value;

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
