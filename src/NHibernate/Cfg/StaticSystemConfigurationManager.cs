using System;
using System.Configuration;
using System.Linq;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	class StaticSystemConfigurationManager : ConfigurationProvider
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
}
