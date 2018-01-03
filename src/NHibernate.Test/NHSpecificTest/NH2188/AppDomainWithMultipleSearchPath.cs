using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2188
{
	[TestFixture]
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
#if NETCOREAPP2_0
			Assert.Ignore("Not applicable for NETCOREAPP2_0");
#else
			// NUnit 3 sets PrivateBinPath when using an NUnit project, so we need to reset back to the correct setting when done.
			var privatePath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

			string binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			var expected = Path.Combine(binPath, Configuration.DefaultHibernateCfgFileName);

			// Test uses an obsolete .Net feature, changing the private path of an ongoing AppDomain.
			// If that feature is dropped, the tests will likely need to be dropped too. (Or heavily changed
			// for running in a newly created AppDomain.)
#pragma warning disable CS0618 // Type or member is obsolete
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
#pragma warning restore CS0618 // Type or member is obsolete
#endif
		}
	}
}
