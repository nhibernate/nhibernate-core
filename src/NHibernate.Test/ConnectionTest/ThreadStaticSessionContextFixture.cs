using System.Collections;
using System.Threading;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class ThreadStaticSessionContextFixture : ConnectionManagementTestCase
	{
		protected override ISession GetSessionUnderTest()
		{
			var session = OpenSession();
			session.BeginTransaction();
			return session;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.CurrentSessionContextClass, "thread_static");
		}

		[Test]
		public void ThreadStaticIsolation()
		{
			using (var session1 = OpenSession())
			using (var session2 = OpenSession())
			{
				var thread1 = new Thread(() =>
				{
					for (var i = 1; i <= 100; i++)
					{
						CurrentSessionContext.Bind(session1);
						Thread.Sleep(1);
						// Yes, NUnit catches asserts inside threads.
						AssertCurrentSession(Sfi, session1, $"At iteration {i}, unexpected session for thread 1.");
					}
				});

				var thread2 = new Thread(() =>
				{
					for (var i = 1; i <= 34; i++)
					{
						CurrentSessionContext.Bind(session2);
						// Have a different longer sleep for ensuring the other thread have changed its own.
						Thread.Sleep(3);
						AssertCurrentSession(Sfi, session2, $"At iteration {i}, unexpected session for thread 2.");
					}
				});

				thread1.Start();
				thread2.Start();
				thread1.Join();
				thread2.Join();
			}
		}

		[Test]
		public void ThreadStaticMultiFactory()
		{
			using (var factory1 = cfg.BuildSessionFactory())
			using (var session1 = factory1.OpenSession())
			using (var factory2 = cfg.BuildSessionFactory())
			using (var session2 = factory2.OpenSession())
			{
				CurrentSessionContext.Bind(session1);
				AssertCurrentSession(factory1, session1, "Unexpected session for factory1 after bind of session1.");
				CurrentSessionContext.Bind(session2);
				AssertCurrentSession(factory2, session2, "Unexpected session for factory2 after bind of session2.");
				AssertCurrentSession(factory1, session1, "Unexpected session for factory1 after bind of session2.");
			}
		}

		private void AssertCurrentSession(ISessionFactory factory, ISession session, string message)
		{
			Assert.That(
				factory.GetCurrentSession(),
				Is.EqualTo(session),
				"{0} {1} instead of {2}.", message,
				factory.GetCurrentSession().GetSessionImplementation().SessionId,
				session.GetSessionImplementation().SessionId);
		}
	}
}
