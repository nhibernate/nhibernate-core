using System;
using System.Configuration;
using System.Xml;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Summary description for ConfigurationSectionHandler.
	/// </summary>
	public class ConfigurationSectionHandler : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
		{
			return HibernateConfiguration.FromAppConfig(section);
		}

		#endregion
	}
}