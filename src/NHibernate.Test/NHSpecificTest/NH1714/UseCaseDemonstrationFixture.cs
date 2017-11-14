using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1714
{
	[TestFixture]
	public class UseCaseDemonstrationFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			var listener = new IPreInsertEventListener[this.cfg.EventListeners.PreInsertEventListeners.Length + 1];
			this.cfg.EventListeners.PreInsertEventListeners.CopyTo(listener, 0);
			listener[listener.Length - 1] = new MyCustomEventListener();

			this.cfg.EventListeners.PreInsertEventListeners = listener;
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect as MsSql2005Dialect != null;
		}

		[Test]
		public void DbCommandsFromEventListenerShouldBeEnlistedInRunningTransaction()
		{
			using (ISession session = this.OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					var entity = new DomainClass();
					session.Save(entity);

					tx.Commit();
				}
			}

			using (ISession session = this.OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					session.Delete("from DomainClass");
					session.Delete("from LogClass");
					tx.Commit();
				}
			}
		}
	}

	public partial class MyCustomEventListener : IPreInsertEventListener
	{
		public bool OnPreInsert(PreInsertEvent e)
		{
			if(e.Entity is DomainClass == false)
				return false;

			// this will join into the parent's transaction
			using (var session = e.Session.SessionWithOptions().Connection().OpenSession())
			{
				//should insert log record here
				session.Save(new LogClass());
				session.Flush();
			}

			return false;
		}
	}
}