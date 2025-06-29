using log4net;
using NHibernate.Cfg;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3657
{
	[TestFixture]
	public class Fixture
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(Fixture));
		private const string TestSessionFactoryName = "TestName";

		private Configuration _cfg;
		private ISessionFactory _builtSessionFactory;

		[OneTimeSetUp]
		public void TestFixtureSetUp()
		{
			_cfg = TestConfigurationHelper.GetDefaultConfiguration();
			var type = GetType();
			_cfg.AddResource(type.Namespace + ".Mappings.hbm.xml", type.Assembly);
			_cfg.SetProperty(Environment.SessionFactoryName, TestSessionFactoryName);
		}

		[TearDown]
		public void TearDown()
		{
			_builtSessionFactory?.Dispose();
			_builtSessionFactory = null;
		}

		private ISessionFactory SessionFactoryBuilder()
		{
			Assert.That(_builtSessionFactory, Is.Null, "SessionFactory was already built");

			_builtSessionFactory = _cfg.BuildSessionFactory();
			_log.Info("Successfully built session factory");

			return _builtSessionFactory;
		}

		[Test]
		public void GetOrAddTwice()
		{
			var factory = SessionFactoryObjectFactory.GetOrBuildNamedInstance(TestSessionFactoryName, SessionFactoryBuilder);
			Assert.That(factory, Is.Not.Null, "Failed to get the factory once");

			var factory2 = SessionFactoryObjectFactory.GetOrBuildNamedInstance(TestSessionFactoryName, SessionFactoryBuilder);
			Assert.That(factory2, Is.Not.Null, "Failed to get the factory twice");
			Assert.That(factory, Is.SameAs(factory2), "The two factories should be the same");
		}
	}
}
