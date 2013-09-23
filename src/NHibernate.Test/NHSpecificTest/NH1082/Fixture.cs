using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1082
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1082"; }
		}

		/* Exceptions thrown in IInterceptor.BeforeTransactionCompletion should cause the transaction to be rolled back
		   */
		[Test]
		public void TestBug()
		{
			C c = new C();
			c.ID = 1;
			c.Value = "value";

			var sessionInterceptor = new SessionInterceptorThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = sessions.OpenSession(sessionInterceptor))
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(c);
				try
				{
					t.Commit();
					Assert.Fail("BadException expected");
				}
				catch (BadException)
				{
				}
			}

			using (ISession s = sessions.OpenSession())
			{
				var objectInDb = s.Get<C>(1);
				Assert.IsNull(objectInDb);
			}
		}
	}
}
