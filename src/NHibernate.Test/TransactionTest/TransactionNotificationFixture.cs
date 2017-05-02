using System.Collections;
using System.Data.Common;
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
			using (sessions.WithOptions().Interceptor(interceptor).OpenSession())
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
			using (var session = sessions.WithOptions().Interceptor(interceptor).OpenSession())
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
			using (var session = sessions.WithOptions().Interceptor(interceptor).OpenSession())
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
			using (var session = sessions.WithOptions().Interceptor(interceptor).OpenSession())
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

			using (var ownConnection = sessions.ConnectionProvider.GetConnection())
			{
				using (s = sessions.WithOptions().Connection(ownConnection).Interceptor(interceptor).OpenSession())
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