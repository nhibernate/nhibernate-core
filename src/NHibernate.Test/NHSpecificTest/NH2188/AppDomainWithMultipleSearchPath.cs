using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;
using SharpTestsEx;

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
			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			var expected = Path.Combine(binPath, Configuration.DefaultHibernateCfgFileName);

			try
			{
				AppDomain.CurrentDomain.AppendPrivatePath("bin");
				AppDomain.CurrentDomain.AppendPrivatePath("DbScripts");
				var configuration = new MyNhConfiguration();
				configuration.DefaultConfigurationFilePath().Should().Be(expected);
			}
			finally
			{
				AppDomain.CurrentDomain.ClearPrivatePath();
			}
		}
	}
}