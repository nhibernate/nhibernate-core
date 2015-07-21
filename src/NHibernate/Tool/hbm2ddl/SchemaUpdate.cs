using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Cfg;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;
using NHibernate.AdoNet.Util;

namespace NHibernate.Tool.hbm2ddl
{
	public class SchemaUpdate
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (SchemaUpdate));
		private readonly Configuration configuration;
		private readonly IConnectionHelper connectionHelper;
		private readonly Dialect.Dialect dialect;
		private readonly List<Exception> exceptions;
		private IFormatter formatter;

		public SchemaUpdate(Configuration cfg) : this(cfg, cfg.Properties) {}

		public SchemaUpdate(Configuration cfg, IDictionary<string, string> configProperties)
		{
			configuration = cfg;
			dialect = Dialect.Dialect.GetDialect(configProperties);
			var props = new Dictionary<string, string>(dialect.DefaultProperties);
			foreach (var prop in configProperties)
			{
				props[prop.Key] = prop.Value;
			}
			connectionHelper = new ManagedProviderConnectionHelper(props);
			exceptions = new List<Exception>();
			formatter = (PropertiesHelper.GetBoolean(Environment.FormatSql, configProperties, true) ? FormatStyle.Ddl : FormatStyle.None).Formatter;
		}

		public SchemaUpdate(Configuration cfg, Settings settings)
		{
			configuration = cfg;
			dialect = settings.Dialect;
			connectionHelper = new SuppliedConnectionProviderConnectionHelper(settings.ConnectionProvider);
			exceptions = new List<Exception>();
			formatter = (settings.SqlStatementLogger.FormatSql ? FormatStyle.Ddl : FormatStyle.None).Formatter;
		}

		/// <summary>
		///  Returns a List of all Exceptions which occurred during the export.
		/// </summary>
		/// <returns></returns>
		public IList<Exception> Exceptions
		{
			get { return exceptions; }
		}

		public static void Main(string[] args)
		{
			try
			{
				var cfg = new Configuration();

				bool script = true;
				// If true then execute db updates, otherwise just generate and display updates
				bool doUpdate = true;
				//String propFile = null;

				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("--"))
					{
						if (args[i].Equals("--quiet"))
						{
							script = false;
						}
						else if (args[i].StartsWith("--properties="))
						{
							throw new NotSupportedException("No properties file for .NET, use app.config instead");
							//propFile = args[i].Substring( 13 );
						}
						else if (args[i].StartsWith("--config="))
						{
							cfg.Configure(args[i].Substring(9));
						}
						else if (args[i].StartsWith("--text"))
						{
							doUpdate = false;
						}
						else if (args[i].StartsWith("--naming="))
						{
							cfg.SetNamingStrategy(
								(INamingStrategy)
								Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(args[i].Substring(9))));
						}
					}
					else
					{
						cfg.AddFile(args[i]);
					}
				}

				/* NH: No props file for .NET
				 * if ( propFile != null ) {
					Hashtable props = new Hashtable();
					props.putAll( cfg.Properties );
					props.load( new FileInputStream( propFile ) );
					cfg.SetProperties( props );
				}*/

				new SchemaUpdate(cfg).Execute(script, doUpdate);
			}
			catch (Exception e)
			{
				log.Error("Error running schema update", e);
				Console.WriteLine(e);
			}
		}

		/// <summary>
		/// Execute the schema updates
		/// </summary>
		public void Execute(bool useStdOut, bool doUpdate)
		{
			if (useStdOut)
			{
				Execute(Console.WriteLine, doUpdate);
			}
			else
			{
				Execute(null, doUpdate);
			}
		}

		/// <summary>
		/// Execute the schema updates
		/// </summary>
		/// <param name="scriptAction">The action to write the each schema line.</param>
		/// <param name="doUpdate">Commit the script to DB</param>
		public void Execute(Action<string> scriptAction, bool doUpdate)
		{
			log.Info("Running hbm2ddl schema update");

			string autoKeyWordsImport = PropertiesHelper.GetString(Environment.Hbm2ddlKeyWords, configuration.Properties, "not-defined");
			autoKeyWordsImport = autoKeyWordsImport.ToLowerInvariant();
			if (autoKeyWordsImport == Hbm2DDLKeyWords.AutoQuote)
			{
				SchemaMetadataUpdater.QuoteTableAndColumns(configuration);
			}

			DbConnection connection;
			IDbCommand stmt = null;

			exceptions.Clear();

			try
			{
				DatabaseMetadata meta;
				try
				{
					log.Info("fetching database metadata");
					connectionHelper.Prepare();
					connection = connectionHelper.Connection;
					meta = new DatabaseMetadata(connection, dialect);
					stmt = connection.CreateCommand();
				}
				catch (Exception sqle)
				{
					exceptions.Add(sqle);
					log.Error("could not get database metadata", sqle);
					throw;
				}

				log.Info("updating schema");

				string[] createSQL = configuration.GenerateSchemaUpdateScript(dialect, meta);
				for (int j = 0; j < createSQL.Length; j++)
				{
					string sql = createSQL[j];
					string formatted = formatter.Format(sql);

					try
					{
						if (scriptAction != null)
						{
							scriptAction(formatted);
						}
						if (doUpdate)
						{
							log.Debug(sql);
							stmt.CommandText = sql;
							stmt.ExecuteNonQuery();
						}
					}
					catch (Exception e)
					{
						exceptions.Add(e);
						log.Error("Unsuccessful: " + sql, e);
					}
				}

				log.Info("schema update complete");
			}
			catch (Exception e)
			{
				exceptions.Add(e);
				log.Error("could not complete schema update", e);
			}
			finally
			{
				try
				{
					if (stmt != null)
					{
						stmt.Dispose();
					}
					connectionHelper.Release();
				}
				catch (Exception e)
				{
					exceptions.Add(e);
					log.Error("Error closing connection", e);
				}
			}
		}
	}
}