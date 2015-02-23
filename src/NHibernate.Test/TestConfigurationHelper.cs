using System.Reflection;

namespace NHibernate.Test
{
	using System;
	using System.IO;
	using Cfg;

	public static class TestConfigurationHelper
	{
	    private const string CurrentTestConfiguration = "current-test-configuration";

		public static readonly string hibernateConfigFile;

		static TestConfigurationHelper()
		{
			// Verify if hibernate.cfg.xml exists
			hibernateConfigFile = GetDefaultConfigurationFilePath();

            // Set up path to load extra binary files if needed (ex. Firebird).
            if (hibernateConfigFile != null)
                System.Environment.SetEnvironmentVariable("PATH", Path.GetDirectoryName(hibernateConfigFile) + ";" + System.Environment.GetEnvironmentVariable("PATH"));

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string simpleAssemblyName = args.Name.Split(',')[0];
            string filename = FindCurrentTestConfigurationFile(simpleAssemblyName + ".dll");
            if (filename != null)
                return Assembly.LoadFrom(filename);
            return null;
        }

		public static string GetDefaultConfigurationFilePath()
		{
		    return FindCurrentTestConfigurationFile(Configuration.DefaultHibernateCfgFileName);
		}

		/// <summary>
		/// Standard Configuration for tests.
		/// </summary>
		/// <returns>The configuration using merge between App.Config and hibernate.cfg.xml if present.</returns>
		public static Configuration GetDefaultConfiguration()
		{
			Configuration result = new Configuration();
			if (hibernateConfigFile != null)
				result.Configure(hibernateConfigFile);
			return result;
		}

        private static string FindCurrentTestConfigurationFile(string filename)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
            string folder = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);

            // Check for the file right in the working folder first.
            if (File.Exists(Path.Combine(folder, filename)))
                return Path.Combine(folder, filename);

            // Look for a "current-test-configuration" folder up in all the parent folders.
            while (folder != null)
            {
                string current = Path.Combine(Path.Combine(folder, CurrentTestConfiguration), filename);
                if (File.Exists(current))
                    return current;
                folder = Path.GetDirectoryName(folder);
            }
            return null;
        }
	}
}