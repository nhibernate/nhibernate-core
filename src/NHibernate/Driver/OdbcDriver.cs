using System;
using System.Data;
using System.Data.Odbc;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

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
		public OdbcDriver()
		{
		}

		public override IDbConnection CreateConnection()
		{
			return new OdbcConnection();
		}

		public override IDbCommand CreateCommand()
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

		private static void SetVariableLengthParameterSize(IDbDataParameter dbParam, SqlType sqlType)
		{
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

		public static void SetParameterSizes(IDataParameterCollection parameters, SqlType[] parameterTypes)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				SetVariableLengthParameterSize((IDbDataParameter)parameters[i], parameterTypes[i]);
			}
		}

		public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			IDbCommand command = base.GenerateCommand(type, sqlString, parameterTypes);
			//if (IsPrepareSqlEnabled)
			{
				SetParameterSizes(command.Parameters, parameterTypes);
			}
			return command;
		}

	}
}