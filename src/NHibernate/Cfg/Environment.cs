using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

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
	/// </remarks>
	public class Environment 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Environment));

		private static IDictionary properties = new Hashtable();
		
		public const string ConnectionProvider = "hibernate.connection.provider";
		public const string ConnectionDriver = "hibernate.connection.driver_class";
		public const string ConnectionString = "hibernate.connection.connection_string";
		public const string Isolation = "hibernate.connection.isolation";
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
		public const string CacheProvider = "hibernate.cache.provider_class";
		public const string PrepareSql = "hibernate.prepare_sql";

		static Environment() 
		{
			log4net.Config.DOMConfigurator.Configure();

			NameValueCollection props = System.Configuration.ConfigurationSettings.GetConfig("nhibernate") as NameValueCollection;
			if (props==null) 
			{
				log.Debug("no hibernate settings in app.config/web.config were found");
				return;
			}
			
			foreach(string key in props.Keys) 
			{
				properties[key] = props[key];
			}

			
		}

		/// <summary>
		/// Gets a copy of the configuration found in app.config/web.config
		/// </summary>
		/// <remarks>
		/// This is the replacement for hibernate.properties
		/// </remarks>
		public static IDictionary Properties 
		{
			get 
			{ 
				IDictionary copy = new Hashtable(properties.Count);
				foreach(DictionaryEntry de in properties) 
				{
					copy[de.Key] = de.Value;
				}
				return copy;
			}
		}

		public static bool UseStreamsForBinary 
		{
			get { return true; }			
		}
	}
}
