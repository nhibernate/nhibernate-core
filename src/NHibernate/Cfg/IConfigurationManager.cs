using System;
using System.Configuration;
using System.Linq;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	internal static class ConfigurationManagerExtensions
	{
		//TODO 6.0:  Replace with GetAppSetting and document as possible breaking change all usages. 
		internal static string GetAppSettingIgnoringCase(this IConfigurationManager config, string name)
		{
			if (!(config is StaticSystemConfigurationManager))
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

	public class SystemConfigurationManager : IConfigurationManager
	{
		private readonly System.Configuration.Configuration _configuration;

		public SystemConfigurationManager(System.Configuration.Configuration configuration)
		{
			_configuration = configuration;
		}

		public IHibernateConfiguration GetConfiguration()
		{
			ConfigurationSection configurationSection = _configuration.GetSection(CfgXmlHelper.CfgSectionName);
			var xml = configurationSection?.SectionInformation.GetRawXml();
			return xml == null ? null : HibernateConfiguration.FromAppConfig(xml);
		}

		public string GetNamedConnectionString(string name)
		{
			return _configuration.ConnectionStrings.ConnectionStrings[name]?.ConnectionString;
		}

		public string GetAppSetting(string name)
		{
			return _configuration.AppSettings.Settings[name]?.Value;
		}
	}

	class StaticSystemConfigurationManager : IConfigurationManager
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

	class NullConfigurationManager : IConfigurationManager
	{
		public IHibernateConfiguration GetConfiguration()
		{
			return null;
		}

		public string GetNamedConnectionString(string name)
		{
			return null;
		}

		public string GetAppSetting(string name)
		{
			return null;
		}
	}
}
