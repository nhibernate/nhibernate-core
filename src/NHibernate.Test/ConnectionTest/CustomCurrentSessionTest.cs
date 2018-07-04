using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class CustomCurrentSessionTest : ConnectionManagementTestCase
	{
		protected override ISession GetSessionUnderTest()
		{
			var session = OpenSession();
			CustomContext.Session = session;
			return session;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.CurrentSessionContextClass, typeof(CustomContext).AssemblyQualifiedName);
		}

		protected override void Release(ISession session)
		{
			CustomContext.Session = null;
			base.Release(session);
		}

		[Test]
		public void ContextIsSetup()
		{
			Assert.That(Sfi.CurrentSessionContext, Is.InstanceOf<CustomContext>());
			Assert.That(
				((CustomContext) Sfi.CurrentSessionContext).Factory,
				Is.SameAs(((DebugSessionFactory) Sfi).ActualFactory));
		}
	}

	public class CustomContext : ICurrentSessionContextWithFactory
	{
		internal ISessionFactoryImplementor Factory;
		internal static ISession Session;

		public ISession CurrentSession()
		{
			return Session;
		}

		public void SetFactory(ISessionFactoryImplementor factory)
		{
			Factory = factory;
		}
	}
}
