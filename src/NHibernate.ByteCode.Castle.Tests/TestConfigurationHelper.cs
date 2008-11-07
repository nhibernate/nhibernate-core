using System;
using System.IO;
using NHibernate.Cfg;

namespace NHibernate.ByteCode.Castle.Tests
{
	public static class TestConfigurationHelper
	{
		public static readonly string hibernateConfigFile;

		static TestConfigurationHelper()
		{
			// Verify if hibernate.cfg.xml exists
			hibernateConfigFile = GetDefaultConfigurationFilePath();
		}

		public static string GetDefaultConfigurationFilePath()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
			string fullPath = Path.Combine(binPath, Configuration.DefaultHibernateCfgFileName);
			return File.Exists(fullPath) ? fullPath : null;
		}

		/// <summary>
		/// Standar Configuration for tests.
		/// </summary>
		/// <returns>The configuration using merge between App.Config and hibernate.cfg.xml if present.</returns>
		public static Configuration GetDefaultConfiguration()
		{
			var result = new Configuration();
			if (hibernateConfigFile != null)
			{
				result.Configure(hibernateConfigFile);
			}
			return result;
		}
	}
}