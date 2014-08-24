using System;
using NHibernate.Driver;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.DriverTest
{
	public class ReflectionBasedDriverTest
	{
		private class MyDriverWithWrongClassesAndGoodDbProviderFactory : ReflectionBasedDriver
		{
			public MyDriverWithWrongClassesAndGoodDbProviderFactory()
				: base("System.Data.OracleClient", "pizza1", "pizza2", "pizza3")
			{
			}
			public MyDriverWithWrongClassesAndGoodDbProviderFactory(string assemblyName)
				: base("System.Data.OracleClient", assemblyName, "pizza2", "pizza3")
			{
			}

			public override bool UseNamedPrefixInSql
			{
				get { throw new NotImplementedException(); }
			}

			public override bool UseNamedPrefixInParameter
			{
				get { throw new NotImplementedException(); }
			}

			public override string NamedPrefix
			{
				get { throw new NotImplementedException(); }
			}
		}
		private class MyDriverWithNoDbProviderFactory : ReflectionBasedDriver
		{
			public MyDriverWithNoDbProviderFactory():
			base(null,
				"System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", 
			"System.Data.OracleClient.OracleConnection", 
			"System.Data.OracleClient.OracleCommand") { }

			public override bool UseNamedPrefixInSql
			{
				get { throw new NotImplementedException(); }
			}

			public override bool UseNamedPrefixInParameter
			{
				get { throw new NotImplementedException(); }
			}

			public override string NamedPrefix
			{
				get { throw new NotImplementedException(); }
			}
		}

		[Test]
		public void WhenCreatedWithGoodDbProviderThenNotThrows()
		{
			Executing.This(()=> new MyDriverWithWrongClassesAndGoodDbProviderFactory()).Should().NotThrow();
		}

		[Test]
		public void WhenCreatedWithNullAssemblyAndGoodDbProviderThenNotThrows()
		{
			Executing.This(() => new MyDriverWithWrongClassesAndGoodDbProviderFactory(null)).Should().NotThrow();
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnConnection()
		{
			var provider = new MyDriverWithWrongClassesAndGoodDbProviderFactory();
			using (var connection = provider.CreateConnection())
			{
				connection.Should().Not.Be.Null();
			}
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnCommand()
		{
			var provider = new MyDriverWithWrongClassesAndGoodDbProviderFactory();
			using (var command = provider.CreateCommand())
			{
				command.Should().Not.Be.Null();
			}
		}

		[Test]
		public void WhenCreatedWithNoDbProviderThenNotThrows()
		{
			Executing.This(() => new MyDriverWithNoDbProviderFactory()).Should().NotThrow();
		}

		[Test]
		public void WhenCreatedWithNoDbFactoryThenCanReturnConnection()
		{
			var provider = new MyDriverWithNoDbProviderFactory();
			using (var connection = provider.CreateConnection())
			{
				connection.Should().Not.Be.Null();
			}
		}

		[Test]
		public void WhenCreatedNoWithDbFactoryThenCanReturnCommand()
		{
			var provider = new MyDriverWithNoDbProviderFactory();
			using (var command = provider.CreateCommand())
			{
				command.Should().Not.Be.Null();
			}
		}
	}
}