using System;
using System.Data.Common;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
	public class DbProviderFactoryDriveConnectionCommandProviderTest
	{
		[Test]
		public void WhenCreatedWithNullDbFactoryThenThrows()
		{
#if NETCOREAPP2_0
			Assert.Ignore("Not applicable for NETCOREAPP2_0");
#else
			Assert.That(() => new DbProviderFactoryDriveConnectionCommandProvider(null), Throws.TypeOf<ArgumentNullException>());
#endif
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnConnection()
		{
#if NETCOREAPP2_0
			Assert.Ignore("Not applicable for NETCOREAPP2_0");
#else
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using(var connection =provider.CreateConnection())
			{
				Assert.That(connection, Is.Not.Null);
			}
#endif
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnCommand()
		{
#if NETCOREAPP2_0
			Assert.Ignore("Not applicable for NETCOREAPP2_0");
#else
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using (var command = provider.CreateCommand())
			{
				Assert.That(command, Is.Not.Null);
			}
#endif
		}
	}
}
