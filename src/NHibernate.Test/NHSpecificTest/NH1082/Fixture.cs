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

	[TestFixture]
	public class OldBehaviorEnabledFixture : TestCase
	{
		protected override IList Mappings
		{
			get
			{
				return new string[]
				{
					"NHSpecificTest.NH1082.Mappings.hbm.xml"
				};
			}
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
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
				}
				catch (BadException)
				{
					Assert.Fail("BadException not expected");
				}
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
