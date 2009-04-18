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
		public SqlStatementLogger() : this(false, false) {}

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

		/// <summary> Log a SQL statement string. </summary>
		/// <param name="statement">The SQL statement. </param>
		/// <param name="style">The requested formatting style. </param>
		public virtual void LogStatement(string statement, FormatStyle style)
		{
			if (!log.IsDebugEnabled && !LogToStdout)
			{
				return;
			}
			style = DetermineActualStyle(style);
			statement = style.Formatter.Format(statement);
			log.Debug(statement);
			if (LogToStdout)
			{
				Console.Out.WriteLine("NHibernate: " + statement);
			}
		}

		public virtual void LogInfo(string info)
		{
			log.Debug(info);
		}

		/// <summary> Log a IDbCommand. </summary>
		/// <param name="command">The SQL statement. </param>
		/// <param name="style">The requested formatting style. </param>
		public virtual string LogCommand(IDbCommand command, FormatStyle style)
		{
			if (log.IsDebugEnabled || LogToStdout)
			{
				style = DetermineActualStyle(style);
				string statement = style.Formatter.Format(GetCommandLineWithParameters(command));
				log.Debug(statement);
				if (LogToStdout)
				{
					Console.Out.WriteLine("NHibernate: " + statement);
				}
				return statement;
			}
			return null;
		}

		protected string GetCommandLineWithParameters(IDbCommand command)
		{
			string outputText;

			if (command.Parameters.Count == 0)
			{
				outputText = command.CommandText;
			}
			else
			{
				var output = new StringBuilder(command.CommandText.Length + (command.Parameters.Count * 20));
				output.Append(command.CommandText);
				output.Append("; ");

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
					output.Append(string.Format("{0} = '{1}'", p.ParameterName, p.Value));
				}
				outputText = output.ToString();
			}
			return outputText;
		}

		private FormatStyle DetermineActualStyle(FormatStyle style)
		{
			return FormatSql ? style : FormatStyle.None;
		}
	}
}