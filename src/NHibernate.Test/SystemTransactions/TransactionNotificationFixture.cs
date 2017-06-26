using System;
using System.Collections;
using System.Data.Common;
using System.Threading;
using System.Transactions;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
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
			using (Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				Assert.AreEqual(0, interceptor.afterTransactionBeginCalled);
				Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
				Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
			}
		}

		[Test]
		public void AfterBegin()
		{
			var interceptor = new RecordingInterceptor();
			using (new TransactionScope()) 
			using (Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				Assert.AreEqual(1, interceptor.afterTransactionBeginCalled);
				Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
				Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
			}
		}

		[Test]
		public void Complete()
		{
			var interceptor = new RecordingInterceptor();
			ISession session;
			using(var scope = new TransactionScope())
			{
				session = Sfi.WithOptions().Interceptor(interceptor).OpenSession();
				scope.Complete();
			}
			session.Dispose();
			Assert.AreEqual(1, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(1, interceptor.afterTransactionCompletionCalled);
			
		}

		[Test]
		public void Rollback()
		{
			var interceptor = new RecordingInterceptor();
			using (new TransactionScope())
			using (Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
			}
			Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(1, interceptor.afterTransactionCompletionCalled);
		}

		[Test]
		public void TwoTransactionScopesInsideOneSession()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				using (var scope = new TransactionScope())
				{
					session.CreateCriteria<object>().List();
					scope.Complete();
				}

				using (var scope = new TransactionScope())
				{
					session.CreateCriteria<object>().List();
					scope.Complete();
				}
			}
			Assert.AreEqual(2, interceptor.afterTransactionBeginCalled);
			Assert.AreEqual(2, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(2, interceptor.afterTransactionCompletionCalled);
		}

		[Test]
		public void OneTransactionScopesInsideOneSession()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			{
				using (var scope = new TransactionScope())
				{
					session.CreateCriteria<object>().List();
					scope.Complete();
				}
			}
			Assert.AreEqual(1, interceptor.afterTransactionBeginCalled);
			Assert.AreEqual(1, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(1, interceptor.afterTransactionCompletionCalled);
		}


		[Description("NH2128, NH3572")]
		[Theory]
		public void ShouldNotifyAfterDistributedTransaction(bool doCommit)
		{
			// Note: For distributed transaction, calling Close() on the session isn't
			// supported, so we don't need to test that scenario.

			var interceptor = new RecordingInterceptor();
			ISession s1 = null;
			ISession s2 = null;

			using (var tx = new TransactionScope())
			{
				try
				{
					s1 = OpenSession(interceptor);
					s2 = OpenSession(interceptor);

					s1.CreateCriteria<object>().List();
					s2.CreateCriteria<object>().List();
				}
				finally
				{
					if (s1 != null)
						s1.Dispose();
					if (s2 != null)
						s2.Dispose();
				}

				if (doCommit)
					tx.Complete();
			}

			Assert.That(s1.IsOpen, Is.False);
			Assert.That(s2.IsOpen, Is.False);
			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(2));
		}


		[Description("NH2128")]
		[Theory]
		public void ShouldNotifyAfterDistributedTransactionWithOwnConnection(bool doCommit)
		{
			// Note: For distributed transaction, calling Close() on the session isn't
			// supported, so we don't need to test that scenario.

			var interceptor = new RecordingInterceptor();
			ISession s1 = null;

			using (var tx = new TransactionScope())
			{
				var ownConnection1 = Sfi.ConnectionProvider.GetConnection();

				try
				{
					using (s1 = Sfi.WithOptions().Connection(ownConnection1).Interceptor(interceptor).OpenSession())
					{
						s1.CreateCriteria<object>().List();
					}

					if (doCommit)
						tx.Complete();
				}
				finally
				{
					Sfi.ConnectionProvider.CloseConnection(ownConnection1);
				}
			}

			// Transaction completion may happen asynchronously, so allow some delay.
			Assert.That(() => s1.IsOpen, Is.False.After(500, 100));

			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
		}

	}
}