using System.Collections;
using System.Threading;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class MapBasedSessionContextFixture : ConnectionManagementTestCase
	{
		protected override ISession GetSessionUnderTest()
		{
			ISession session = OpenSession();
			session.BeginTransaction();
			return session;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.CurrentSessionContextClass, typeof(TestableMapBasedSessionContext).AssemblyQualifiedName);
		}

		protected override void OnSetUp()
		{
			TestableMapBasedSessionContext._map = null;
		}

		[Test]
		public void MapContextThreadSafety()
		{
			using (var factory1 = cfg.BuildSessionFactory())
			using (var session1 = factory1.OpenSession())
			using (var factory2 = cfg.BuildSessionFactory())
			using (var session2 = factory2.OpenSession())
			{
				var thread1 = new Thread(() =>
				{
					CurrentSessionContext.Bind(session1);
				});

				var thread2 = new Thread(() =>
				{
					CurrentSessionContext.Bind(session2);
				});

				thread1.Start();
				thread2.Start();
				thread1.Join();
				thread2.Join();

				Assert.IsTrue(CurrentSessionContext.HasBind(factory1), $"No session bound to \"{nameof(factory1)}\" factory.");
				Assert.IsTrue(CurrentSessionContext.HasBind(factory2), $"No session bound to \"{nameof(factory2)}\" factory.");
			}
		}
	}

	public class TestableMapBasedSessionContext : MapBasedSessionContext
	{
		public TestableMapBasedSessionContext(ISessionFactoryImplementor factory) : base(factory) { }

		// Context is the app with such implementation. Just for the test case.
		internal static IDictionary _map;

		protected override IDictionary GetMap()
		{
			return _map;
		}

		protected override void SetMap(IDictionary value)
		{
			// Give a fair chance to have a concurrency bug if base implementation is not thread safe.
			Thread.Sleep(100);
			_map = value;
		}
	}
}