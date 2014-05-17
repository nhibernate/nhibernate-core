using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.TransactionTest
{
	[TestFixture]
	public class TransactionNotificationFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] {}; }
		}


		[Test]
		public void NoTransaction()
		{
			var interceptor = new RecordingInterceptor();
			using (sessions.OpenSession(interceptor))
			{
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(0));
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0));
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(0));
			}
		}

		[Test]
		public void AfterBegin()
		{
			var interceptor = new RecordingInterceptor();
			using (ISession session = sessions.OpenSession(interceptor))
			using (session.BeginTransaction())
			{
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1));
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0));
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(0));
			}
		}

		[Test]
		public void Commit()
		{
			var interceptor = new RecordingInterceptor();
			using (ISession session = sessions.OpenSession(interceptor))
			{
				ITransaction tx = session.BeginTransaction();
				tx.Commit();
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1));
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(1));
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
			}
		}

		[Test]
		public void Rollback()
		{
			var interceptor = new RecordingInterceptor();
			using (ISession session = sessions.OpenSession(interceptor))
			{
				ITransaction tx = session.BeginTransaction();
				tx.Rollback();
				Assert.That(interceptor.afterTransactionBeginCalled, Is.EqualTo(1));
				Assert.That(interceptor.beforeTransactionCompletionCalled, Is.EqualTo(0));
				Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
			}
		}


		[Theory]
		[Description("NH2128")]
		public void ShouldNotifyAfterTransaction(bool usePrematureClose)
		{
			var interceptor = new RecordingInterceptor();
			ISession s;

			using (s = OpenSession(interceptor))
			using (s.BeginTransaction())
			{
				s.CreateCriteria<object>().List();

				// Call session close while still inside transaction?
				if (usePrematureClose)
					s.Close();
			}

			Assert.That(s.IsOpen, Is.False);
			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
		}


		[Description("NH2128")]
		[Theory]
		public void ShouldNotifyAfterTransactionWithOwnConnection(bool usePrematureClose)
		{
			var interceptor = new RecordingInterceptor();
			ISession s;

			using (IDbConnection ownConnection = sessions.ConnectionProvider.GetConnection())
			{
				using (s = sessions.OpenSession(ownConnection, interceptor))
				using (s.BeginTransaction())
				{
					s.CreateCriteria<object>().List();

					// Call session close while still inside transaction?
					if (usePrematureClose)
						s.Close();
				}
			}

			Assert.That(s.IsOpen, Is.False);
			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
		}
	}
}