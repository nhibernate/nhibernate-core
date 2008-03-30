using System;
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
			public bool afterTransactionBeginCalled;
			public bool afterTransactionCompletionCalled;
			public bool beforeTransactionCompletionCalled;

			public override void AfterTransactionBegin(ITransaction tx)
			{
				afterTransactionBeginCalled = true;
			}

			public override void AfterTransactionCompletion(ITransaction tx)
			{
				afterTransactionCompletionCalled = true;
			}

			public override void BeforeTransactionCompletion(ITransaction tx)
			{
				beforeTransactionCompletionCalled = true;
			}
		}

		[Test]
		public void NoTransaction()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (sessions.OpenSession(interceptor))
			{
				Assert.IsFalse(interceptor.afterTransactionBeginCalled);
				Assert.IsFalse(interceptor.beforeTransactionCompletionCalled);
				Assert.IsFalse(interceptor.afterTransactionCompletionCalled);
			}
		}

		[Test]
		public void AfterBegin()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (new TransactionScope()) 
			using (sessions.OpenSession(interceptor))
			{
				Assert.IsTrue(interceptor.afterTransactionBeginCalled);
				Assert.IsFalse(interceptor.beforeTransactionCompletionCalled);
				Assert.IsFalse(interceptor.afterTransactionCompletionCalled);
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
			Assert.IsTrue(interceptor.beforeTransactionCompletionCalled);
			Assert.IsTrue(interceptor.afterTransactionCompletionCalled);
			
		}

		[Test]
		public void Rollback()
		{
			RecordingInterceptor interceptor = new RecordingInterceptor();
			using (new TransactionScope())
			using (sessions.OpenSession(interceptor))
			{
			}
			Assert.IsFalse(interceptor.beforeTransactionCompletionCalled);
			Assert.IsTrue(interceptor.afterTransactionCompletionCalled);
		
		}
	}
}