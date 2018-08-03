using System;
using System.Collections.Generic;
using NHibernate.Bytecode;
using NHibernate.Connection;
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
				new Dictionary<string, string> {{ "conn", typeof(DriverConnectionProvider).AssemblyQualifiedName } },
				typeof(DebugConnectionProvider));
			Assert.That(instance, Is.Not.Null);
			Assert.That(instance, Is.TypeOf<DriverConnectionProvider>());
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
		public void GetInstanceByInvalidProperty()
		{
			Assert.Throws<HibernateException>(
				() =>
				{
					PropertiesHelper.GetInstance<IConnectionProvider>(
						"conn",
						new Dictionary<string, string> {{"conn", typeof(PropertiesHelperTest).AssemblyQualifiedName}},
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
