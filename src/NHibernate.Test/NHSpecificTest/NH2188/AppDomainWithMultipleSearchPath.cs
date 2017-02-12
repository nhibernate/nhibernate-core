using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2188
{
	public class AppDomainWithMultipleSearchPath
	{
		public class MyNhConfiguration: Configuration
		{
			public string DefaultConfigurationFilePath()
			{
				return GetDefaultConfigurationFilePath();
			}
		}

		[Test]
		public void WhenSerchInMultiplePathsThenNotThrows()
		{
			// NUnit 3 sets PrivateBinPath when using an NUnit project, so we need to reset back to the correct setting when done.
			var privatePath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			var expected = Path.Combine(binPath, Configuration.DefaultHibernateCfgFileName);

			try
			{
				AppDomain.CurrentDomain.ClearPrivatePath();
				AppDomain.CurrentDomain.AppendPrivatePath("bin");
				AppDomain.CurrentDomain.AppendPrivatePath("DbScripts");
				var configuration = new MyNhConfiguration();
				Assert.That(configuration.DefaultConfigurationFilePath(), Is.EqualTo(expected));
			}
			finally
			{
				AppDomain.CurrentDomain.ClearPrivatePath();
				if (privatePath != null) AppDomain.CurrentDomain.AppendPrivatePath(privatePath);
			}
		}
	}
}
