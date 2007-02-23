using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using log4net;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{
	/// <summary>
	/// Generates ddl to export table schema for a configured <c>Configuration</c> to the database
	/// </summary>
	/// <remarks>
	/// This Class can be used directly or the command line wrapper NHibernate.Tool.hbm2ddl.exe can be
	/// used when a dll can not be directly used.
	/// </remarks>
	public class SchemaExport
	{
		private string[] dropSQL;
		private string[] createSQL;
		private IDictionary connectionProperties;
		private string outputFile = null;
		private Dialect.Dialect dialect;
		private string delimiter = null;

		private static readonly ILog log = LogManager.GetLogger(typeof(SchemaExport));

		/// <summary>
		/// Create a schema exported for a given Configuration
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		public SchemaExport(Configuration cfg)
			: this(cfg, cfg.Properties)
		{
		}

		/// <summary>
		/// Create a schema exporter for the given Configuration, with the given
		/// database connection properties
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		/// <param name="connectionProperties">The Properties to use when connecting to the Database.</param>
		public SchemaExport(Configuration cfg, IDictionary connectionProperties)
		{
			this.connectionProperties = connectionProperties;
			dialect = Dialect.Dialect.GetDialect(connectionProperties);
			dropSQL = cfg.GenerateDropSchemaScript(dialect);
			createSQL = cfg.GenerateSchemaCreationScript(dialect);
		}

		/// <summary>
		/// Set the output filename. The generated script will be written to this file
		/// </summary>
		/// <param name="filename">The name of the file to output the ddl to.</param>
		/// <returns>The SchemaExport object.</returns>
		public SchemaExport SetOutputFile(string filename)
		{
			outputFile = filename;
			return this;
		}

		/// <summary>
		/// Set the end of statement delimiter 
		/// </summary>
		/// <param name="delimiter">The end of statement delimiter.</param>
		/// <returns>The SchemaExport object.</returns>
		public SchemaExport SetDelimiter(string delimiter)
		{
			this.delimiter = delimiter;
			return this;
		}

		/// <summary>
		/// Run the schema creation script
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool, bool)"/> and sets
		/// the justDrop parameter to false and the format parameter to true.
		/// </remarks>
		public void Create(bool script, bool export)
		{
			Execute(script, export, false, true);
		}

		/// <summary>
		/// Run the drop schema script
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool, bool)"/> and sets
		/// the justDrop and format parameter to true.
		/// </remarks>
		public void Drop(bool script, bool export)
		{
			Execute(script, export, true, true);
		}

		private void Execute(bool script, bool export, bool format, bool throwOnError, TextWriter exportOutput,
		                     IDbCommand statement, string sql)
		{
			try
			{
				string formatted;
				if (format)
				{
					formatted = Format(sql);
				}
				else
				{
					formatted = sql;
				}

				if (delimiter != null)
				{
					formatted += delimiter;
				}
				if (script)
				{
					Console.WriteLine(formatted);
				}
				log.Debug(formatted);
				if (exportOutput != null)
				{
					exportOutput.WriteLine(formatted);
				}
				if (export)
				{
					statement.CommandText = sql;
					statement.CommandType = CommandType.Text;
					statement.ExecuteNonQuery();
				}
			}
			catch (Exception e)
			{
				log.Warn("Unsuccessful: " + sql);
				log.Warn(e.Message);
				if (throwOnError)
				{
					throw;
				}
			}
		}

		/// <summary>
		/// Executes the Export of the Schema in the given connection
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <param name="justDrop"><c>true</c> if only the ddl to drop the Database objects should be executed.</param>
		/// <param name="format"><c>true</c> if the ddl should be nicely formatted instead of one statement per line.</param>
		/// <param name="connection">
		/// The connection to use when executing the commands when export is <c>true</c>.
		/// Must be an opened connection. The method doesn't close the connection.
		/// </param>
		/// <param name="exportOutput">The writer used to output the generated schema</param>
		/// <remarks>
		/// This method allows for both the drop and create ddl script to be executed.
		/// This overload is provided mainly to enable use of in memory databases. 
		/// It does NOT close the given connection!
		/// </remarks>
		public void Execute(bool script, bool export, bool justDrop, bool format,
		                    IDbConnection connection, TextWriter exportOutput)
		{
			IDbCommand statement = null;

			if (export && connection == null)
			{
				throw new ArgumentNullException("connection", "When export is set to true, you need to pass a non null connection");
			}
			if (export)
			{
				statement = connection.CreateCommand();
			}

			try
			{
				for (int i = 0; i < dropSQL.Length; i++)
				{
					Execute(script, export, format, false, exportOutput, statement, dropSQL[i]);
				}

				if (!justDrop)
				{
					for (int j = 0; j < createSQL.Length; j++)
					{
						Execute(script, export, format, true, exportOutput, statement, createSQL[j]);
					}
				}
			}
			finally
			{
				try
				{
					if (statement != null)
					{
						statement.Dispose();
					}
				}
				catch (Exception e)
				{
					log.Error("Could not close connection: " + e.Message, e);
				}
				if (exportOutput != null)
				{
					try
					{
						exportOutput.Close();
					}
					catch (Exception ioe)
					{
						log.Error("Error closing output file " + outputFile + ": " + ioe.Message, ioe);
					}
				}
			}
		}

		/// <summary>
		/// Executes the Export of the Schema.
		/// </summary>
		/// <param name="script"><c>true</c> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><c>true</c> if the ddl should be executed against the Database.</param>
		/// <param name="justDrop"><c>true</c> if only the ddl to drop the Database objects should be executed.</param>
		/// <param name="format"><c>true</c> if the ddl should be nicely formatted instead of one statement per line.</param>
		/// <remarks>
		/// This method allows for both the drop and create ddl script to be executed.
		/// </remarks>
		public void Execute(bool script, bool export, bool justDrop, bool format)
		{
			IDbConnection connection = null;
			StreamWriter fileOutput = null;
			IConnectionProvider connectionProvider = null;

			IDictionary props = new Hashtable();
			foreach (DictionaryEntry de in dialect.DefaultProperties)
			{
				props[de.Key] = de.Value;
			}

			if (connectionProperties != null)
			{
				foreach (DictionaryEntry de in connectionProperties)
				{
					props[de.Key] = de.Value;
				}
			}

			try
			{
				if (outputFile != null)
				{
					fileOutput = new StreamWriter(outputFile);
				}

				if (export)
				{
					connectionProvider = ConnectionProviderFactory.NewConnectionProvider(props);
					connection = connectionProvider.GetConnection();
				}

				Execute(script, export, justDrop, format, connection, fileOutput);
			}
			catch (HibernateException)
			{
				// So that we don't wrap HibernateExceptions in HibernateExceptions
				throw;
			}
			catch (Exception e)
			{
				log.Error(e.Message, e);
				throw new HibernateException(e.Message, e);
			}
			finally
			{
				if (connection != null)
				{
					connectionProvider.CloseConnection(connection);
					connectionProvider.Dispose();
				}
			}
		}

		/// <summary>
		/// Format an SQL statement using simple rules
		/// </summary>
		/// <param name="sql">The string containing the sql to format.</param>
		/// <returns>A string that contains formatted sql.</returns>
		/// <remarks>
		/// The simple rules to used when formatting are:
		/// <list type="number">
		///		<item>
		///			<description>Insert a newline after each comma</description>
		///		</item>
		///		<item>
		///			<description>Indent three spaces after each inserted newline</description>
		///		</item>
		///		<item>
		///			<description>
		///			If the statement contains single/double quotes return unchanged because
		///			it is too complex and could be broken by simple formatting.
		///			</description>
		///		</item>
		/// </list>
		/// </remarks>
		private static string Format(string sql)
		{
			if (sql.IndexOf("\"") > 0 || sql.IndexOf("'") > 0)
			{
				return sql;
			}

			string formatted;

			if (StringHelper.StartsWithCaseInsensitive(sql, "create table"))
			{
				StringBuilder result = new StringBuilder(60);
				StringTokenizer tokens = new StringTokenizer(sql, "(,)", true);

				int depth = 0;

				foreach (string tok in tokens)
				{
					if (StringHelper.ClosedParen.Equals(tok))
					{
						depth--;
						if (depth == 0)
						{
							result.Append("\n");
						}
					}
					result.Append(tok);
					if (StringHelper.Comma.Equals(tok) && depth == 1)
					{
						result.Append("\n  ");
					}
					if (StringHelper.OpenParen.Equals(tok))
					{
						depth++;
						if (depth == 1)
						{
							result.Append("\n  ");
						}
					}
				}

				formatted = result.ToString();
			}
			else
			{
				formatted = sql;
			}

			return formatted;
		}
	}
}