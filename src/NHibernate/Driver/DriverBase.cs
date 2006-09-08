using System;
using System.Data;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;
using NHibernate.SqlTypes;

namespace NHibernate.Driver
{
	/// <summary>
	/// Base class for the implementation of IDriver
	/// </summary>
	public abstract class DriverBase : IDriver
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(DriverBase));

		public abstract IDbConnection CreateConnection();
		public abstract IDbCommand CreateCommand();

		public abstract bool UseNamedPrefixInSql { get; }
		public abstract bool UseNamedPrefixInParameter { get; }
		public abstract string NamedPrefix { get; }

		public string FormatNameForSql(string parameterName)
		{
			return UseNamedPrefixInSql ? (NamedPrefix + parameterName) : StringHelper.SqlParameter;
		}

		public string FormatNameForParameter(string parameterName)
		{
			return UseNamedPrefixInParameter ? (NamedPrefix + parameterName) : parameterName;
		}

		public virtual bool SupportsMultipleOpenReaders
		{
			get { return true; }
		}

		public virtual bool SupportsPreparingCommands
		{
			get { return true; }
		}

		public virtual IDbCommand GenerateCommand(CommandType type, SqlString sqlString)
		{
			IDbCommand cmd = CreateCommand();
			cmd.CommandType = type;

			SetCommandTimeout(cmd, Environment.Properties[Environment.CommandTimeout]);
			SetCommandText(cmd, sqlString);
			SetCommandParameters(cmd, sqlString.ParameterTypes);

			return cmd;
		}
		
		private static void SetCommandTimeout(IDbCommand cmd, object envTimeout)
		{
			if (envTimeout != null)
			{
				int timeout = Convert.ToInt32(envTimeout);
				if (timeout >= 0)
				{
					if (log.IsDebugEnabled)
					{
						log.Debug(string.Format("setting ADO Command timeout to '{0}' seconds", timeout));
					}
					try
					{
						cmd.CommandTimeout = timeout;
					}
					catch (Exception e)
					{
						if (log.IsWarnEnabled)
						{
							log.Warn(e.ToString());
						}
					}
				}
				else
				{
					log.Error("Invalid timeout of '" + envTimeout + "' specified, ignoring");
				}
			}
		}
		
		private static string ToParameterName(int index)
		{
			return "p" + index;
		}
		
		private void SetCommandText(IDbCommand cmd, SqlString sqlString)
		{
			int paramIndex = 0;
			StringBuilder builder = new StringBuilder(sqlString.Count * 15);
			foreach (object part in sqlString.SqlParts)
			{
				Parameter parameter = part as Parameter;

				if (parameter != null)
				{
					string paramName = ToParameterName(paramIndex);
					builder.Append(FormatNameForSql(paramName));
					paramIndex++;
				}
				else
				{
					builder.Append((string) part);
				}
			}

			cmd.CommandText = builder.ToString();
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

		/// <summary>
		/// Prepare the <paramref name="command" />. Will only be called if <see cref="SupportsPreparingCommands" />
		/// is <c>true</c>.
		/// </summary>
		/// <remarks>
		/// Drivers that require Size or Precision/Scale to be set before the IDbCommand is prepared should 
		/// override this method and use the info contained in <paramref name="parameterTypes" /> to set those
		/// values.  By default those values are not set, only the DbType and ParameterName are set.
		/// </remarks>
		public virtual void PrepareCommand(IDbCommand command, SqlType[] parameterTypes)
		{
			command.Prepare();
		}
	}
}