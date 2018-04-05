using System.Configuration;
using System.IO;
using log4net.Repository.Hierarchy;
using NHibernate.Cfg;
using NHibernate.Cfg.ConfigurationSchema;
using NUnit.Framework;

namespace NHibernate.Test
{
	[SetUpFixture]
	public class TestsContext
	{
		public static bool ExecutingWithVsTest { get; } =
			System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name.StartsWith("testhost") ?? false;

		public static void AssumeSystemTypeIsSerializable() =>
			Assume.That(typeof(System.Type).IsSerializable, Is.True);

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			//When .NET Core App 2.0 tests run from VS/VSTest the entry assembly is "testhost.dll"
			//Dotnet test can run from "testhost.exe" or "testhost.x86.exe"
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
	}
}
