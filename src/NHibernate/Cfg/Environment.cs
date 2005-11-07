using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;

using log4net;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Provides access to configuration info
	/// </summary>
	/// <remarks>
	/// Hibernate has two property scopes:
	/// <list>
	///		<item>
	///		 Factory-Level properties may be passed to the <c>ISessionFactory</c> when it is instantiated.
	///		 Each instance might have different property values. If no properties are specified, the
	///		 factory gets them from Environment
	///		</item>
	///		<item>
	///		 System-Level properties are shared by all factory instances and are always determined
	///		 by the <c>Environment</c> properties
	///		</item>
	/// </list>
	/// In NHibernate, <c>&lt;nhibernate&gt;</c> section in the application configuration file
	/// corresponds to Java system-level properties; <c>&lt;hibernate-configuration&gt;</c>
	/// section is considered to be the session-factory-level configuration. It is possible
	/// to use the configuration file at the same time.
	/// </remarks>
	public sealed class Environment
	{
		public const string Version = "1.1-alpha1";

		public const string ConnectionProvider = "hibernate.connection.provider";
		public const string ConnectionDriver = "hibernate.connection.driver_class";
		public const string ConnectionString = "hibernate.connection.connection_string";
		public const string Isolation = "hibernate.connection.isolation";

		// Unused, Java-specific
		public const string SessionFactoryName = "hibernate.session_factory_name";
		
		public const string Dialect = "hibernate.dialect";
		public const string DefaultSchema = "hibernate.default_schema";
		public const string ShowSql = "hibernate.show_sql";
		public const string MaxFetchDepth = "hibernate.max_fetch_depth";
		
		// Unused, Java-specific
		public const string UseGetGeneratedKeys = "hibernate.jdbc.use_get_generated_keys";
		
		// Unused, not implemented
		public const string StatementFetchSize = "hibernate.jdbc.fetch_size";
		
		// Unused, not implemented
		public const string StatementBatchSize = "hibernate.jdbc.batch_size";
		
		public const string BatchVersionedData = "hibernate.jdbc.batch_versioned_data";
		
		// Unused, not implemented
		public const string OutputStylesheet = "hibernate.xml.output_stylesheet";
		
		// Unused, not implemented (and somewhat Java-specific)
		public const string TransactionStrategy = "hibernate.transaction.factory_class";
		
		// Unused, not implemented (and somewhat Java-specific)
		public const string TransactionManagerStrategy = "hibernate.transaction.manager_lookup_class";

		public const string CacheProvider = "hibernate.cache.provider_class";
		public const string UseQueryCache = "hibernate.cache.use_query_cache";
		public const string QueryCacheFactory = "hibernate.cache.query_cache_factory";
		public const string CacheRegionPrefix = "hibernate.cache.region_prefix";
		public const string UseMinimalPuts = "hibernate.cache.use_minimal_puts";
		public const string QuerySubstitutions = "hibernate.query.substitutions";
		
		// Unused, not implemented
		public const string QueryImports = "hibernate.query.imports";
		public const string Hbm2ddlAuto = "hibernate.hbm2ddl.auto";
		
		// Unused, not implemented
		public const string SqlExceptionConverter = "hibernate.sql_exception_converter";
		
		// Unused, not implemented
		public const string WrapResultSets = "hibernate.wrap_result_sets";

		// NHibernate-specific properties
		public const string PrepareSql = "hibernate.prepare_sql";
		public const string CommandTimeout = "hibernate.command_timeout";
		public const string PropertyUseReflectionOptimizer = "hibernate.use_reflection_optimizer";

		private static bool EnableReflectionOptimizer;

		private static IDictionary GlobalProperties;

		private static readonly ILog log = LogManager.GetLogger( typeof( Environment ) );

		private static bool isConfigured = false;

		/// <summary>
		/// Issue warnings to user when any obsolete property names are used.
		/// </summary>
		/// <param name="props"></param>
		/// <returns></returns>
		public static void VerifyProperties( IDictionary props )
		{
		}

		// This was a static constructor in Java. Now it is called from
		// Configuration constructor.
		internal static void Configure()
		{
			if( isConfigured )
			{
				return;	
			}

			log.Info( "NHibernate " + Environment.Version );

			GlobalProperties = new Hashtable();
			GlobalProperties[ PropertyUseReflectionOptimizer ] = true.ToString();

			LoadGlobalPropertiesFromAppConfig();

			VerifyProperties( GlobalProperties );

			EnableReflectionOptimizer = PropertiesHelper.GetBoolean( PropertyUseReflectionOptimizer, GlobalProperties );

			if( EnableReflectionOptimizer )
			{
				log.Info( "Using reflection optimizer" );
			}

			isConfigured = true;
		}

		private static void LoadGlobalPropertiesFromAppConfig()
		{
			object config = ConfigurationSettings.GetConfig( "nhibernate" );

			if( config == null )
			{
				log.Info( "nhibernate section not found in application configuration file" );
				return;
			}

			NameValueCollection properties = config as NameValueCollection;
			if( properties == null )
			{
				log.Info( "nhibernate section in application configuration file is not using NameValueSectionHandler, ignoring" );
				return;
			}

			foreach( string key in properties )
			{
				GlobalProperties[ key ] = properties[ key ];
			}
		}

		private Environment()
		{
			// should not be created.	
		}

		/// <summary>
		/// Gets a copy of the configuration found in <c>&lt;nhibernate&gt;</c> section
		/// of app.config/web.config.
		/// </summary>
		/// <remarks>
		/// This is the replacement for hibernate.properties
		/// </remarks>
		public static IDictionary Properties
		{
			get { return new Hashtable( GlobalProperties ); }
		}

		[Obsolete]
		public static bool UseStreamsForBinary
		{
			get { return true; }
		}

		public static bool UseReflectionOptimizer
		{
			get { return EnableReflectionOptimizer; }
		}
	}
}