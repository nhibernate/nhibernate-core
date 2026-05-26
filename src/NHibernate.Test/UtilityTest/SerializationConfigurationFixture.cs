using System.Collections.Generic;
using System.Runtime.Serialization;
using NHibernate.Cfg;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	[NonParallelizable]
	public class SerializationConfigurationFixture
	{
		private ISerializationStrategy _initialStrategy;

		[SetUp]
		public void SetUp()
		{
			_initialStrategy = SerializationConfiguration.Strategy;
		}

		[TearDown]
		public void TearDown()
		{
			SerializationConfiguration.Strategy = _initialStrategy;
		}

		[Test]
		public void ThrowsByDefaultWhenConfiguredWithThrowingStrategy()
		{
			SerializationConfiguration.Strategy = new ThrowingSerializationStrategy();
			Assert.That(() => SerializationHelper.Serialize(new object()), Throws.TypeOf<SerializationException>());
		}

		[Test]
		public void CanUseBinaryFormatterStrategy()
		{
			TestConfigurationHelper.UseBinaryFormatterSerialization();
			var value = new[] { 1, 2, 3 };
			var bytes = SerializationHelper.Serialize(value);
			var copy = (int[]) SerializationHelper.Deserialize(bytes);

			Assert.That(copy, Is.EqualTo(value));
		}

		[Test]
		public void SessionFactoryPropertiesDoNotOverrideGlobalStrategy()
		{
			SerializationConfiguration.Strategy = new ThrowingSerializationStrategy();

			var properties = new Dictionary<string, string>
			{
				[Environment.Dialect] = typeof(Dialect.GenericDialect).AssemblyQualifiedName,
				[Environment.ConnectionDriver] = typeof(Driver.SqlClientDriver).AssemblyQualifiedName,
				[Environment.ConnectionString] = "Server=(localdb)\\MSSQLLocalDB;Database=master;Integrated Security=SSPI",
				[Environment.SerializationStrategy] = typeof(BinaryFormatterSerializationStrategy).AssemblyQualifiedName
			};

			var settingsFactory = new SettingsFactory();
			settingsFactory.BuildSettings(properties);

			Assert.That(SerializationConfiguration.Strategy, Is.TypeOf<ThrowingSerializationStrategy>());
		}

		[Test]
		public void InitializeGlobalPropertiesConfiguresSerializationStrategy()
		{
			SerializationConfiguration.Strategy = new ThrowingSerializationStrategy();

			var sessionFactoryConfiguration = new SessionFactoryConfigurationBase();
			sessionFactoryConfiguration.Properties[Environment.SerializationStrategy] =
				typeof(BinaryFormatterSerializationStrategy).AssemblyQualifiedName;

			var hibernateConfiguration = new StubHibernateConfiguration
			{
				ByteCodeProviderType = "lcg",
				UseReflectionOptimizer = true,
				SessionFactory = sessionFactoryConfiguration
			};

			Environment.InitializeGlobalProperties(hibernateConfiguration);

			Assert.That(SerializationConfiguration.Strategy, Is.TypeOf<BinaryFormatterSerializationStrategy>());
		}

		private sealed class StubHibernateConfiguration : IHibernateConfiguration
		{
			public string ByteCodeProviderType { get; set; }

			public bool UseReflectionOptimizer { get; set; }

			public ISessionFactoryConfiguration SessionFactory { get; set; }
		}
	}
}
