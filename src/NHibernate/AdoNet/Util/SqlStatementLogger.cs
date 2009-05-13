using System;
using System.Data;
using System.Text;
using log4net;

namespace NHibernate.AdoNet.Util
{
	/// <summary> Centralize logging handling for SQL statements. </summary>
	public class SqlStatementLogger
	{
		private static readonly ILog log = LogManager.GetLogger("NHibernate.SQL");

		/// <summary> Constructs a new SqlStatementLogger instance.</summary>
		public SqlStatementLogger() : this(false, false) { }

		/// <summary> Constructs a new SqlStatementLogger instance. </summary>
		/// <param name="logToStdout">Should we log to STDOUT in addition to our internal logger. </param>
		/// <param name="formatSql">Should we format SQL ('prettify') prior to logging. </param>
		public SqlStatementLogger(bool logToStdout, bool formatSql)
		{
			LogToStdout = logToStdout;
			FormatSql = formatSql;
		}

		public bool LogToStdout { get; set; }

		public bool FormatSql { get; set; }

		public bool IsDebugEnabled
		{
			get { return log.IsDebugEnabled; }
		}

		/// <summary> Log a IDbCommand. </summary>
		/// <param name="message">Title</param>
		/// <param name="command">The SQL statement. </param>
		/// <param name="style">The requested formatting style. </param>
		public virtual void LogCommand(string message, IDbCommand command, FormatStyle style)
		{
			if (!log.IsDebugEnabled && !LogToStdout || string.IsNullOrEmpty(command.CommandText))
			{
				return;
			}

			style = DetermineActualStyle(style);
			string statement = style.Formatter.Format(GetCommandLineWithParameters(command));
			string logMessage;
			if (string.IsNullOrEmpty(message))
			{
				logMessage = statement;
			}
			else
			{
				logMessage = message + statement;
			}
			log.Debug(logMessage);
			if (LogToStdout)
			{
				Console.Out.WriteLine("NHibernate: " + statement);
			}
		}

		/// <summary> Log a IDbCommand. </summary>
		/// <param name="command">The SQL statement. </param>
		/// <param name="style">The requested formatting style. </param>
		public virtual void LogCommand(IDbCommand command, FormatStyle style)
		{
			LogCommand(null, command, style);
		}

		public string GetCommandLineWithParameters(IDbCommand command)
		{
			string outputText;

			if (command.Parameters.Count == 0)
			{
				outputText = command.CommandText;
			}
			else
			{
				var output = new StringBuilder(command.CommandText.Length + (command.Parameters.Count * 20));
				output.Append(command.CommandText.TrimEnd(' ', ';', '\n'));
				output.Append(";");

				IDataParameter p;
				int count = command.Parameters.Count;
				bool appendComma = false;
				for (int i = 0; i < count; i++)
				{
					if (appendComma)
					{
						output.Append(", ");
					}
					appendComma = true;
					p = (IDataParameter)command.Parameters[i];
					output.Append(string.Format("{0} = {1}", p.ParameterName, GetParameterLogableValue(p)));
				}
				outputText = output.ToString();
			}
			return outputText;
		}

		public string GetParameterLogableValue(IDataParameter parameter)
		{
			if (parameter.Value == null || DBNull.Value.Equals(parameter.Value))
			{
				return "NULL";
			}
			if (IsStringType(parameter.DbType))
			{
				return string.Concat("'", parameter.Value.ToString(), "'");
			}
			var buffer = parameter.Value as byte[];
			if (buffer != null)
			{
				return GetBufferAsHexString(buffer);
			}
			return parameter.Value.ToString();
		}

		private static string GetBufferAsHexString(byte[] buffer)
		{
			var sb = new StringBuilder(buffer.Length * 2 + 2);
			sb.Append("0x");
			foreach (var b in buffer)
			{
				sb.Append(b.ToString("X2"));
			}
			return sb.ToString();
		}

		private static bool IsStringType(DbType dbType)
		{
			return DbType.String.Equals(dbType) || DbType.AnsiString.Equals(dbType)
						 || DbType.AnsiStringFixedLength.Equals(dbType) || DbType.StringFixedLength.Equals(dbType);
		}

		private FormatStyle DetermineActualStyle(FormatStyle style)
		{
			return FormatSql ? style : FormatStyle.None;
		}
	}
}