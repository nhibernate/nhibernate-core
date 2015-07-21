using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1864
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s=> s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}

		[Test]
		public void FilterOnOffOn()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => { }));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}

		[Test]
		public void FilterQueryTwice()
		{
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
			Assert.DoesNotThrow(() => ExecuteQuery(s => s.EnableFilter("validity").SetParameter("date", DateTime.Now)));
		}
		
		private void ExecuteQuery(Action<ISession> sessionModifier)
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					sessionModifier(session);
					
					session
						.CreateQuery(@"select cat from Invoice inv, Category cat where cat.ValidUntil = :now and inv.Foo = :foo")
						.SetInt32("foo", 42)
						.SetDateTime("now", DateTime.Now)
						.List();

					tx.Commit();
				}
			}
		}
	}
}
