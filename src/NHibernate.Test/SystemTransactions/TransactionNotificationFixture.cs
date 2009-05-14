using System.Collections;
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

		public class RecordingInterceptor : EmptyInterceptor
		{
			public int afterTransactionBeginCalled;
			public int afterTransactionCompletionCalled;
			public int beforeTransactionCompletionCalled;

			public override void AfterTransactionBegin(ITransaction tx)
			{
				afterTransactionBeginCalled++;
			}

			public override void AfterTransactionCompletion(ITransaction tx)
			{
				afterTransactionCompletionCalled++;
			}

			public override void BeforeTransactionCompletion(ITransaction tx)
			{
				beforeTransactionCompletionCalled++;
			}
		}

		[Test]
		public void NoTransaction()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (sessions.OpenSession(interceptor))
			{
				Assert.AreEqual(0, interceptor.afterTransactionBeginCalled);
				Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
				Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
			}
		}

		[Test]
		public void AfterBegin()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (new TransactionScope()) 
			using (sessions.OpenSession(interceptor))
			{
				Assert.AreEqual(1, interceptor.afterTransactionBeginCalled);
				Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
				Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
			}
		}

		[Test]
		public void Complete()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			ISession session;
			using(TransactionScope scope = new TransactionScope())
			{
				session = sessions.OpenSession(interceptor);
				scope.Complete();
			}
			session.Dispose();
			Assert.AreEqual(1, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(1, interceptor.afterTransactionCompletionCalled);
			
		}

		[Test]
		public void Rollback()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (new TransactionScope())
			using (sessions.OpenSession(interceptor))
			{
			}
			Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(2, interceptor.afterTransactionCompletionCalled);
		}

		[Test]
		public void TwoTransactionScopesInsideOneSession()
		{
			var interceptor = new RecordingInterceptor();
			using (var session = sessions.OpenSession(interceptor))
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
			using (var session = sessions.OpenSession(interceptor))
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
	}
}