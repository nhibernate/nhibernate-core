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
			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			var expected = Path.Combine(binPath, Configuration.DefaultHibernateCfgFileName);

			try
			{
				AppDomain.CurrentDomain.AppendPrivatePath("bin");
				AppDomain.CurrentDomain.AppendPrivatePath("DbScripts");
				var configuration = new MyNhConfiguration();
				Assert.That(configuration.DefaultConfigurationFilePath(), Is.EqualTo(expected));
			}
			finally
			{
				AppDomain.CurrentDomain.ClearPrivatePath();
			}
		}
	}
}