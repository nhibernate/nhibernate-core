using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using NHibernate.Cfg;
using log4net;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{

	public class SchemaUpdate
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (SchemaUpdate));
		private readonly IConnectionHelper connectionHelper;
		private readonly Configuration configuration;
		private readonly Dialect.Dialect dialect;
		private readonly IList exceptions;

		public SchemaUpdate(Configuration cfg)
			: this(cfg, cfg.Properties)
		{
		}

		public SchemaUpdate(Configuration cfg, IDictionary<string, string> connectionProperties)
		{
			configuration = cfg;
			dialect = NHibernate.Dialect.Dialect.GetDialect(connectionProperties);
			Dictionary<string, string> props = new Dictionary<string, string>(dialect.DefaultProperties);
			foreach (KeyValuePair<string, string> prop in connectionProperties)
			{
				props[prop.Key] = prop.Value;
			}
			connectionHelper = new ManagedProviderConnectionHelper(props);
			exceptions = new ArrayList();
		}

		public SchemaUpdate(Configuration cfg, Settings settings)
		{
			configuration = cfg;
			dialect = settings.Dialect;
			connectionHelper = new SuppliedConnectionProviderConnectionHelper(
				settings.ConnectionProvider
				);
			exceptions = new ArrayList();
		}

		public static void main(String[] args)
		{
			try
			{
				Configuration cfg = new Configuration();

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
								(INamingStrategy) Activator.CreateInstance(ReflectHelper.ClassForName(args[i].Substring(9)))
								);
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
		public void Execute(bool script, bool doUpdate)
		{
			log.Info("Running hbm2ddl schema update");

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
					connection = (DbConnection) connectionHelper.GetConnection();
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

				String[] createSQL = configuration.GenerateSchemaUpdateScript(dialect, meta);
				for (int j = 0; j < createSQL.Length; j++)
				{
					String sql = createSQL[j];
					try
					{
						if (script)
						{
							Console.WriteLine(sql);
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

		/// <summary>
		///  Returns a List of all Exceptions which occured during the export.
		/// </summary>
		/// <returns></returns>
		public IList Exceptions
		{
			get { return exceptions; }
		}
	}
}
