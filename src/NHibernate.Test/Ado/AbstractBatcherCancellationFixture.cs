using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Impl;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.Ado
{
	[TestFixture]
	public class AbstractBatcherCancellationFixture
	{
		private static IBatcher CreateBatcher(bool supportsMultipleOpenReaders)
		{
			var factory = Substitute.For<ISessionFactoryImplementor>();
			var provider = Substitute.For<IConnectionProvider>();
			var driver = Substitute.For<IDriver>();
			driver.SupportsMultipleOpenReaders.Returns(supportsMultipleOpenReaders);

			provider.Driver.Returns(driver);
			factory.ConnectionProvider.Returns(provider);
			factory.Settings.Returns(new Settings
			{
				BatcherFactory = new NonBatchingBatcherFactory(),
				SqlStatementLogger = new SqlStatementLogger()
			});

			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Returns(factory);

			var connectionManager = new ConnectionManager(
				session, Substitute.For<DbConnection>(), ConnectionReleaseMode.OnClose,
				EmptyInterceptor.Instance, false, new NonContextualConnectionAccess(factory));

			return connectionManager.Batcher;
		}

		[Test]
		public void ExecuteReaderAsyncDisposesUnregisteredReaderWhenWrappingIsCancelled()
		{
			using var batcher = CreateBatcher(false);
			var command = Substitute.For<DbCommand>();
			using DbDataReader reader = new System.Data.DataTable().CreateDataReader();
			using var cancellationTokenSource = new CancellationTokenSource();

			command.ExecuteReaderAsync(Arg.Any<CancellationToken>()).Returns(async _ =>
			{
#if NET8_0_OR_GREATER
				await cancellationTokenSource.CancelAsync();
#else
				cancellationTokenSource.Cancel();
				await Task.CompletedTask;
#endif

				return reader;
			});

			Assert.That(
				() => batcher.ExecuteReaderAsync(command, cancellationTokenSource.Token),
				Throws.InstanceOf<OperationCanceledException>());

			Assert.That(reader.IsClosed, Is.True);
			Assert.That(batcher.HasOpenResources, Is.False);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void ExecuteReaderReturnsOpenReaderOnSuccess(bool supportsMultipleOpenReaders)
		{
			using var batcher = CreateBatcher(supportsMultipleOpenReaders);
			using DbDataReader reader = new System.Data.DataTable().CreateDataReader();
			var command = CreateCommand(reader);

			var result = batcher.ExecuteReader(command);

			AssertReaderOwnership(batcher, reader, result);
		}

		[TestCase(false)]
		[TestCase(true)]
		public async Task ExecuteReaderAsyncReturnsOpenReaderOnSuccess(bool supportsMultipleOpenReaders)
		{
			using var batcher = CreateBatcher(supportsMultipleOpenReaders);
			using DbDataReader reader = new System.Data.DataTable().CreateDataReader();
			var command = CreateCommand(reader);

			var result = await batcher.ExecuteReaderAsync(command, CancellationToken.None);

			AssertReaderOwnership(batcher, reader, result);
		}

		private static void AssertReaderOwnership(IBatcher batcher, DbDataReader reader, DbDataReader result)
		{
			Assert.That(reader.IsClosed, Is.False);
			Assert.That(result.IsClosed, Is.False);
			Assert.That(batcher.HasOpenResources, Is.True);

			batcher.CloseReader(result);

			Assert.That(reader.IsClosed, Is.True);
			Assert.That(batcher.HasOpenResources, Is.False);
		}

		private static DbCommand CreateCommand(DbDataReader reader)
		{
			var command = Substitute.For<DbCommand>();
			command.ExecuteReader().Returns(reader);
			command.ExecuteReaderAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(reader));

			return command;
		}

		[Test]
		public void ExecuteReaderRejectsNullReader()
		{
			using var batcher = CreateBatcher(false);
			var command = Substitute.For<DbCommand>();

			Assert.Throws<InvalidOperationException>(() => batcher.ExecuteReader(command));

			Assert.That(batcher.HasOpenResources, Is.False);
		}

		[Test]
		public void ExecuteReaderAsyncRejectsNullReader()
		{
			using var batcher = CreateBatcher(false);
			var command = Substitute.For<DbCommand>();
			command.ExecuteReaderAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<DbDataReader>(null));

			Assert.ThrowsAsync<InvalidOperationException>(() => batcher.ExecuteReaderAsync(command, CancellationToken.None));

			Assert.That(batcher.HasOpenResources, Is.False);
		}
	}
}
