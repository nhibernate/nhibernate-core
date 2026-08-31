using System;
using NUnit.Framework;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Driver;

namespace NHibernate.Test.Ado;

[TestFixture]
public class CancellationTokenFixture : TestCase
{
	protected override string MappingsAssembly => "NHibernate.Test";

	protected override string[] Mappings => ["Ado.VerySimple.hbm.xml"];

	protected override void Configure(Configuration configuration)
	{
		base.Configure(configuration);
		configuration.SetProperty("connection.driver_class", typeof(TestDriver).AssemblyQualifiedName);
	}

	[SetUp]
	public void Setup()
	{
		TestDriver.CancellationTokenSource = null;
		TestDriver.CommandWasDisposed = false;
		TestDriver.Reader = null;
		TestDriver.SupportsMultipleOpenReaders = false;
	}


	[Test]
	public async Task CancellationDuringAsyncQueryDisposesResources()
	{
		using (var session = Sfi.OpenSession())
		{
			await session.SaveAsync(new VerySimple { Id = 1, Name = "Fabio", Weight = 119.5 });
		}

		TestDriver.CancellationTokenSource = new CancellationTokenSource();

		using (var session = Sfi.OpenSession())
		{
			using (session.BeginTransaction())
			{
				Assert.ThrowsAsync<OperationCanceledException>(() => session.QueryOver<VerySimple>()
				                                                            .Skip(1)
				                                                            .ListAsync(TestDriver.CancellationTokenSource.Token));
			}
		}
		
		Assert.That(TestDriver.Reader.IsClosed, Is.True);
		Assert.That(TestDriver.CommandWasDisposed, Is.True);
	}

	[Test]
	public void CancellationDuringAsyncReaderExecutionDisposesCommand()
	{
		TestDriver.SupportsMultipleOpenReaders = true;
		TestDriver.CancellationTokenSource = new CancellationTokenSource();
		
		using (var session = Sfi.OpenSession())
		using (session.BeginTransaction())
		{
			Assert.ThrowsAsync<OperationCanceledException>(
				() => session.QueryOver<VerySimple>()
				             // -1 to fail UseLimit in Loader.GetResultSetAsync and force AdvanceAsync
				             .Skip(-1)
				             .ListAsync(TestDriver.CancellationTokenSource.Token));
		}

		Assert.That(TestDriver.Reader.IsClosed, Is.True);
		Assert.That(TestDriver.CommandWasDisposed, Is.True);
	}

	private sealed class TestDriver : MicrosoftDataSqlClientDriver, IDriver
	{
		public static CancellationTokenSource CancellationTokenSource { get; set; }
		public static bool CommandWasDisposed { get; set; }
		public static DbDataReader Reader { get; set; }
		public new static bool SupportsMultipleOpenReaders { get; set; }

		bool IDriver.SupportsMultipleOpenReaders => SupportsMultipleOpenReaders;

		public override DbCommand CreateCommand()
		{
			return new TestDbCommand(base.CreateCommand());
		}
	}

	private sealed class TestDbCommand : DbCommand
	{
		private readonly DbCommand _underlyingCommand;

		public TestDbCommand(DbCommand underlyingCommand)
		{
			_underlyingCommand = underlyingCommand;
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			TestDriver.Reader = _underlyingCommand.ExecuteReader(behavior);
			return TestDriver.Reader;
		}

		protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
			CommandBehavior behavior,
			CancellationToken cancellationToken)
		{
			var reader = await _underlyingCommand.ExecuteReaderAsync(
				behavior,
				cancellationToken);
			TestDriver.Reader = reader;

			if (TestDriver.CancellationTokenSource != null)
			{
#if NET8_0_OR_GREATER
				await TestDriver.CancellationTokenSource.CancelAsync();
#else
				TestDriver.CancellationTokenSource.Cancel();
#endif
			}

			return TestDriver.Reader;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				TestDriver.CommandWasDisposed = true;
				_underlyingCommand.Dispose();
			}

			base.Dispose(disposing);
		}

		// the rest simply redirect to the underlying command

		public override string CommandText
		{
			get => _underlyingCommand.CommandText;
			set => _underlyingCommand.CommandText = value;
		}

		public override int CommandTimeout
		{
			get => _underlyingCommand.CommandTimeout;
			set => _underlyingCommand.CommandTimeout = value;
		}

		public override CommandType CommandType
		{
			get => _underlyingCommand.CommandType;
			set => _underlyingCommand.CommandType = value;
		}

		public override bool DesignTimeVisible
		{
			get => _underlyingCommand.DesignTimeVisible;
			set => _underlyingCommand.DesignTimeVisible = value;
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get => _underlyingCommand.UpdatedRowSource;
			set => _underlyingCommand.UpdatedRowSource = value;
		}

		protected override DbConnection DbConnection
		{
			get => _underlyingCommand.Connection;
			set => _underlyingCommand.Connection = value;
		}

		protected override DbParameterCollection DbParameterCollection => _underlyingCommand.Parameters;

		protected override DbTransaction DbTransaction
		{
			get => _underlyingCommand.Transaction;
			set => _underlyingCommand.Transaction = value;
		}

		public override void Cancel() => _underlyingCommand.Cancel();

		public override int ExecuteNonQuery() => _underlyingCommand.ExecuteNonQuery();

		public override object ExecuteScalar() => _underlyingCommand.ExecuteScalar();

		public override void Prepare() => _underlyingCommand.Prepare();

		protected override DbParameter CreateDbParameter() => _underlyingCommand.CreateParameter();

		public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) =>
			_underlyingCommand.ExecuteNonQueryAsync(cancellationToken);

		public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) =>
			_underlyingCommand.ExecuteScalarAsync(cancellationToken);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
		public override Task PrepareAsync(CancellationToken cancellationToken =
 default) => _underlyingCommand.PrepareAsync(cancellationToken);

		public override ValueTask DisposeAsync() => _underlyingCommand.DisposeAsync();
#endif
	}
}
