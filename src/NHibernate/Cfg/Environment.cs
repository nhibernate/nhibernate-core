using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using NHibernate.Util;

namespace NHibernate.Cfg {

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
	/// </remarks>
	public class Environment {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Environment));

		private static IDictionary properties = new Hashtable();
		private static IDictionary isolationLevels = new Hashtable();

		private const string Version = "prealpha3";

		public const string ConnectionProvider = "hibernate.connection.provider";
		public const string ConnectionDriver = "hibernate.connection.driver_class";
		public const string ConnectionString = "hibernate.connection.connection_string";
		public const string Isolation = "hibernate.connection.isolation";
		public const string StatementCacheSize = "hibernate.statement_cache.size";
		public const string SessionFactoryName = "hibernate.session_factory_name";
		public const string Dialect = "hibernate.dialect";
		public const string DefaultSchema = "hibernate.default_schema";
		public const string ShowSql = "hibernate.show_sql";
		public const string OuterJoin = "hibernate.use_outer_join";
		public const string OutputStylesheet = "hibernate.xml.output_stylesheet";
		public const string TransactionStrategy = "hibernate.transaction.factory_class";
		public const string TransactionManagerStrategy = "hibernate.transaction.manager_lookup_class";
		public const string QuerySubstitutions = "hibernate.query.substitutions";
		public const string QueryImports = "hibernate.query.imports";

		// MikeD added these while synching up SessionFactoryImpl.  Not sure if they have any ado.net
		// equivalents - we can probably remove these and remove the SessionFactoryImpl code that
		// uses them.
		public const string PoolSize = "hibernate.connection.pool_size";
		public const string StatementBatchSize = "hibernate.jdbc.batch_size";
		public const string StatementFetchSize = "hibernate.jdbc.fetch_size";
		public const string UseScrollableResultSet = "hibernate.jdbc.use_scrollable_resultset";

		static Environment() {
			log4net.Config.DOMConfigurator.Configure();

			NameValueCollection props = System.Configuration.ConfigurationSettings.GetConfig("nhibernate") as NameValueCollection;
			if (props==null) throw new HibernateException("no nhibernate settings available");
			
			foreach(string key in props.Keys) {
				properties.Add(key, props[key]);
			}

			isolationLevels.Add( System.Data.IsolationLevel.Chaos, "NONE" );
			isolationLevels.Add( System.Data.IsolationLevel.ReadUncommitted, "READ_UNCOMMITTED" );
			isolationLevels.Add( System.Data.IsolationLevel.ReadCommitted, "READ_COMMITTED" );
			isolationLevels.Add( System.Data.IsolationLevel.RepeatableRead, "REPEATABLE_READ" );
			isolationLevels.Add( System.Data.IsolationLevel.Serializable, "SERIALIZABLE" );
		}

		public static IDictionary Properties {
			get { return properties; }
		}

		public static bool UseStreamsForBinary {
			get { return true; }			
		}
	}
}
