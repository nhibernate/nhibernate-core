using System;
using System.Collections;
using log4net;
using NUnit.Framework;

namespace NHibernate.Test.Stateless.Fetching
{
	[TestFixture]
	public class StatelessSessionFetchingTest : TestCase
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(StatelessSessionFetchingTest));

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new[] { "Stateless.Fetching.Mappings.hbm.xml" };
			}
		}

		[Test]
		public void DynamicFetch()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				DateTime now = DateTime.Now;
				User me = new User("me");
				User you = new User("you");
				Resource yourClock = new Resource("clock", you);
				Task task = new Task(me, "clean", yourClock, now); // :)
				s.Save(me);
				s.Save(you);
				s.Save(yourClock);
				s.Save(task);
				tx.Commit();
			}

			using (IStatelessSession ss = sessions.OpenStatelessSession())
			using (ITransaction tx = ss.BeginTransaction())
			{
				ss.BeginTransaction();
				Task taskRef =
					(Task)ss.CreateQuery("from Task t join fetch t.Resource join fetch t.User").UniqueResult();
				Assert.That(taskRef, Is.Not.Null);
				Assert.That(NHibernateUtil.IsInitialized(taskRef), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(taskRef.User), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(taskRef.Resource), Is.True);
				Assert.That(NHibernateUtil.IsInitialized(taskRef.Resource.Owner), Is.False);
				tx.Commit();
			}

			cleanup();
		}

		private void cleanup()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.BeginTransaction();
				s.CreateQuery("delete Task").ExecuteUpdate();
				s.CreateQuery("delete Resource").ExecuteUpdate();
				s.CreateQuery("delete User").ExecuteUpdate();
				tx.Commit();
			}
		}
	}

}
