using System.Collections;
using NHibernate.Cfg;
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

		[Test]
		public void ExceptionsInBeforeTransactionCompletionAbortTransaction()
		{
			Assert.IsFalse(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptions);

			var c = new C {ID = 1, Value = "value"};

			var sessionInterceptor = new SessionInterceptorThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = sessions.OpenSession(sessionInterceptor))
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(c);

				Assert.Throws<BadException>(t.Commit);
			}

			using (ISession s = sessions.OpenSession())
			{
				var objectInDb = s.Get<C>(1);
				Assert.IsNull(objectInDb);
			}
		}
	}

	[TestFixture]
	public class OldBehaviorEnabledFixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1082"; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.InterceptorsBeforeTransactionCompletionIgnoreExceptions, "true");
			base.Configure(configuration);
		}

		[Test]
		public void ExceptionsInBeforeTransactionCompletionAreIgnored()
		{
			Assert.IsTrue(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptions);

			var c = new C {ID = 1, Value = "value"};

			var sessionInterceptor = new SessionInterceptorThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = sessions.OpenSession(sessionInterceptor))
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(c);

				Assert.DoesNotThrow(t.Commit);
			}

			using (ISession s = sessions.OpenSession())
			{
				var objectInDb = s.Get<C>(1);

				Assert.IsNotNull(objectInDb);

				s.Delete(objectInDb);
				s.Flush();
			}
		}
	}
}
