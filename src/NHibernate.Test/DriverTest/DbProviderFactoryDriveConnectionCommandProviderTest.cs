using System;
using System.Data.Common;
using NHibernate.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DriverTest
{
	public class DbProviderFactoryDriveConnectionCommandProviderTest
	{
		[Test]
		public void WhenCreatedWithNullDbFactoryThenThrows()
		{
			Executing.This(() => new DbProviderFactoryDriveConnectionCommandProvider(null)).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnConnection()
		{
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using(var connection =provider.CreateConnection())
			{
				connection.Should().Not.Be.Null();
			}
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnCommand()
		{
			var factory = DbProviderFactories.GetFactory("System.Data.OracleClient");
			var provider = new DbProviderFactoryDriveConnectionCommandProvider(factory);
			using (var command = provider.CreateCommand())
			{
				command.Should().Not.Be.Null();
			}
		}
	}
}