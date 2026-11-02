using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Connection;
using NHibernate.Driver;
using NSubstitute;
using NUnit.Framework;
using CfgEnvironment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.ConnectionTest
{
	[TestFixture]
	public class DriverConnectionProviderTest
	{
		private const int NoDisposeFailure = 0;
		private const int DisposeThrowsInvalidOperation = 1;

		[TestCase(NoDisposeFailure)]
		[TestCase(DisposeThrowsInvalidOperation)]
		public void GetConnectionAsyncDisposesConnectionAndPreservesCancellation(int disposeFailure)
		{
			using var cancellationTokenSource = new CancellationTokenSource();
			var openFailure = new OperationCanceledException(cancellationTokenSource.Token);
			var connection = CreateConnection(null, failOnConnectionString: false, disposeFailure);

			connection.OpenAsync(Arg.Any<CancellationToken>()).Returns(async _ =>
			{
#if NET8_0_OR_GREATER
				await cancellationTokenSource.CancelAsync();
#else
				cancellationTokenSource.Cancel();
				await Task.CompletedTask;
#endif

				throw openFailure;
			});

			using var provider = CreateProvider(connection);

			var exception =
				Assert.ThrowsAsync<OperationCanceledException>(() => provider.GetConnectionAsync(
					                                               cancellationTokenSource.Token));

			Assert.That(exception, Is.SameAs(openFailure));
			connection.Received(1).Dispose();
		}

		[TestCase(NoDisposeFailure)]
		[TestCase(DisposeThrowsInvalidOperation)]
		public void GetConnectionDisposesConnectionAndPreservesOpenFailure(int disposeFailure)
		{
			var openFailure = new InvalidOperationException("Opening failed");
			var connection = CreateConnection(openFailure, failOnConnectionString: false, disposeFailure);
			using var provider = CreateProvider(connection);

			var exception = Assert.Throws<InvalidOperationException>(() => provider.GetConnection());

			Assert.That(exception, Is.SameAs(openFailure));
			connection.Received(1).Dispose();
		}

		[TestCase(NoDisposeFailure)]
		[TestCase(DisposeThrowsInvalidOperation)]
		public void GetConnectionAsyncDisposesConnectionAndPreservesOpenFailure(int disposeFailure)
		{
			var openFailure = new InvalidOperationException("Opening failed");
			var connection = CreateConnection(openFailure, failOnConnectionString: false, disposeFailure);
			using var provider = CreateProvider(connection);

			var exception = Assert.ThrowsAsync<InvalidOperationException>(
				() => provider.GetConnectionAsync(CancellationToken.None));

			Assert.That(exception, Is.SameAs(openFailure));
			connection.Received(1).Dispose();
		}

		[TestCase(NoDisposeFailure)]
		[TestCase(DisposeThrowsInvalidOperation)]
		public void GetConnectionDisposesConnectionAndPreservesConnectionStringFailure(int disposeFailure)
		{
			var openFailure = new InvalidOperationException("Setting connection string failed");
			var connection = CreateConnection(openFailure, failOnConnectionString: true, disposeFailure);
			using var provider = CreateProvider(connection);

			var exception = Assert.Throws<InvalidOperationException>(() => provider.GetConnection());

			Assert.That(exception, Is.SameAs(openFailure));
			connection.Received(1).Dispose();
		}

		[TestCase(NoDisposeFailure)]
		[TestCase(DisposeThrowsInvalidOperation)]
		public void GetConnectionAsyncDisposesConnectionAndPreservesConnectionStringFailure(int disposeFailure)
		{
			var openFailure = new InvalidOperationException("Setting connection string failed");
			var connection = CreateConnection(openFailure, failOnConnectionString: true, disposeFailure);
			using var provider = CreateProvider(connection);

			var exception = Assert.ThrowsAsync<InvalidOperationException>(
				() => provider.GetConnectionAsync(CancellationToken.None));

			Assert.That(exception, Is.SameAs(openFailure));
			connection.Received(1).Dispose();
		}

		[Test]
		public void GetConnectionDoesNotDisposeSuccessfulConnection()
		{
			var connection = CreateConnection(null, failOnConnectionString: false, NoDisposeFailure);
			using var provider = CreateProvider(connection);

			var result = provider.GetConnection();

			Assert.That(result, Is.SameAs(connection));
			Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
			connection.DidNotReceive().Dispose();

			connection.Dispose();
		}

		[Test]
		public async Task GetConnectionAsyncDoesNotDisposeSuccessfulConnection()
		{
			var connection = CreateConnection(null, failOnConnectionString: false, NoDisposeFailure);
			using var provider = CreateProvider(connection);

			var result = await provider.GetConnectionAsync(CancellationToken.None);

			Assert.That(result, Is.SameAs(connection));
			Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
			connection.DidNotReceive().Dispose();

			connection.Dispose();
		}

		[Test]
		public void GetConnectionAsyncDoesNotCreateConnectionWhenAlreadyCancelled()
		{
			using var provider = CreateProvider(CreateConnection(null, failOnConnectionString: false, NoDisposeFailure));
			using var cancellationTokenSource = new CancellationTokenSource();

			cancellationTokenSource.Cancel();

			Assert.That(
				() => provider.GetConnectionAsync(cancellationTokenSource.Token),
				Throws.InstanceOf<OperationCanceledException>());

			Assert.That(TestDriver.CreateConnectionCount, Is.Zero);
		}

		private static Exception GetDisposeFailure(int failure)
		{
			return failure switch
			{
				NoDisposeFailure => null,
				DisposeThrowsInvalidOperation => new InvalidOperationException("Disposal failed"),
				_ => throw new ArgumentOutOfRangeException(nameof(failure))
			};
		}

		private static DriverConnectionProvider CreateProvider(DbConnection connection)
		{
			TestDriver.Connection = connection;
			TestDriver.CreateConnectionCount = 0;

			var provider = new DriverConnectionProvider();
			provider.Configure(
				new Dictionary<string, string>
				{
					[CfgEnvironment.ConnectionString] = "connection-string",
					[CfgEnvironment.ConnectionDriver] = typeof(TestDriver).AssemblyQualifiedName
				});

			return provider;
		}

		private static DbConnection CreateConnection(Exception openFailure, bool failOnConnectionString, int disposeFailure)
		{
			var connection = Substitute.For<DbConnection>();
			connection.State.Returns(ConnectionState.Open);
			
			if (openFailure != null)
			{
				connection.When(c => c.Open()).Do(_ => throw openFailure);
				connection.OpenAsync(Arg.Any<CancellationToken>()).Returns(Task.FromException(openFailure));

				if (failOnConnectionString)
					connection.When(c => c.ConnectionString = Arg.Any<string>()).Do(_ => throw openFailure);
			}
			else
			{
				connection.OpenAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
			}

			var failure = GetDisposeFailure(disposeFailure);
			if (failure != null)
				connection.When(c => c.Dispose()).Do(_ => throw failure);

			return connection;
		}

		private sealed class TestDriver : DriverBase
		{
			public static DbConnection Connection { get; set; }
			public static int CreateConnectionCount { get; set; }

			public override DbConnection CreateConnection()
			{
				CreateConnectionCount++;
				return Connection;
			}

			public override DbCommand CreateCommand()
			{
				throw new NotSupportedException();
			}

			public override bool UseNamedPrefixInSql => false;
			public override bool UseNamedPrefixInParameter => false;
			public override string NamedPrefix => "@";
		}
	}
}
