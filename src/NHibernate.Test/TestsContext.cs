using NUnit.Framework;
using System.Configuration;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate.Cfg;

namespace NHibernate.Test
{
	[SetUpFixture]
	public class TestsContext
	{
		private static readonly Assembly TestAssembly = typeof(TestsContext).Assembly;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			ConfigureLog4Net();

			//When .NET Core App 2.0 tests run from VS/VSTest the entry assembly is "testhost.dll"
			//so we need to explicitly load the configuration
			if (Assembly.GetEntryAssembly() != null)
			{
				ConfigurationProvider.Current = new SystemConfigurationProvider(ConfigurationManager.OpenExeConfiguration(TestAssembly.Location));
			}
		}

		private static void ConfigureLog4Net()
		{
			using (var log4NetXml = TestAssembly.GetManifestResourceStream("NHibernate.Test.log4net.xml"))
				XmlConfigurator.Configure(LogManager.GetRepository(TestAssembly), log4NetXml);
		}
	}
}
