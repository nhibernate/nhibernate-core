using System;
using System.Collections.Generic;
using System.Data.Common;

using NHibernate.Cfg;
using NHibernate.Util;

namespace NHibernate.Tool.hbm2ddl
{
	public class SchemaValidator
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof (SchemaValidator));
		private readonly Configuration configuration;
		private readonly IConnectionHelper connectionHelper;
		private readonly Dialect.Dialect dialect;

		public SchemaValidator(Configuration cfg) : this(cfg, cfg.Properties) {}

		public SchemaValidator(Configuration cfg, IDictionary<string, string> connectionProperties)
		{
			configuration = cfg;
			dialect = Dialect.Dialect.GetDialect(connectionProperties);
			IDictionary<string, string> props = new Dictionary<string, string>(dialect.DefaultProperties);
			foreach (var prop in connectionProperties)
			{
				props[prop.Key] = prop.Value;
			}
			connectionHelper = new ManagedProviderConnectionHelper(props);
		}

		public SchemaValidator(Configuration cfg, Settings settings)
		{
			configuration = cfg;
			dialect = settings.Dialect;
			connectionHelper = new SuppliedConnectionProviderConnectionHelper(settings.ConnectionProvider);
		}

		public static void Main(string[] args)
		{
			try
			{
				var cfg = new Configuration();

				//string propFile = null;

				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("--"))
					{
						//if (args[i].StartsWith("--properties="))
						//{
						//  propFile = args[i].Substring(13);
						//}
						//else 
						if (args[i].StartsWith("--config="))
						{
							cfg.Configure(args[i].Substring(9));
						}
						else if (args[i].StartsWith("--naming="))
						{
							cfg.SetNamingStrategy(
								(INamingStrategy)
								Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(ReflectHelper.ClassForName(args[i].Substring(9))));
						}
					}
					else
					{
						cfg.AddFile(args[i]);
					}
				}
				/* NH: No props file for .NET
				if ( propFile != null ) {
					Properties props = new Properties();
					props.putAll( cfg.getProperties() );
					props.load( new FileInputStream( propFile ) );
					cfg.setProperties( props );
				}
				*/
				new SchemaValidator(cfg).Validate();
			}
			catch (Exception e)
			{
				log.Error("Error running schema update", e);
				Console.WriteLine(e);
			}
		}

		// Perform the validations.
		public void Validate()
		{
			log.Info("Running schema validator");
			try
			{
				DatabaseMetadata meta;
				try
				{
					log.Info("fetching database metadata");
					connectionHelper.Prepare();
					var connection = connectionHelper.Connection;
					meta = new DatabaseMetadata(connection, dialect, false);
				}
				catch (Exception sqle)
				{
					log.Error("could not get database metadata", sqle);
					throw;
				}
				configuration.ValidateSchema(dialect, meta);
			}
			catch (Exception e)
			{
				log.Error("could not complete schema validation", e);
				throw;
			}
			finally
			{
				try
				{
					connectionHelper.Release();
				}
				catch (Exception e)
				{
					log.Error("Error closing connection", e);
				}
			}
		}
	}
}