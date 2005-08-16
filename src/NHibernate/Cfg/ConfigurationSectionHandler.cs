using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Summary description for ConfigurationSectionHandler.
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		public ConfigurationSectionHandler()
		{
		}

		public object Create (object parent, object configContext, XmlNode xmlNode)
		{
			return xmlNode;
		}
	}
}
