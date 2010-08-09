using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using NHibernate.AdoNet.Util;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

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
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof (SchemaExport));
		private bool wasInitialized;
		private readonly Configuration cfg;
		private readonly IDictionary<string, string> configProperties;
		private string[] createSQL;
		private Dialect.Dialect dialect;
		private string[] dropSQL;
		private IFormatter formatter;
		private string delimiter;
		private string outputFile;

		/// <summary>
		/// Create a schema exported for a given Configuration
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		public SchemaExport(Configuration cfg) : this(cfg, cfg.Properties) {}

		/// <summary>
		/// Create a schema exporter for the given Configuration, with the given
		/// database connection properties
		/// </summary>
		/// <param name="cfg">The NHibernate Configuration to generate the schema from.</param>
		/// <param name="configProperties">The Properties to use when connecting to the Database.</param>
		public SchemaExport(Configuration cfg, IDictionary<string, string> configProperties)
		{
			this.cfg = cfg;
			this.configProperties = configProperties;
		}

		private void Initialize()
		{
			if(wasInitialized)
			{
				return;
			}
			string autoKeyWordsImport = PropertiesHelper.GetString(Environment.Hbm2ddlKeyWords, configProperties, "not-defined");
			autoKeyWordsImport = autoKeyWordsImport.ToLowerInvariant();
			if (autoKeyWordsImport == Hbm2DDLKeyWords.AutoQuote)
			{
				SchemaMetadataUpdater.QuoteTableAndColumns(cfg);
			}

			dialect = Dialect.Dialect.GetDialect(configProperties);
			dropSQL = cfg.GenerateDropSchemaScript(dialect);
			createSQL = cfg.GenerateSchemaCreationScript(dialect);
			formatter = (PropertiesHelper.GetBoolean(Environment.FormatSql, configProperties, true) ? FormatStyle.Ddl : FormatStyle.None).Formatter;
			wasInitialized = true;
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
		/// <param name="script"><see langword="true" /> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><see langword="true" /> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool)"/> and sets
		/// the justDrop parameter to false.
		/// </remarks>
		public void Create(bool script, bool export)
		{
			Execute(script, export, false);
		}

		public void Create(Action<string> scriptAction, bool export)
		{
			Execute(scriptAction, export, false);
		}

		/// <summary>
		/// Run the drop schema script
		/// </summary>
		/// <param name="script"><see langword="true" /> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><see langword="true" /> if the ddl should be executed against the Database.</param>
		/// <remarks>
		/// This is a convenience method that calls <see cref="Execute(bool, bool, bool)"/> and sets
		/// the justDrop parameter to true.
		/// </remarks>
		public void Drop(bool script, bool export)
		{
			Execute(script, export, true);
		}

		private void Execute(Action<string> scriptAction, bool export, bool throwOnError, TextWriter exportOutput,
		                     IDbCommand statement, string sql)
		{
			Initialize();
			try
			{
				string formatted = formatter.Format(sql);

				if (delimiter != null)
				{
					formatted += delimiter;
				}
				if (scriptAction != null)
				{
					scriptAction(formatted);
				}
				log.Debug(formatted);
				if (exportOutput != null)
				{
					exportOutput.WriteLine(formatted);
				}
				if (export)
				{
                    ExecuteSql(statement, sql);
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

        private void ExecuteSql(IDbCommand cmd, string sql)
        {
            if (dialect.SupportsSqlBatches)
            {
                var objFactory = Environment.BytecodeProvider.ObjectsFactory;
                ScriptSplitter splitter = (ScriptSplitter)objFactory.CreateInstance(typeof(ScriptSplitter), sql);

                foreach (string stmt in splitter)
                {
                    log.DebugFormat("SQL Batch: {0}", stmt);
                    cmd.CommandText = stmt;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

		/// <summary>
		/// Executes the Export of the Schema in the given connection
		/// </summary>
		/// <param name="script"><see langword="true" /> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><see langword="true" /> if the ddl should be executed against the Database.</param>
		/// <param name="justDrop"><see langword="true" /> if only the ddl to drop the Database objects should be executed.</param>
		/// <param name="connection">
		/// The connection to use when executing the commands when export is <see langword="true" />.
		/// Must be an opened connection. The method doesn't close the connection.
		/// </param>
		/// <param name="exportOutput">The writer used to output the generated schema</param>
		/// <remarks>
		/// This method allows for both the drop and create ddl script to be executed.
		/// This overload is provided mainly to enable use of in memory databases. 
		/// It does NOT close the given connection!
		/// </remarks>
		public void Execute(bool script, bool export, bool justDrop, IDbConnection connection,
		                    TextWriter exportOutput)
		{
			if (script)
			{
				Execute(Console.WriteLine, export, justDrop, connection, exportOutput);
			}
			else
			{
				Execute(null, export, justDrop, connection, exportOutput);
			}
		}

		public void Execute(Action<string> scriptAction, bool export, bool justDrop, IDbConnection connection,
		                    TextWriter exportOutput)
		{
			Initialize();
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
					Execute(scriptAction, export, false, exportOutput, statement, dropSQL[i]);
				}

				if (!justDrop)
				{
					for (int j = 0; j < createSQL.Length; j++)
					{
						Execute(scriptAction, export, true, exportOutput, statement, createSQL[j]);
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
		/// <param name="script"><see langword="true" /> if the ddl should be outputted in the Console.</param>
		/// <param name="export"><see langword="true" /> if the ddl should be executed against the Database.</param>
		/// <param name="justDrop"><see langword="true" /> if only the ddl to drop the Database objects should be executed.</param>
		/// <remarks>
		/// This method allows for both the drop and create ddl script to be executed.
		/// </remarks>
		public void Execute(bool script, bool export, bool justDrop)
		{
			if (script)
			{
				Execute(Console.WriteLine, export, justDrop);
			}
			else
			{
				Execute(null, export, justDrop);
			}
		}

		public void Execute(Action<string> scriptAction, bool export, bool justDrop)
		{
			Initialize();
			IDbConnection connection = null;
			StreamWriter fileOutput = null;
			IConnectionProvider connectionProvider = null;

			var props = new Dictionary<string, string>();
			foreach (var de in dialect.DefaultProperties)
			{
				props[de.Key] = de.Value;
			}

			if (configProperties != null)
			{
				foreach (var de in configProperties)
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

				Execute(scriptAction, export, justDrop, connection, fileOutput);
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
	}
}