using System;
using System.Configuration;
using System.Linq;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Base class for configuration settings that NHibernate might require.
	/// </summary>
	public abstract class ConfigurationManagerBase
	{
		public abstract IHibernateConfiguration GetConfiguration();
		public abstract string GetNamedConnectionString(string name);

		/// <summary>
		/// Type that implements <see cref="INHibernateLoggerFactory"/>
		/// </summary>
		public abstract string GetLoggerFactoryClassName();
	}

	/// <summary>
	/// Configuration manager that supports user provided configuration
	/// </summary>
	public class SystemConfigurationManager : ConfigurationManagerBase
	{
		private readonly System.Configuration.Configuration _configuration;

		public SystemConfigurationManager(System.Configuration.Configuration configuration)
		{
			_configuration = configuration;
		}

		public override IHibernateConfiguration GetConfiguration()
		{
			ConfigurationSection configurationSection = _configuration.GetSection(CfgXmlHelper.CfgSectionName);
			var xml = configurationSection?.SectionInformation.GetRawXml();
			return xml == null ? null : HibernateConfiguration.FromAppConfig(xml);
		}

		public override string GetNamedConnectionString(string name)
		{
			return _configuration.ConnectionStrings.ConnectionStrings[name]?.ConnectionString;
		}

		public override string GetLoggerFactoryClassName()
		{
			return GetAppSetting(AppSettings.LoggerFactoryClassName);
		}

		private string GetAppSetting(string name)
		{
			return  _configuration.AppSettings.Settings[name]?.Value;
		}
	}

	class StaticSystemConfigurationManager : ConfigurationManagerBase
	{
		public override IHibernateConfiguration GetConfiguration()
		{
			//TODO 6.0: Throw if not null and not IHibernateConfiguration
			return ConfigurationManager.GetSection(CfgXmlHelper.CfgSectionName) as IHibernateConfiguration;
		}

		public override string GetNamedConnectionString(string name)
		{
			return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
		}

		public override string GetLoggerFactoryClassName()
		{
			var name = AppSettings.LoggerFactoryClassName;
			var value = ConfigurationManager.AppSettings[name];

			//TODO 6.0:  Return value right away. Don't do ignore case search and document it as possible breaking change.
			if (value != null)
				return value;

			return GetAppSettingIgnoreCase(name);
		}

		//TODO 6.0: Remove it
		private static string GetAppSettingIgnoreCase(string name)
		{
			var key = ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => name.Equals(k, StringComparison.OrdinalIgnoreCase));
			return string.IsNullOrEmpty(key)
				? null
				: ConfigurationManager.AppSettings[key];
		}
	}

	class NullConfigurationManager : ConfigurationManagerBase
	{
		public override IHibernateConfiguration GetConfiguration()
		{
			return null;
		}

		public override string GetNamedConnectionString(string name)
		{
			return null;
		}

		public override string GetLoggerFactoryClassName()
		{
			return null;
		}
	}
}
