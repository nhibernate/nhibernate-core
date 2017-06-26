using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Context;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class AsyncLocalSessionContextFixture : ConnectionManagementTestCase
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
			cfg.SetProperty(Environment.CurrentSessionContextClass, "async_local");
		}

		[Test]
		public async Task AsyncLocalIsolation()
		{
			using (var session = OpenSession())
			{
				CurrentSessionContext.Bind(session);
				AssertCurrentSession(session, "Unexpected session after outer bind.");

				await SubBind(session);
				AssertCurrentSession(session, "Unexpected session after end of SubBind call.");
			}
		}

		private async Task SubBind(ISession firstSession)
		{
			AssertCurrentSession(firstSession, "Unexpected session at start of SubBind.");

			using (var session = OpenSession())
			{
				CurrentSessionContext.Bind(session);
				AssertCurrentSession(session, "Unexpected session after inner bind.");

				await Dummy();
				AssertCurrentSession(session, "Unexpected session after dummy await.");
			}
		}

		private Task Dummy()
		{
			return Task.FromResult(0);
		}

		private void AssertCurrentSession(ISession session, string message)
		{
			Assert.That(
				Sfi.GetCurrentSession(),
				Is.EqualTo(session),
				"{0} {1} instead of {2}.", message,
				Sfi.GetCurrentSession().GetSessionImplementation().SessionId,
				session.GetSessionImplementation().SessionId);
		}
	}
}