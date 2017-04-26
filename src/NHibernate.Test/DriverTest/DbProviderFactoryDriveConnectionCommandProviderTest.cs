using System;
using System.Data.Common;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	public class DbProviderFactoryDriveConnectionCommandProviderTest
	{
		[Test]
		public void WhenCreatedWithNullDbFactoryThenThrows()
		{
			Assert.That(() => new DbProviderFactoryDriveConnectionCommandProvider(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnConnection()
		{
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using(var connection =provider.CreateConnection())
			{
				Assert.That(connection, Is.Not.Null);
			}
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnCommand()
		{
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using (var command = provider.CreateCommand())
			{
				Assert.That(command, Is.Not.Null);
			}
		}
	}
}