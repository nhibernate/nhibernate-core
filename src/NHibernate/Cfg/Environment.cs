using System;
using System.Xml;
using System.Collections;
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
	public class Environment : IConfigurationSectionHandler {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Environment));

		private IDictionary properties;
		private IDictionary isolationLevels = new Hashtable();

		private const string Version = "0.1 beta 1";

		public const string ConnectionProvider = "hibernate.connection.provider";
		public const string DataSource = "hibernate.connection.datasource";
		public const string User = "hibernate.connection.username";
		public const string Pass = "hibernate.connection.password";
		public const string Database = "hibernate.connection.database"; //initial catalog for sql server
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

		


		public object Create(object parent, object configContext, XmlNode section) {
			isolationLevels.Add( System.Data.IsolationLevel.Chaos, "NONE" );
			isolationLevels.Add( System.Data.IsolationLevel.ReadUncommitted, "READ_UNCOMMITTED" );
			isolationLevels.Add( System.Data.IsolationLevel.ReadCommitted, "READ_COMMITTED" );
			isolationLevels.Add( System.Data.IsolationLevel.RepeatableRead, "REPEATABLE_READ" );
			isolationLevels.Add( System.Data.IsolationLevel.Serializable, "SERIALIZABLE" );

			properties = PropertiesHelper.GetParams((XmlElement) section);

			return null;
		}

		public IDictionary Properties {
			get { return properties; }
		}

		public static bool UseStreamsForBinary {
			get { return true; }			
		}
	}
}
