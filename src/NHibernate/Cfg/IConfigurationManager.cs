using System;
using System.Configuration;
using System.Linq;

namespace NHibernate.Cfg
{
	internal static class ConfigurationManagerExtensions
	{
		//TODO 6.0:  Replace with GetAppSetting and document as possible breaking change all usages. 
		internal static string GetAppSettingIgnoringCase(this IConfigurationManager config, string name)
		{
			if (!(config is SystemConfigurationManager))
				return config.GetAppSetting(name);

			var key = ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => name.Equals(k, StringComparison.OrdinalIgnoreCase));
			return string.IsNullOrEmpty(key)
				? null
				: ConfigurationManager.AppSettings[key];
		}
	}

	public interface IConfigurationManager
	{
		IHibernateConfiguration GetConfiguration();
		string GetNamedConnectionString(string name);
		string GetAppSetting(string name);
	}

	class SystemConfigurationManager : IConfigurationManager
	{
		public IHibernateConfiguration GetConfiguration()
		{
			//TODO 6.0: Throw if not null and not IHibernateConfiguration
			return ConfigurationManager.GetSection(ConfigurationSchema.CfgXmlHelper.CfgSectionName) as IHibernateConfiguration;
		}

		public string GetNamedConnectionString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
		}

		public string GetAppSetting(string name)
		{
			return ConfigurationManager.AppSettings[name];
		}
	}
}
