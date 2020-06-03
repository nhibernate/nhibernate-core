using System;
using System.Collections.Generic;
using System.Reflection;

using NHibernate.Bytecode;
using NHibernate.Cfg.ConfigurationSchema;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Util;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Provides access to configuration information.
	/// </summary>
	/// <remarks>
	/// NHibernate has two property scopes:
	/// <list>
	///		<item><description>
	///		 Factory-level properties may be passed to the <see cref="ISessionFactory" /> when it is
	///		 instantiated. Each instance might have different property values. If no properties are
	///		 specified, the factory gets them from Environment
	///		</description></item>
	///		<item><description>
	///		 System-level properties are shared by all factory instances and are always determined
	///		 by the <see cref="Cfg.Environment" /> properties
	///		</description></item>
	/// </list>
	/// In NHibernate, <c>&lt;hibernate-configuration&gt;</c> section in the application configuration file
	/// corresponds to Java system-level properties; <c>&lt;session-factory&gt;</c>
	/// section is the session-factory-level configuration. 
	/// 
	/// It is possible to use the application configuration file (App.config) together with the NHibernate 
	/// configuration file (hibernate.cfg.xml) at the same time.
	/// Properties in hibernate.cfg.xml override/merge properties in application configuration file where same
	/// property is found. For others configuration a merge is applied.
	/// </remarks>
	public static class Environment
	{
		private static string cachedVersion;

		/// <summary>
		/// NHibernate version (informational).
		/// </summary>
		public static string Version
		{
			get
			{
				if (cachedVersion == null)
				{
					Assembly thisAssembly = Assembly.GetExecutingAssembly();
					var attrs =
						(AssemblyInformationalVersionAttribute[])
						thisAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);

					if (attrs != null && attrs.Length > 0)
					{
						cachedVersion = string.Format("{0} (assembly {1})", attrs[0].InformationalVersion, thisAssembly.GetName().Version);
					}
					else
					{
						cachedVersion = thisAssembly.GetName().Version.ToString();
					}
				}
				return cachedVersion;
			}
		}

		public const string ConnectionProvider = "connection.provider";
		public const string ConnectionDriver = "connection.driver_class";
		public const string ConnectionString = "connection.connection_string";
		public const string Isolation = "connection.isolation";
		public const string ReleaseConnections = "connection.release_mode";

		/// <summary>
		/// Used to find the .Net 2.0 named connection string
		/// </summary>
		public const string ConnectionStringName = "connection.connection_string_name";

		// Unused, Java-specific
		// But has many code usage though.
		public const string SessionFactoryName = "session_factory_name";

		public const string Dialect = "dialect";

		/// <summary> A default database schema (owner) name to use for unqualified tablenames</summary>
		public const string DefaultSchema = "default_schema";

		/// <summary> A default database catalog name to use for unqualified tablenames</summary>
		public const string DefaultCatalog = "default_catalog";

		// Since v5
		[Obsolete("DefaultEntityMode is deprecated.")]
		public const string DefaultEntityMode = "default_entity_mode";

		/// Implementation of NH-3619 - Make default value of FlushMode configurable
		public const string DefaultFlushMode = "default_flush_mode";

		/// <summary>
		/// When using an enhanced id generator and pooled optimizers (<see cref="NHibernate.Id.Enhanced.IOptimizer"/>),
		/// prefer interpreting the database value as the lower (lo) boundary. The default is to interpret it as the high boundary.
		/// </summary>
		public const string PreferPooledValuesLo = "id.optimizer.pooled.prefer_lo";

		public const string ShowSql = "show_sql";
		public const string MaxFetchDepth = "max_fetch_depth";
		public const string CurrentSessionContextClass = "current_session_context_class";
		public const string UseSqlComments = "use_sql_comments";

		/// <summary> Enable formatting of SQL logged to the console</summary>
		public const string FormatSql = "format_sql";

		// Since v5.0.1
		[Obsolete("This setting has no usages and will be removed in a future version")]
		public const string UseGetGeneratedKeys = "jdbc.use_get_generated_keys";

		// Since v5.0.1
		[Obsolete("This setting has no usages and will be removed in a future version")]
		public const string StatementFetchSize = "jdbc.fetch_size";

		// Since v5.0.1
		[Obsolete("This setting has no usages and will be removed in a future version")]
		public const string OutputStylesheet = "xml.output_stylesheet";

		/// <summary>
		/// The class name of a custom <see cref="Transaction.ITransactionFactory"/> implementation. Defaults to the
		/// built-in <see cref="Transaction.AdoNetWithSystemTransactionFactory" />.
		/// </summary>
		public const string TransactionStrategy = "transaction.factory_class";
		/// <summary>
		/// <para>Timeout duration in milliseconds for the system transaction completion lock.</para>
		/// <para>When a system transaction completes, it may have its completion events running on concurrent threads,
		/// after scope disposal. This occurs when the transaction is distributed.
		/// This notably concerns <see cref="ISessionImplementor.AfterTransactionCompletion(bool, ITransaction)"/>.
		/// NHibernate protects the session from being concurrently used by the code following the scope disposal
		/// with a lock. To prevent any application freeze, this lock has a default timeout of five seconds. If the
		/// application appears to require longer (!) running transaction completion events, this setting allows to
		/// raise this timeout. <c>-1</c> disables the timeout.</para>
		/// </summary>
		public const string SystemTransactionCompletionLockTimeout = "transaction.system_completion_lock_timeout";
		/// <summary>
		/// When a system transaction is being prepared, is using connection during this process enabled?
		/// Default is <see langword="true"/>, for supporting <see cref="FlushMode.Commit"/> with transaction factories
		/// supporting system transactions. But this requires enlisting additional connections, retaining disposed
		/// sessions and their connections till transaction end, and may trigger undesired transaction promotions to
		/// distributed. Set to <see langword="false"/> for disabling using connections from system
		/// transaction preparation, while still benefiting from <see cref="FlushMode.Auto"/> on querying.
		/// </summary>
		public const string UseConnectionOnSystemTransactionPrepare = "transaction.use_connection_on_system_prepare";
		/// <summary>
		/// Should sessions check on every operation whether there is an ongoing system transaction or not, and enlist
		/// into it if any? Default is <see langword="true"/>. It can also be controlled at session opening, see
		/// <see cref="ISessionFactory.WithOptions" />. A session can also be instructed to explicitly join the current
		/// transaction by calling <see cref="ISession.JoinTransaction" />. This setting has no effect when using a
		/// transaction factory that is not system transactions aware.
		/// </summary>
		public const string AutoJoinTransaction = "transaction.auto_join";

		// Since v5.0.1
		[Obsolete("This setting has no usages and will be removed in a future version")]
		public const string TransactionManagerStrategy = "transaction.manager_lookup_class";

		public const string CacheProvider = "cache.provider_class";
		public const string UseQueryCache = "cache.use_query_cache";
		public const string QueryCacheFactory = "cache.query_cache_factory";
		public const string UseSecondLevelCache = "cache.use_second_level_cache";
		public const string CacheRegionPrefix = "cache.region_prefix";
		public const string UseMinimalPuts = "cache.use_minimal_puts";
		public const string CacheDefaultExpiration = "cache.default_expiration";
		public const string QuerySubstitutions = "query.substitutions";

		/// <summary> Should named queries be checked during startup (the default is enabled). </summary>
		/// <remarks>Mainly intended for test environments.</remarks>
		public const string QueryStartupChecking = "query.startup_check";

		/// <summary> Enable statistics collection</summary>
		public const string GenerateStatistics = "generate_statistics";

		// Its test is ignored with reason "Not supported yet".
		public const string UseIdentifierRollBack = "use_identifier_rollback";

		/// <summary>
		/// The classname of the HQL query parser factory.
		/// </summary>
		public const string QueryTranslator = "query.factory_class";

		/// <summary>
		/// The class name of the LINQ query provider class, implementing <see cref="INhQueryProvider"/>.
		/// </summary>
		public const string QueryLinqProvider = "query.linq_provider_class";

		// Since v5.0.1
		[Obsolete("This setting has no usages and will be removed in a future version")]
		public const string QueryImports = "query.imports";
		public const string Hbm2ddlAuto = "hbm2ddl.auto";
		public const string Hbm2ddlKeyWords = "hbm2ddl.keywords";

		public const string SqlExceptionConverter = "sql_exception_converter";

		public const string BatchVersionedData = "adonet.batch_versioned_data";
		public const string WrapResultSets = "adonet.wrap_result_sets";
		public const string BatchSize = "adonet.batch_size";
		public const string BatchStrategy = "adonet.factory_class";

		// NHibernate-specific properties
		public const string PrepareSql = "prepare_sql";
		/// <summary>
		/// Set the default timeout in seconds for ADO.NET queries.
		/// </summary>
		public const string CommandTimeout = "command_timeout";

		public const string PropertyBytecodeProvider = "bytecode.provider";
		public const string PropertyUseReflectionOptimizer = "use_reflection_optimizer";

		/// <summary>
		/// Set the <see cref="IObjectsFactory"/> used to instantiate NHibernate's objects.
		/// </summary>
		public const string PropertyObjectsFactory = "objects_factory";

		public const string UseProxyValidator = "use_proxy_validator";
		public const string ProxyFactoryFactoryClass = "proxyfactory.factory_class";

		public const string DefaultBatchFetchSize = "default_batch_fetch_size";

		public const string CollectionTypeFactoryClass = "collectiontype.factory_class";

		public const string LinqToHqlGeneratorsRegistry = "linqtohql.generatorsregistry";

		/// <summary>
		/// Whether to use the legacy pre-evaluation or not in Linq queries. <c>true</c> by default.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Legacy pre-evaluation is causing special properties or functions like <c>DateTime.Now</c> or
		/// <c>Guid.NewGuid()</c> to be always evaluated with the .Net runtime and replaced in the query by
		/// parameter values.
		/// </para>
		/// <para>
		/// The new pre-evaluation allows them to be converted to HQL function calls which will be run on the db
		/// side. This allows for example to retrieve the server time instead of the client time, or to generate
		/// UUIDs for each row instead of an unique one for all rows. (This does not happen if the dialect does
		/// not support the required HQL function.)
		/// </para>
		/// <para>
		/// The new pre-evaluation will likely be enabled by default in the next major version (6.0).
		/// </para>
		/// </remarks>
		public const string LinqToHqlLegacyPreEvaluation = "linqtohql.legacy_preevaluation";

		/// <summary>
		/// When the new pre-evaluation is enabled, should methods which translation is not supported by the current
		/// dialect fallback to pre-evaluation? <c>false</c> by default.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When this fallback option is enabled while legacy pre-evaluation is disabled, properties or functions
		/// like <c>DateTime.Now</c> or <c>Guid.NewGuid()</c> used in Linq expressions will not fail when the dialect does not
		/// support them, but will instead be pre-evaluated.
		/// </para>
		/// <para>
		/// When this fallback option is disabled while legacy pre-evaluation is disabled, properties or functions
		/// like <c>DateTime.Now</c> or <c>Guid.NewGuid()</c> used in Linq expressions will fail when the dialect does not
		/// support them.
		/// </para>
		/// <para>
		/// This option has no effect if the legacy pre-evaluation is enabled.
		/// </para>
		/// </remarks>
		public const string LinqToHqlFallbackOnPreEvaluation = "linqtohql.fallback_on_preevaluation";

		/// <summary> Enable ordering of insert statements for the purpose of more efficient batching.</summary>
		public const string OrderInserts = "order_inserts";

		/// <summary> Enable ordering of update statements for the purpose of more efficient batching.</summary>
		public const string OrderUpdates = "order_updates";

		public const string QueryModelRewriterFactory = "query.query_model_rewriter_factory";

		/// <summary>
		/// The class name of the LINQ query pre-transformer initializer, implementing <see cref="IExpressionTransformerInitializer"/>.
		/// </summary>
		public const string PreTransformerInitializer = "query.pre_transformer_initializer";

		/// <summary>
		/// Set the default length used in casting when the target type is length bound and
		/// does not specify it. <c>4000</c> by default, automatically trimmed down according to dialect type registration.
		/// </summary>
		public const string QueryDefaultCastLength = "query.default_cast_length";

		/// <summary>
		/// Set the default precision used in casting when the target type is decimal and
		/// does not specify it. <c>29</c> by default, automatically trimmed down according to dialect type registration.
		/// </summary>
		public const string QueryDefaultCastPrecision = "query.default_cast_precision";

		/// <summary>
		/// Set the default scale used in casting when the target type is decimal and
		/// does not specify it. <c>10</c> by default, automatically trimmed down according to dialect type registration.
		/// </summary>
		public const string QueryDefaultCastScale = "query.default_cast_scale";

		/// <summary>
		/// This may need to be set to 3 if you are using the OdbcDriver with MS SQL Server 2008+.
		/// </summary>
		public const string OdbcDateTimeScale = "odbc.explicit_datetime_scale";

		/// <summary>
		/// Disable switching built-in NHibernate date-time types from DbType.DateTime to DbType.DateTime2
		/// for dialects supporting datetime2.
		/// </summary>
		public const string SqlTypesKeepDateTime = "sql_types.keep_datetime";

		/// <summary>
		/// <para>Oracle has a dual Unicode support model.</para>
		/// <para>Either the whole database use an Unicode encoding, and then all string types
		/// will be Unicode. In such case, Unicode strings should be mapped to non <c>N</c> prefixed
		/// types, such as <c>Varchar2</c>. This is the default.</para>
		/// <para>Or <c>N</c> prefixed types such as <c>NVarchar2</c> are to be used for Unicode strings.</para>
		/// </summary>
		/// <remarks>
		/// See https://docs.oracle.com/cd/B19306_01/server.102/b14225/ch6unicode.htm#CACHCAHF
		/// https://docs.oracle.com/database/121/ODPNT/featOraCommand.htm#i1007557
		/// This setting applies only to Oracle dialects and ODP.Net managed or unmanaged driver.
		/// </remarks>
		public const string OracleUseNPrefixedTypesForUnicode = "oracle.use_n_prefixed_types_for_unicode";

		/// <summary>
		/// Oracle 10g introduced BINARY_DOUBLE and BINARY_FLOAT types which are compatible with .NET
		/// <see cref="double"/> and <see cref="float"/> types, where FLOAT and DOUBLE are not. Oracle
		/// FLOAT and DOUBLE types do not conform to the IEEE standard as they are internally implemented as
		/// NUMBER type, which makes them an exact numeric type.
		/// <para>
		/// <see langword="false"/> by default.
		/// </para>
		/// </summary>
		/// <remarks>
		/// See https://docs.oracle.com/database/121/TTSQL/types.htm#TTSQL126
		/// </remarks>
		public const string OracleUseBinaryFloatingPointTypes = "oracle.use_binary_floating_point_types";

		/// <summary>
		/// <para>
		/// Firebird with FirebirdSql.Data.FirebirdClient may be unable to determine the type
		/// of parameters in many circumstances, unless they are explicitly casted in the SQL
		/// query. To avoid this trouble, the NHibernate <c>FirebirdClientDriver</c> parses SQL
		/// commands for detecting parameters in them and adding an explicit SQL cast around
		/// parameters which may trigger the issue.
		/// </para>
		/// <para>
		/// For disabling this behavior, set this setting to true.
		/// </para>
		/// </summary>
		public const string FirebirdDisableParameterCasting = "firebird.disable_parameter_casting";

		/// <summary>
		/// <para>
		/// SQLite can store GUIDs in binary or text form, controlled by the BinaryGuid
		/// connection string parameter (default is 'true'). The BinaryGuid setting will affect
		/// how to cast GUID to string in SQL. NHibernate will attempt to detect this
		/// setting automatically from the connection string, but if the connection
		/// or connection string is being handled by the application instead of by NHibernate,
		/// you can use the 'sqlite.binaryguid' NHibernate setting to override the behavior.
		/// </para>
		/// </summary>
		public const string SqliteBinaryGuid = "sqlite.binaryguid";

		/// <summary>
		/// <para>Set whether tracking the session id or not. When <see langword="true"/>, each session 
		/// will have an unique <see cref="Guid"/> that can be retrieved by <see cref="ISessionImplementor.SessionId"/>,
		/// otherwise <see cref="ISessionImplementor.SessionId"/> will always be <see cref="Guid.Empty"/>. Session id 
		/// is used for logging purpose that can be also retrieved in a static context by 
		/// <see cref="NHibernate.Impl.SessionIdLoggingContext.SessionId"/>, where the current session id is stored,
		/// when tracking is enabled.</para>
		/// In case the current session id won't be used, it is recommended to disable it, in order to increase performance.
		/// <para>Default is <see langword="true"/>.</para>
		/// </summary>
		public const string TrackSessionId = "track_session_id";

		private static readonly Dictionary<string, string> GlobalProperties = new Dictionary<string, string>();

		private static IBytecodeProvider BytecodeProviderInstance;
		private static bool EnableReflectionOptimizer;

		private static readonly INHibernateLogger log = NHibernateLogger.For(typeof(Environment));

		/// <summary>
		/// Issue warnings to user when any obsolete property names are used.
		/// </summary>
		/// <param name="props"></param>
		/// <returns></returns>
		public static void VerifyProperties(IDictionary<string, string> props) { }

		static Environment()
		{
			// Computing the version string is a bit expensive, so do it only if logging is enabled.
			if (log.IsInfoEnabled())
			{
				log.Info("NHibernate {0}", Version);
			}

			InitializeGlobalProperties(GetHibernateConfiguration());
		}

		public static void InitializeGlobalProperties(IHibernateConfiguration config)
		{
			GlobalProperties.Clear();
			if (config != null)
			{
				HibernateConfiguration = config;
				GlobalProperties[PropertyBytecodeProvider] = config.ByteCodeProviderType;
				GlobalProperties[PropertyUseReflectionOptimizer] = config.UseReflectionOptimizer.ToString();
				if (config is HibernateConfiguration nhConfig)
				{
					GlobalProperties[PropertyObjectsFactory] = nhConfig.ObjectsFactoryType;
				}
				if (config.SessionFactory != null)
				{
					foreach (var kvp in config.SessionFactory.Properties)
					{
						GlobalProperties[kvp.Key] = kvp.Value;
					}
				}
			}
			else
			{
				GlobalProperties[PropertyUseReflectionOptimizer] = bool.TrueString;
			}

			VerifyProperties(GlobalProperties);

			BytecodeProviderInstance = BuildBytecodeProvider(GlobalProperties);
			ObjectsFactory = BuildObjectsFactory(GlobalProperties);
			EnableReflectionOptimizer = PropertiesHelper.GetBoolean(PropertyUseReflectionOptimizer, GlobalProperties);

			if (EnableReflectionOptimizer)
			{
				log.Info("Using reflection optimizer");
			}
		}

		internal static IHibernateConfiguration HibernateConfiguration { get; private set; }

		private static IHibernateConfiguration GetHibernateConfiguration()
		{
			var nhConfig = ConfigurationProvider.Current.GetConfiguration();
			if (nhConfig == null && log.IsInfoEnabled())
			{
				log.Info("{0} section not found in application configuration file", CfgXmlHelper.CfgSectionName);
			}

			return nhConfig;
		}

		/// <summary>
		/// Gets a copy of the configuration found in <c>&lt;hibernate-configuration&gt;</c> section
		/// of app.config/web.config.
		/// </summary>
		/// <remarks>
		/// This is the replacement for hibernate.properties
		/// </remarks>
		//Since v5.3
		[Obsolete("This property is not used and will be removed in a future version.")]
		public static IDictionary<string, string> Properties
		{
			get { return new Dictionary<string, string>(GlobalProperties); }
		}

		/// <summary>
		/// The bytecode provider to use.
		/// </summary>
		/// <remarks>
		/// This property is read from the <c>&lt;hibernate-configuration&gt;</c> section
		/// of the application configuration file by default. Since it is not
		/// always convenient to configure NHibernate through the application
		/// configuration file, it is also possible to set the property value
		/// manually. This should only be done before a configuration object
		/// is created, otherwise the change may not take effect.
		/// </remarks>
		public static IBytecodeProvider BytecodeProvider
		{
			get { return BytecodeProviderInstance; }
			set
			{
				BytecodeProviderInstance = value;
				// 6.0 TODO: remove following code.
#pragma warning disable 618
				var objectsFactory = BytecodeProviderInstance.ObjectsFactory;
#pragma warning restore 618
				if (objectsFactory != null)
					ObjectsFactory = objectsFactory;
			}
		}

		/// <summary>
		/// NHibernate's object instantiator.
		/// </summary>
		/// <remarks>
		/// This property is read from the <c>&lt;hibernate-configuration&gt;</c> section
		/// of the application configuration file by default. Since it is not
		/// always convenient to configure NHibernate through the application
		/// configuration file, it is also possible to set the property value
		/// manually.
		/// This should only be set before a configuration object
		/// is created, otherwise the change may not take effect.
		/// For entities see <see cref="IReflectionOptimizer"/> and its implementations.
		/// </remarks>
		public static IObjectsFactory ObjectsFactory { get; set; } = new ActivatorObjectsFactory();

		/// <summary>
		/// Whether to enable the use of reflection optimizer
		/// </summary>
		/// <remarks>
		/// This property is read from the <c>&lt;hibernate-configuration&gt;</c> section
		/// of the application configuration file by default. Since it is not
		/// always convenient to configure NHibernate through the application
		/// configuration file, it is also possible to set the property value
		/// manually. This should only be done before a configuration object
		/// is created, otherwise the change may not take effect.
		/// </remarks>
		public static bool UseReflectionOptimizer
		{
			get { return EnableReflectionOptimizer; }
			set { EnableReflectionOptimizer = value; }
		}

		public static IBytecodeProvider BuildBytecodeProvider(IDictionary<string, string> properties)
		{
			const string defaultBytecodeProvider = "lcg";
			string provider = PropertiesHelper.GetString(PropertyBytecodeProvider, properties, defaultBytecodeProvider);
			log.Info("Bytecode provider name : {0}", provider);
			return BuildBytecodeProvider(provider);
		}

		private static IBytecodeProvider BuildBytecodeProvider(string providerName)
		{
			switch (providerName)
			{
				case "lcg":
					return new Bytecode.Lightweight.BytecodeProviderImpl();
				case "null":
					return new NullBytecodeProvider();
				default:
					log.Info("custom bytecode provider [{0}]", providerName);
					return CreateCustomBytecodeProvider(providerName);
			}
		}

		public static IObjectsFactory BuildObjectsFactory(IDictionary<string, string> properties)
		{
			var typeAssemblyQualifiedName = PropertiesHelper.GetString(PropertyObjectsFactory, properties, null);
			if (typeAssemblyQualifiedName == null)
			{
				// 6.0 TODO: use default value of ObjectsFactory property
#pragma warning disable 618
				var objectsFactory = BytecodeProvider.ObjectsFactory ?? ObjectsFactory;
#pragma warning restore 618
				log.Info("Objects factory class : {0}", objectsFactory.GetType());
				return objectsFactory;
			}
			log.Info("Custom objects factory class : {0}", typeAssemblyQualifiedName);
			return CreateCustomObjectsFactory(typeAssemblyQualifiedName);
		}

		private static IBytecodeProvider CreateCustomBytecodeProvider(string assemblyQualifiedName)
		{
			try
			{
				var type = ReflectHelper.ClassForName(assemblyQualifiedName);
				try
				{
					return (IBytecodeProvider)Activator.CreateInstance(type);
				}
				catch (MissingMethodException ex)
				{
					throw new HibernateByteCodeException("Public constructor was not found for " + type, ex);
				}
				catch (InvalidCastException ex)
				{
					throw new HibernateByteCodeException(type + "Type does not implement " + typeof(IBytecodeProvider), ex);
				}
				catch (Exception ex)
				{
					throw new HibernateByteCodeException("Unable to instantiate: " + type, ex);
				}
			}
			catch (Exception e)
			{
				throw new HibernateByteCodeException("Unable to create the instance of Bytecode provider; check inner exception for detail", e);
			}
		}

		private static IObjectsFactory CreateCustomObjectsFactory(string assemblyQualifiedName)
		{
			try
			{
				var type = ReflectHelper.ClassForName(assemblyQualifiedName);
				try
				{
					return (IObjectsFactory) Activator.CreateInstance(type);
				}
				catch (MissingMethodException ex)
				{
					throw new HibernateObjectsFactoryException("Public constructor was not found for " + type, ex);
				}
				catch (InvalidCastException ex)
				{
					throw new HibernateObjectsFactoryException(type + "Type does not implement " + typeof(IObjectsFactory), ex);
				}
				catch (Exception ex)
				{
					throw new HibernateObjectsFactoryException("Unable to instantiate: " + type, ex);
				}
			}
			catch (Exception e)
			{
				throw new HibernateObjectsFactoryException("Unable to create the instance of objects factory; check inner exception for detail", e);
			}
		}

		/// <summary>
		/// Get a named connection string, if configured.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when a <see cref="ConnectionStringName"/> was found 
		/// in the <c>settings</c> parameter but could not be found in the app.config.
		/// </exception>
		internal static string GetNamedConnectionString(IDictionary<string, string> settings)
		{
			if (!settings.TryGetValue(ConnectionStringName, out var connStringName))
				return null;

			return ConfigurationProvider.Current.GetNamedConnectionString(connStringName)
			       ?? throw new HibernateException($"Could not find named connection string '{connStringName}'.");
		}

		/// <summary>
		/// Get the configured connection string, from <see cref="ConnectionString"/> if that
		/// is set, otherwise from <see cref="ConnectionStringName"/>, or null if that isn't
		/// set either.
		/// </summary>
		internal static string GetConfiguredConnectionString(IDictionary<string, string> settings)
		{ 
			// Connection string in the configuration overrides named connection string.
			if (!settings.TryGetValue(ConnectionString, out string connString))
				connString = GetNamedConnectionString(settings);

			return connString;
		}
	}
}
