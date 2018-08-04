using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NHibernate.Connection;
using NHibernate.Linq.Functions;
using NHibernate.Test.CfgTest;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.UtilityTest
{
	[TestFixture]
	public class PropertiesHelperTest
	{
		[Test]
		public void WhenInvalidBoolValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> {{"myProp", "pizza"}}, false), Is.False);
		}

		[Test]
		public void WhenInvalidInt32ValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5), Is.EqualTo(5));
		}

		[Test]
		public void WhenInvalidInt64ValueThenUseDefault()
		{
			Assert.That(PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", "pizza" } }, 5), Is.EqualTo(5));
		}

		[Test]
		public void WhenValidBoolValueThenValue()
		{
			Assert.That(PropertiesHelper.GetBoolean("myProp", new Dictionary<string, string> { { "myProp", "true" } }, false), Is.True);
		}

		[Test]
		public void WhenValidInt32ValueThenValue()
		{
			Assert.That(PropertiesHelper.GetInt32("myProp", new Dictionary<string, string> { { "myProp", int.MaxValue.ToString() } }, 5), Is.EqualTo(int.MaxValue));
		}

		[Test]
		public void WhenValidInt64ValueThenValue()
		{
			Assert.That(PropertiesHelper.GetInt64("myProp", new Dictionary<string, string> { { "myProp", long.MaxValue.ToString() } }, 5), Is.EqualTo(long.MaxValue));
		}

		[Test]
		public void GetInstanceByDefault()
		{
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string>(),
				typeof(DebugConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DebugConnectionProvider>());
		}

		[Test]
		public void GetInstanceByDefaultNull()
		{
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string>(),
				null);
			Assert.That(instance, Is.Null);
		}

		[Test]
		public void GetInstanceByDefaultWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string>(),
				typeof(DriverConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DriverConnectionProvider>());
		}

		[Test]
		public void GetExternalInstanceByDefaultWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string>(),
						typeof(DebugConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByRegistration()
		{
			var sp = new SimpleServiceProvider();
			sp.Register<IConnectionProvider, DriverConnectionProvider>();
			Cfg.Environment.ServiceProvider = sp;
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string>(),
				typeof(DebugConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DriverConnectionProvider>());
		}

		[Test]
		public void GetInstanceByProperty()
		{
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string> {{"conn", typeof(DriverConnectionProvider).AssemblyQualifiedName}},
				typeof(DebugConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DriverConnectionProvider>());
		}

		[Test]
		public void GetInstanceByPropertyWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			var instance = PropertiesHelper.GetInstance<IConnectionProvider>(
				"conn",
				new Dictionary<string, string> {{"conn", typeof(DriverConnectionProvider).AssemblyQualifiedName}},
				typeof(DebugConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DriverConnectionProvider>());
		}

		[Test]
		public void GetExternalInstanceByPropertyWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", typeof(DebugConnectionProvider).AssemblyQualifiedName}},
						typeof(DriverConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByInvalidDefault()
		{
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string>(),
						typeof(PropertiesHelperTest));
				});
		}

		[Test]
		public void GetInstanceByInvalidRegistration()
		{
			var sp = new SimpleServiceProvider();
			sp.Register(typeof(IConnectionProvider), () => new PropertiesHelperTest());
			Cfg.Environment.ServiceProvider = sp;
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string>(),
						typeof(DriverConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByInvalidPropertyClassType()
		{
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", typeof(DefaultLinqToHqlGeneratorsRegistry).AssemblyQualifiedName}},
						typeof(DriverConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByInvalidPropertyClassTypeWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", typeof(DefaultLinqToHqlGeneratorsRegistry).AssemblyQualifiedName}},
						typeof(DriverConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByInvalidPropertyClassName()
		{
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", "test"}},
						typeof(DriverConnectionProvider));
				});
		}

		[Test]
		public void GetInstanceByInvalidPropertyClassNameWithExplicitServiceProvider()
		{
			Cfg.Environment.ServiceProvider = new SimpleServiceProvider(true);
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", "test"}},
						typeof(DriverConnectionProvider));
				});
		}

		private IServiceProvider _originalSp;

		[SetUp]
		public void Setup()
		{
			_originalSp = Cfg.Environment.ServiceProvider;
		}

		[TearDown]
		public void TearDown()
		{
			Cfg.Environment.ServiceProvider = _originalSp;
		}
	}
}
