using System;
using System.Data;
using System.Text;


namespace NHibernate.AdoNet.Util
{
	/// <summary> Centralize logging handling for SQL statements. </summary>
	public class SqlStatementLogger
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor("NHibernate.SQL");

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
					output.Append(string.Format("{0} = {1} [Type: {2}]", p.ParameterName, GetParameterLogableValue(p), GetParameterLogableType(p)));
				}
				outputText = output.ToString();
			}
			return outputText;
		}

	    private static string GetParameterLogableType(IDataParameter dataParameter)
	    {
            var p = dataParameter as IDbDataParameter;
            if (p != null)
                return p.DbType + " (" + p.Size + ")";
	        return p.DbType.ToString();

	    }

			public string GetParameterLogableValue(IDataParameter parameter)
			{
				const int maxLogableStringLength = 1000;
				if (parameter.Value == null || DBNull.Value.Equals(parameter.Value))
				{
					return "NULL";
				}
				if (IsStringType(parameter.DbType))
				{
					return string.Concat("'", TruncateWithEllipsis(parameter.Value.ToString(), maxLogableStringLength), "'");
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
			const int maxBytes = 128;
			int bufferLength = buffer.Length;

			var sb = new StringBuilder(maxBytes * 2 + 8);
			sb.Append("0x");
			for (int i = 0; i < bufferLength && i < maxBytes; i++)
			{
				sb.Append(buffer[i].ToString("X2"));
			}
			if(bufferLength > maxBytes)
			{
				sb.Append("...");
			}
			return sb.ToString();
		}

		private static bool IsStringType(DbType dbType)
		{
			return DbType.String.Equals(dbType) || DbType.AnsiString.Equals(dbType)
						 || DbType.AnsiStringFixedLength.Equals(dbType) || DbType.StringFixedLength.Equals(dbType);
		}

		public FormatStyle DetermineActualStyle(FormatStyle style)
		{
			return FormatSql ? style : FormatStyle.None;
		}

		public void LogBatchCommand(string batchCommand)
		{
			log.Debug(batchCommand);
			if(LogToStdout)
			{
				Console.Out.WriteLine("NHibernate: " + batchCommand);
			}
		}

		private string TruncateWithEllipsis(string source, int length)
		{
			const string ellipsis = "...";
			if (source.Length > length)
			{
				return source.Substring(0, length) + ellipsis;
			}
			return source;
		}
	}
}