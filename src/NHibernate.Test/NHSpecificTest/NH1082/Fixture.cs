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
			var c = new C {ID = 1, Value = "value"};

			var sessionInterceptor = new SessionInterceptorThatThrowsExceptionAtBeforeTransactionCompletion();
			using (var s = Sfi.WithOptions().Interceptor(sessionInterceptor).OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(c);

				Assert.Throws<BadException>(t.Commit);
			}

			using (ISession s = Sfi.OpenSession())
			{
				var objectInDb = s.Get<C>(1);
				Assert.IsNull(objectInDb);
			}
		}


		[Test]
		public void ExceptionsInSynchronizationBeforeTransactionCompletionAbortTransaction()
		{
			var c = new C { ID = 1, Value = "value" };

			var synchronization = new SynchronizationThatThrowsExceptionAtBeforeTransactionCompletion();
			using (ISession s = Sfi.OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				t.RegisterSynchronization(synchronization);

				s.Save(c);

				Assert.Throws<BadException>(t.Commit);
			}

			using (ISession s = Sfi.OpenSession())
			{
				var objectInDb = s.Get<C>(1);
				Assert.IsNull(objectInDb);
			}
		}
	}
}
