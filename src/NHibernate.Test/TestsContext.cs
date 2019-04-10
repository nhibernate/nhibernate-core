using NUnit.Framework;

#if NETCOREAPP2_0
using System.Configuration;
using System.IO;
using NHibernate.Cfg;
using NHibernate.Cfg.ConfigurationSchema;
#endif

namespace NHibernate.Test
{
#if NETCOREAPP2_0
	[SetUpFixture]
#endif
	public class TestsContext
	{
		private static bool ExecutingWithVsTest { get; } =
			System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name == "testhost";

#if NETCOREAPP2_0
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			//When .NET Core App 2.0 tests run from VS/VSTest the entry assembly is "testhost.dll"
			//so we need to explicitly load the configuration
			if (ExecutingWithVsTest)
			{
				Settings.ConfigurationManager = new NetCoreConfigurationManager();
			}
		}

		class NetCoreConfigurationManager : IConfigurationManager
		{
			private readonly System.Configuration.Configuration _configuration;

			public NetCoreConfigurationManager()
			{
				var assemblyPath =
					Path.Combine(TestContext.CurrentContext.TestDirectory, Path.GetFileName(typeof(TestsContext).Assembly.Location));
				_configuration = ConfigurationManager.OpenExeConfiguration(assemblyPath);
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
#endif
	}
}
