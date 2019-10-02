using System.Configuration;
using NHibernate.Cfg.ConfigurationSchema;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Configuration manager that supports user provided configuration
	/// </summary>
	public class SystemConfigurationManager : ConfigurationProvider
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
}
