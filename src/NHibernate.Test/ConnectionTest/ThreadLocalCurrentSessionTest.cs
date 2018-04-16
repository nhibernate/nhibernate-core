using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture,Ignore("Not yet supported. Need AutoClosed feature.(TransactionContext)")]
	public class ThreadLocalCurrentSessionTest : ConnectionManagementTestCase
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
			cfg.SetProperty(Environment.CurrentSessionContextClass, typeof (TestableThreadLocalContext).AssemblyQualifiedName);
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override void Release(ISession session)
		{
			long initialCount = Sfi.Statistics.SessionCloseCount;
			session.Transaction.Commit();
			long subsequentCount = Sfi.Statistics.SessionCloseCount;
			Assert.AreEqual(initialCount + 1, subsequentCount, "Session still open after commit");
			// also make sure it was cleaned up from the internal ThreadLocal...
			Assert.IsFalse(TestableThreadLocalContext.HasBind(), "session still bound to internal ThreadLocal");
		}

		//TODO: Need AutoCloseEnabled feature after commit.
		[Test]
		public void ContextCleanup()
		{
			ISession session = Sfi.OpenSession();
			session.BeginTransaction();
			session.Transaction.Commit();
			Assert.IsFalse(session.IsOpen, "session open after txn completion");
			Assert.IsFalse(TestableThreadLocalContext.IsSessionBound(session), "session still bound after txn completion");
			
			ISession session2 = OpenSession();
			Assert.IsFalse(session.Equals(session2), "same session returned after txn completion");
			session2.Close();
			Assert.IsFalse(session2.IsOpen, "session open after closing");
			Assert.IsFalse(TestableThreadLocalContext.IsSessionBound(session2), "session still bound after closing");
		}

		[Test]
		public void TransactionProtection()
		{
			using (ISession session = OpenSession())
			{
				try
				{
					session.CreateQuery("from Silly");
					Assert.Fail("method other than beginTransaction{} allowed");
				}
				catch (HibernateException)
				{
					// ok
				}
			}
		}
	}

	public class TestableThreadLocalContext : ThreadLocalSessionContext
	{
		private static TestableThreadLocalContext me;

		public TestableThreadLocalContext(ISessionFactoryImplementor factory)
			: base(factory)
		{
			me = this;
		}

		public static bool IsSessionBound(ISession session)
		{
			return context != null && context.ContainsKey(me.factory)
			       && context[me.factory] == session;
		}

		public static bool HasBind()
		{
			return context != null && context.ContainsKey(me.factory);
		}
	}
}