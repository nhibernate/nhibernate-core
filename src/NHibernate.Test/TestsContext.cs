using System.Configuration;
using System.IO;
using System.Xml;

using NHibernate.Cfg.ConfigurationSchema;
using NUnit.Framework;
using log4net.Repository.Hierarchy;

namespace NHibernate.Test
{
	[SetUpFixture]
	public class TestsContext
    {
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			HibernateConfiguration config = GetTestAssemblyHibernateConfiguration();
			NHibernate.Cfg.Environment.InitializeGlobalProperties(config);

			ConfigureLog4Net();
		}

		public static HibernateConfiguration GetTestAssemblyHibernateConfiguration()
		{
			HibernateConfiguration config;
			string assemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"NHibernate.Test.dll");
			var configuration = ConfigurationManager.OpenExeConfiguration(assemblyPath);
			ConfigurationSection configSection = configuration.GetSection(CfgXmlHelper.CfgSectionName);

			using (XmlTextReader reader = new XmlTextReader(configSection.SectionInformation.GetRawXml(), XmlNodeType.Document, null))
			{
				config = new HibernateConfiguration(reader);
			}

			return config;
		}

		private static void ConfigureLog4Net()
		{
			var hierarchy = (Hierarchy)log4net.LogManager.GetRepository(typeof(TestsContext).Assembly);

			var consoleAppender = new log4net.Appender.ConsoleAppender()
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
