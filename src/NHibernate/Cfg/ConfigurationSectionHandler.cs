using System;
using System.Configuration;
using System.Xml;

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

		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}

		#endregion
	}
}