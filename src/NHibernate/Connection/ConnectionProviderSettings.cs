using System;
using System.Xml;
using System.Collections;
using System.Configuration;

namespace NHibernate.Connection {
	/// <summary>
	/// Summary description for ConnectionProviderSettings.
	/// </summary>
	public class ConnectionProviderSettings : IConfigurationSectionHandler {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ConnectionProviderSettings));
		private Hashtable properties = new Hashtable();

		public System.Type Type {
			get { return null; }
		}

		public string this [ string key ] {
			get {
				return properties[key] as string;
			}
		}

		public object Create(object parent, object configContext, XmlNode section) {
			//TODO: Load the type, and add all properties to the hashtable
			log.Info("Creating ConnectionProviderSettings from config file");
			return null;
		}

	}
}
