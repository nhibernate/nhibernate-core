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
	}
}
