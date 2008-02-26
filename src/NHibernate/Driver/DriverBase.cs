using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Driver
{
	/// <summary>
	/// Base class for the implementation of IDriver
	/// </summary>
	public abstract class DriverBase : IDriver, ISqlParameterFormatter
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DriverBase));

		private int commandTimeout;
		private bool prepareSql;

		public virtual void Configure(IDictionary<string, string> settings)
		{
			// Command timeout
			commandTimeout = PropertiesHelper.GetInt32(Environment.CommandTimeout, settings, -1);
			if (commandTimeout > -1 && log.IsInfoEnabled)
			{
				log.Info(string.Format("setting ADO.NET command timeout to {0} seconds", commandTimeout));
			}

			// Prepare SQL
			prepareSql = PropertiesHelper.GetBoolean(Environment.PrepareSql, settings, false);
			if (prepareSql && SupportsPreparingCommands)
			{
				log.Info("preparing SQL enabled");
			}
		}

		protected bool IsPrepareSqlEnabled
		{
			get { return prepareSql; }
		}

		public abstract IDbConnection CreateConnection();
		public abstract IDbCommand CreateCommand();

		/// <summary>
		/// Does this Driver require the use of a Named Prefix in the SQL statement.  
		/// </summary>
		/// <remarks>
		/// For example, SqlClient requires <c>select * from simple where simple_id = @simple_id</c>
		/// If this is false, like with the OleDb provider, then it is assumed that  
		/// the <c>?</c> can be a placeholder for the parameter in the SQL statement.
		/// </remarks>
		public abstract bool UseNamedPrefixInSql { get; }

		/// <summary>
		/// Does this Driver require the use of the Named Prefix when trying
		/// to reference the Parameter in the Command's Parameter collection.  
		/// </summary>
		/// <remarks>
		/// This is really only useful when the UseNamedPrefixInSql == true.  When this is true the
		/// code will look like:
		/// <code>IDbParameter param = cmd.Parameters["@paramName"]</code>
		/// if this is false the code will be 
		/// <code>IDbParameter param = cmd.Parameters["paramName"]</code>.
		/// </remarks>
		public abstract bool UseNamedPrefixInParameter { get; }

		/// <summary>
		/// The Named Prefix for parameters.  
		/// </summary>
		/// <remarks>
		/// Sql Server uses <c>"@"</c> and Oracle uses <c>":"</c>.
		/// </remarks>
		public abstract string NamedPrefix { get; }

		/// <summary>
		/// Change the parameterName into the correct format IDbCommand.CommandText
		/// for the ConnectionProvider
		/// </summary>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbCommand.CommandText</returns>
		public string FormatNameForSql(string parameterName)
		{
			return UseNamedPrefixInSql ? (NamedPrefix + parameterName) : StringHelper.SqlParameter;
		}

		/// <summary>
		/// Changes the parameterName into the correct format for an IDbParameter
		/// for the Driver.
		/// </summary>
		/// <remarks>
		/// For SqlServerConnectionProvider it will change <c>id</c> to <c>@id</c>
		/// </remarks>
		/// <param name="parameterName">The unformatted name of the parameter</param>
		/// <returns>A parameter formatted for an IDbParameter.</returns>
		public string FormatNameForParameter(string parameterName)
		{
			return UseNamedPrefixInParameter ? (NamedPrefix + parameterName) : parameterName;
		}

		public virtual bool SupportsMultipleOpenReaders
		{
			get { return true; }
		}

		/// <summary>
		/// Does this Driver support IDbCommand.Prepare().
		/// </summary>
		/// <remarks>
		/// <para>
		/// A value of <see langword="false" /> indicates that an exception would be thrown or the 
		/// company that produces the Driver we are wrapping does not recommend using
		/// IDbCommand.Prepare().
		/// </para>
		/// <para>
		/// A value of <see langword="true" /> indicates that calling IDbCommand.Prepare() will function
		/// fine on this Driver.
		/// </para>
		/// </remarks>
		protected virtual bool SupportsPreparingCommands
		{
			get { return true; }
		}

		public virtual IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			IDbCommand cmd = CreateCommand();
			cmd.CommandType = type;

			SetCommandTimeout(cmd, PropertiesHelper.GetInt32(Environment.CommandTimeout, Environment.Properties, -1));
			SetCommandText(cmd, sqlString);
			SetCommandParameters(cmd, parameterTypes);

			return cmd;
		}

		private void SetCommandTimeout(IDbCommand cmd, object envTimeout)
		{
			if (commandTimeout >= 0)
			{
				try
				{
					cmd.CommandTimeout = commandTimeout;
				}
				catch (Exception e)
				{
					if (log.IsWarnEnabled)
					{
						log.Warn(e.ToString());
					}
				}
			}
		}

		private static string ToParameterName(int index)
		{
			return "p" + index;
		}

		string ISqlParameterFormatter.GetParameterName(int index)
		{
			return FormatNameForSql(ToParameterName(index));
		}

		private void SetCommandText(IDbCommand cmd, SqlString sqlString)
		{
			SqlStringFormatter formatter = new SqlStringFormatter(this);
			formatter.Format(sqlString);
			cmd.CommandText = formatter.GetFormattedText();
		}

		private void SetCommandParameters(IDbCommand cmd, SqlType[] sqlTypes)
		{
			for (int i = 0; i < sqlTypes.Length; i++)
			{
				string paramName = ToParameterName(i);
				IDbDataParameter dbParam = GenerateParameter(cmd, paramName, sqlTypes[i]);
				cmd.Parameters.Add(dbParam);
			}
		}

		protected virtual void InitializeParameter(IDbDataParameter dbParam, string name, SqlType sqlType)
		{
			if (sqlType == null)
			{
				throw new QueryException(String.Format("No type assigned to parameter '{0}'", name));
			}

			dbParam.ParameterName = FormatNameForParameter(name);
			dbParam.DbType = sqlType.DbType;
		}

		/// <summary>
		/// Generates an IDbDataParameter for the IDbCommand.  It does not add the IDbDataParameter to the IDbCommand's
		/// Parameter collection.
		/// </summary>
		/// <param name="command">The IDbCommand to use to create the IDbDataParameter.</param>
		/// <param name="name">The name to set for IDbDataParameter.Name</param>
		/// <param name="sqlType">The SqlType to set for IDbDataParameter.</param>
		/// <returns>An IDbDataParameter ready to be added to an IDbCommand.</returns>
		protected IDbDataParameter GenerateParameter(IDbCommand command, string name, SqlType sqlType)
		{
			IDbDataParameter dbParam = command.CreateParameter();
			InitializeParameter(dbParam, name, sqlType);

			return dbParam;
		}

		public void PrepareCommand(IDbCommand command)
		{
			if (SupportsPreparingCommands && prepareSql)
			{
				command.Prepare();
			}
		}

		public IDbDataParameter GenerateOutputParameter(IDbCommand command)
		{
			IDbDataParameter param = GenerateParameter(command, "ReturnValue", SqlTypeFactory.Int32);
			param.Direction = ParameterDirection.Output;
			return param;
		}

		public virtual bool SupportsMultipleQueries
		{
			get { return false; }
		}
	}
}