using System;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.DriverTest
{
	[TestFixture]
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
			public MyDriverWithNoDbProviderFactory() : base(
				null,
#if NETFX
				"System.Data.OracleClient, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
#else
				"System.Data.OracleClient",				
#endif
				"System.Data.OracleClient.OracleConnection",
				"System.Data.OracleClient.OracleCommand")
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

#if NETFX
		[Test]
		public void WhenCreatedWithGoodDbProviderThenNotThrows()
		{
			Assert.That(() => new MyDriverWithWrongClassesAndGoodDbProviderFactory(), Throws.Nothing);
		}

		[Test]
		public void WhenCreatedWithNullAssemblyAndGoodDbProviderThenNotThrows()
		{
			Assert.That(() => new MyDriverWithWrongClassesAndGoodDbProviderFactory(null), Throws.Nothing);
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnConnection()
		{
			var provider = new MyDriverWithWrongClassesAndGoodDbProviderFactory();
			using (var connection = provider.CreateConnection())
			{
				Assert.That(connection, Is.Not.Null);
			}
		}

		[Test]
		public void WhenCreatedWithDbFactoryThenCanReturnCommand()
		{
			var provider = new MyDriverWithWrongClassesAndGoodDbProviderFactory();
			using (var command = provider.CreateCommand())
			{
				Assert.That(command, Is.Not.Null);
			}
		}
#endif

		[Test]
		public void WhenCreatedWithNoDbProviderThenNotThrows()
		{
			Assert.That(() => new MyDriverWithNoDbProviderFactory(), Throws.Nothing);
		}

		[Test]
		public void WhenCreatedWithNoDbFactoryThenCanReturnConnection()
		{
			var provider = new MyDriverWithNoDbProviderFactory();
			using (var connection = provider.CreateConnection())
			{
				Assert.That(connection, Is.Not.Null);
			}
		}

		[Test]
		public void WhenCreatedNoWithDbFactoryThenCanReturnCommand()
		{
			var provider = new MyDriverWithNoDbProviderFactory();
			using (var command = provider.CreateCommand())
			{
				Assert.That(command, Is.Not.Null);
			}
		}
	}
}
