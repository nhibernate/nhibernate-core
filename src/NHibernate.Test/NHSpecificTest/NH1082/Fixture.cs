using System;
using NHibernate.Cfg;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

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
#pragma warning disable 618
			Assert.IsFalse(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptionsEnabled);
#pragma warning restore 618

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


		[Test]
		public void ExceptionsInSynchronizationBeforeTransactionCompletionAbortTransaction()
		{
#pragma warning disable 618
			Assert.IsFalse(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptionsEnabled);
#pragma warning restore 618

			var c = new C { ID = 1, Value = "value" };

			var synchronization = new SynchronizationThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = sessions.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				t.RegisterSynchronization(synchronization);

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
	[Obsolete("Can be removed when Environment.InterceptorsBeforeTransactionCompletionIgnoreExceptions is removed.")]
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
			Assert.IsTrue(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptionsEnabled);

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


		[Test]
		public void ExceptionsInSynchronizationBeforeTransactionCompletionAreIgnored()
		{
			Assert.IsTrue(sessions.Settings.IsInterceptorsBeforeTransactionCompletionIgnoreExceptionsEnabled);

			var c = new C { ID = 1, Value = "value" };

			var synchronization = new SynchronizationThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = sessions.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				t.RegisterSynchronization(synchronization);

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
