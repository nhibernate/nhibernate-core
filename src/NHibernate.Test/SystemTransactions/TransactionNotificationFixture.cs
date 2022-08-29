using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.SystemTransactions
{
	public class TransactionNotificationFixture : TestCase
	{
		public class Entity
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }
		}

		protected override string[] Mappings => null;

		protected override void AddMappings(Configuration configuration)
		{
			var modelMapper = new ModelMapper();
			modelMapper.Class<Entity>(
				x =>
				{
					x.Id(e => e.Id);
					x.Property(e => e.Name);
					x.Table(nameof(Entity));
				});

			configuration.AddMapping(modelMapper.CompileMappingForAllExplicitlyAddedEntities());
		}

		protected virtual bool UseConnectionOnSystemTransactionPrepare => true;

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(
				Cfg.Environment.UseConnectionOnSystemTransactionPrepare,
				UseConnectionOnSystemTransactionPrepare.ToString());
		}

		[Test]
		public void NoTransaction()
		{
			var interceptor = new RecordingInterceptor();
			using (Sfi.WithOptions().Interceptor(interceptor).OpenSession()) { }
			Assert.AreEqual(0, interceptor.afterTransactionBeginCalled);
			Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
		}

		[Test]
		public void TransactionDisabled()
		{
			var interceptor = new RecordingInterceptor();
			using (var ts = new TransactionScope())
			using (Sfi.WithOptions().Interceptor(interceptor).AutoJoinTransaction(false).OpenSession())
			{
				ts.Complete();
			}
			Assert.AreEqual(0, interceptor.afterTransactionBeginCalled);
			Assert.AreEqual(0, interceptor.beforeTransactionCompletionCalled);
			Assert.AreEqual(0, interceptor.afterTransactionCompletionCalled);
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
			using (var scope = new TransactionScope())
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
			IgnoreIfTransactionScopeInsideSessionIsNotSupported();

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
			IgnoreIfTransactionScopeInsideSessionIsNotSupported();

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

			using (var tx = new TransactionScope(TransactionScopeOption.Suppress))
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
			if (!Sfi.ConnectionProvider.Driver.SupportsSystemTransactions)
				Assert.Ignore("Driver does not support System.Transactions. Ignoring test.");

			// Note: For system transaction, calling Close() on the session isn't
			// supported, so we don't need to test that scenario.

			var interceptor = new RecordingInterceptor();
			ISession s1;

			var ownConnection1 = Sfi.ConnectionProvider.GetConnection();
			try
			{
				using (var tx = new TransactionScope())
				{
					using (s1 = Sfi.WithOptions().Connection(ownConnection1).Interceptor(interceptor).OpenSession())
					{
						s1.CreateCriteria<object>().List();
					}

					if (doCommit)
						tx.Complete();
				}
			}
			finally
			{
				Sfi.ConnectionProvider.CloseConnection(ownConnection1);
			}

			// Transaction completion may happen asynchronously, so allow some delay. Odbc promotes
			// this test to distributed and have that delay, by example.
			Assert.That(() => s1.IsOpen, Is.False.After(500, 100), "Session not closed.");

			Assert.That(interceptor.afterTransactionCompletionCalled, Is.EqualTo(1));
		}

		private void IgnoreIfTransactionScopeInsideSessionIsNotSupported()
		{
			if (!Sfi.ConnectionProvider.Driver.SupportsSystemTransactions || !TestDialect.SupportsDependentTransaction)
				Assert.Ignore("Driver does not support dependent transactions. Ignoring test.");
		}
	}

	[TestFixture]
	public class TransactionWithoutConnectionFromPrepareNotificationFixture : TransactionNotificationFixture
	{
		protected override bool UseConnectionOnSystemTransactionPrepare => false;
	}
}
