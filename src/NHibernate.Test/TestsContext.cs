using NUnit.Framework;

#if NETCOREAPP2_0
using System.Configuration;
using System.IO;
using log4net.Repository.Hierarchy;
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
		public static bool ExecutingWithVsTest { get; } =
			System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name == "testhost";

#if NETCOREAPP2_0
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			//When .NET Core App 2.0 tests run from VS/VSTest the entry assembly is "testhost.dll"
			//so we need to explicitly load the configuration
			if (ExecutingWithVsTest)
			{
				Environment.InitializeGlobalProperties(GetTestAssemblyHibernateConfiguration());
			}

			ConfigureLog4Net();
		}

		public static IHibernateConfiguration GetTestAssemblyHibernateConfiguration()
		{
			var assemblyPath =
 Path.Combine(TestContext.CurrentContext.TestDirectory, Path.GetFileName(typeof(TestsContext).Assembly.Location));
			var configuration = ConfigurationManager.OpenExeConfiguration(assemblyPath);
			var section = configuration.GetSection(CfgXmlHelper.CfgSectionName);
			return HibernateConfiguration.FromAppConfig(section.SectionInformation.GetRawXml());
		}

		private static void ConfigureLog4Net()
		{
			var hierarchy = (Hierarchy)log4net.LogManager.GetRepository(typeof(TestsContext).Assembly);

			var consoleAppender = new log4net.Appender.ConsoleAppender
			{
				Layout = new log4net.Layout.PatternLayout("%d{ABSOLUTE} %-5p %c{1}:%L - %m%n"),
			};

			((Logger)hierarchy.GetLogger("NHibernate.Hql.Ast.ANTLR")).Level = log4net.Core.Level.Off;
			((Logger)hierarchy.GetLogger("NHibernate.SQL")).Level = log4net.Core.Level.Off;
			((Logger)hierarchy.GetLogger("NHibernate.AdoNet.AbstractBatcher")).Level = log4net.Core.Level.Off;
			((Logger)hierarchy.GetLogger("NHibernate.Tool.hbm2ddl.SchemaExport")).Level = log4net.Core.Level.Error;
			hierarchy.Root.Level = log4net.Core.Level.Warn;
			hierarchy.Root.AddAppender(consoleAppender);
			hierarchy.Configured = true;
		}
#endif
	}
}
