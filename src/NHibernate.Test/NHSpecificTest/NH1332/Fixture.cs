using log4net;
using NHibernate.Cfg;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1332
{
	[TestFixture]
	public partial class Fixture : BugTestCase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Fixture));

		public override string BugNumber
		{
			get { return "NH1332"; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSecondLevelCache, "false");
			configuration.SetProperty(Environment.UseQueryCache, "false");
			configuration.SetProperty(Environment.CacheProvider, null);
			configuration.SetListener(ListenerType.PostCommitDelete, new PostCommitDelete());
		}

		[Test]
		public void Bug()
		{
			A a = new A("NH1332");
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				
				s.Save(a);
				tx.Commit();
			}
			using (LogSpy ls = new LogSpy(log))
			{
				using (ISession s = OpenSession())
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Delete(a);
					tx.Commit();
				}
				Assert.AreEqual(1, ls.Appender.GetEvents().Length);
				string logs = ls.Appender.GetEvents()[0].RenderedMessage;
				Assert.Greater(logs.IndexOf("PostCommitDelete fired."), -1);
			}
		}

		public partial class PostCommitDelete : IPostDeleteEventListener
		{
			public void OnPostDelete(PostDeleteEvent @event)
			{
				log.Debug("PostCommitDelete fired.");
			}
		}
	}
}
