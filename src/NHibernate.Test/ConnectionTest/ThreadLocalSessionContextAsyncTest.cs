using System;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Engine;
using NHibernate.Transaction;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class ThreadLocalSessionContextAsyncTest
	{
		private const int NoCloseFailure = 0;
		private const int CloseThrowsInvalidOperation = 1;

		private ISessionFactoryImplementor _factory;
		private ISession _orphan;
		private ISession _replacement;
		private ITransaction _transaction;

		[SetUp]
		public void SetUp()
		{
			_factory = Substitute.For<ISessionFactoryImplementor>();
			_orphan = Substitute.For<ISession>();
			_replacement = Substitute.For<ISession>();
			_transaction = Substitute.For<ITransaction>();

			var sessionImplementation = Substitute.For<ISessionImplementor>();
			sessionImplementation.Factory.Returns(_factory);
			_factory.Settings.Returns(new Settings { BatcherFactory = Substitute.For<IBatcherFactory>() });

			var transactionFactory = Substitute.For<ITransactionFactory>();
			transactionFactory.CreateTransaction(sessionImplementation).Returns(_transaction);
			_factory.TransactionFactory.Returns(transactionFactory);

			var connectionManager = new ConnectionManager(
				sessionImplementation,
				null,
				ConnectionReleaseMode.OnClose,
				EmptyInterceptor.Instance,
				false,
				Substitute.For<IConnectionAccess>());
			connectionManager.BeginTransaction();

			_orphan.SessionFactory.Returns(_factory);
			_replacement.SessionFactory.Returns(_factory);
			_orphan.GetSessionImplementation().Returns(sessionImplementation);
			sessionImplementation.ConnectionManager.Returns(connectionManager);
			_transaction.IsActive.Returns(true);

			ThreadLocalSessionContext.Bind(_orphan);
		}

		[TearDown]
		public void TearDown()
		{
			ThreadLocalSessionContext.Unbind(_factory);
		}

		[TestCase(NoCloseFailure)]
		[TestCase(CloseThrowsInvalidOperation)]
		public void BindAsyncClosesOrphanedSessionAndPreservesRollbackCancellation(int closeFailure)
		{
			using (var cancellationTokenSource = new CancellationTokenSource())
			{
				var failure = new OperationCanceledException(cancellationTokenSource.Token);

				_transaction
					.RollbackAsync(Arg.Any<CancellationToken>())
					.Returns(async _ =>
					{
#if NET8_0_OR_GREATER
						await cancellationTokenSource.CancelAsync();
#else
						cancellationTokenSource.Cancel();
						await Task.CompletedTask;
#endif

						throw failure;
					});

				SetCloseFailure(closeFailure);

				var exception =
					Assert.ThrowsAsync<OperationCanceledException>(() => ThreadLocalSessionContext.BindAsync(
						                                               _replacement,
						                                               cancellationTokenSource.Token));

				Assert.That(exception, Is.SameAs(failure));
				_orphan.Received(1).Close();
				Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.Null);
			}
		}

		[TestCase(NoCloseFailure)]
		[TestCase(CloseThrowsInvalidOperation)]
		public void BindRollsBackOrphanAndBindsReplacement(int closeFailure)
		{
			SetCloseFailure(closeFailure);

			Assert.DoesNotThrow(() => ThreadLocalSessionContext.Bind(_replacement));

			_transaction.Received(1).Rollback();
			_orphan.Received(1).Close();
			Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.SameAs(_replacement));
		}

		[TestCase(NoCloseFailure)]
		[TestCase(CloseThrowsInvalidOperation)]
		public void BindAsyncRollsBackOrphanAndBindsReplacement(int closeFailure)
		{
			SetCloseFailure(closeFailure);

			Assert.DoesNotThrowAsync(() => ThreadLocalSessionContext.BindAsync(
				_replacement,
				CancellationToken.None));

			_transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
			_orphan.Received(1).Close();
			Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.SameAs(_replacement));
		}

		[TestCase(NoCloseFailure)]
		[TestCase(CloseThrowsInvalidOperation)]
		public void BindClosesOrphanAndBindsReplacementWhenRollbackFails(int closeFailure)
		{
			var rollbackFailure = new InvalidOperationException("Rollback failed");
			_transaction.When(t => t.Rollback()).Do(_ => throw rollbackFailure);
			SetCloseFailure(closeFailure);

			Assert.DoesNotThrow(() => ThreadLocalSessionContext.Bind(_replacement));

			_transaction.Received(1).Rollback();
			_orphan.Received(1).Close();
			Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.SameAs(_replacement));
		}

		[TestCase(NoCloseFailure)]
		[TestCase(CloseThrowsInvalidOperation)]
		public void BindAsyncClosesOrphanAndBindsReplacementWhenRollbackFails(int closeFailure)
		{
			var rollbackFailure = new InvalidOperationException("Rollback failed");
			_transaction.RollbackAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException(rollbackFailure));
			SetCloseFailure(closeFailure);

			Assert.DoesNotThrowAsync(() => ThreadLocalSessionContext.BindAsync(
				_replacement,
				CancellationToken.None));

			_transaction.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
			_orphan.Received(1).Close();
			Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.SameAs(_replacement));
		}

		[Test]
		public void BindClosesOrphanWithoutActiveTransaction()
		{
			_transaction.IsActive.Returns(false);

			ThreadLocalSessionContext.Bind(_replacement);

			_transaction.DidNotReceive().Rollback();
			_orphan.Received(1).Close();
			Assert.That(ThreadLocalSessionContext.Unbind(_factory), Is.SameAs(_replacement));
		}

		private void SetCloseFailure(int closeFailure)
		{
			if (closeFailure == NoCloseFailure)
				return;

			if (closeFailure != CloseThrowsInvalidOperation)
				throw new ArgumentOutOfRangeException(nameof(closeFailure));

			_orphan.When(s => s.Close()).Do(_ => throw new InvalidOperationException("Closing failed"));
		}
	}
}
