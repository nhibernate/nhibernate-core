using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test
{
	[TestFixture]
	public class LoaderPrepareQueryCommandAsyncTest
	{
		private const int BindingThrowsHibernateException = 1;
		private const int BindingThrowsInvalidOperation = 2;

		[TestCase(BindingThrowsHibernateException)]
		[TestCase(BindingThrowsInvalidOperation)]
		public void PrepareQueryCommandClosesCommandOnBindingFailure(int failureKind)
		{
			var (loader, session, batcher, sqlCommand, command) = CreateQueryCommandContext();
			var failure = CreateBindingFailure(failureKind);
			sqlCommand.When(c => c.Bind(command, session)).Do(_ => throw failure);

			Assert.That(() => loader.Prepare(new QueryParameters(), session), Throws.Exception.SameAs(failure));

			batcher.Received(1).CloseCommand(command, null);
		}

		[TestCase(BindingThrowsHibernateException)]
		[TestCase(BindingThrowsInvalidOperation)]
		public void PrepareQueryCommandAsyncClosesCommandOnBindingFailure(int failureKind)
		{
			var (loader, session, batcher, sqlCommand, command) = CreateQueryCommandContext();
			var failure = CreateBindingFailure(failureKind);
			sqlCommand
				.BindAsync(command, session, Arg.Any<CancellationToken>())
				.Returns(Task.FromException(failure));

			Assert.That(
				() => loader.PrepareAsync(new QueryParameters(), session, CancellationToken.None),
				Throws.Exception.SameAs(failure));

			batcher.Received(1).CloseCommand(command, null);
		}

		[Test]
		public void PrepareQueryCommandAsyncClosesCommandOnBindingCancellation()
		{
			var (loader, session, batcher, sqlCommand, command) = CreateQueryCommandContext();
			using var cancellationTokenSource = new CancellationTokenSource();
			var failure = new OperationCanceledException(cancellationTokenSource.Token);

			sqlCommand.BindAsync(command, session, cancellationTokenSource.Token).Returns(async _ =>
			{
#if NET8_0_OR_GREATER
				await cancellationTokenSource.CancelAsync();
#else
				cancellationTokenSource.Cancel();
				await Task.CompletedTask;
#endif

				throw failure;
			});

			Assert.That(
				() => loader.PrepareAsync(new QueryParameters(), session, cancellationTokenSource.Token),
				Throws.Exception.SameAs(failure));

			batcher.Received(1).CloseCommand(command, null);
		}

		[Test]
		public void PrepareQueryCommandDoesNotCloseSuccessfulCommand()
		{
			var (loader, session, batcher, _, command) = CreateQueryCommandContext();

			var result = loader.Prepare(new QueryParameters(), session);

			Assert.That(result, Is.SameAs(command));
			batcher.DidNotReceive().CloseCommand(command, Arg.Any<DbDataReader>());
		}

		[Test]
		public async Task PrepareQueryCommandAsyncDoesNotCloseSuccessfulCommand()
		{
			var (loader, session, batcher, _, command) = CreateQueryCommandContext();

			var result = await loader.PrepareAsync(new QueryParameters(), session, CancellationToken.None);

			Assert.That(result, Is.SameAs(command));
			batcher.DidNotReceive().CloseCommand(command, Arg.Any<DbDataReader>());
		}

		[Test]
		public void GetResultSetClosesCommandAndReaderOnReadFailure()
		{
			var (loader, session, batcher, command, reader) = CreateResultSetContext();
			var failure = new InvalidOperationException("Reading failed");
			reader.When(r => r.Read()).Do(_ => throw failure);

			var parameters = new QueryParameters { RowSelection = new RowSelection { FirstRow = 1 } };

			Assert.That(() => loader.Read(command, parameters, session), Throws.Exception.SameAs(failure));

			batcher.Received(1).CloseCommand(command, reader);
		}

		[Test]
		public void GetResultSetAsyncClosesCommandAndReaderOnCancellation()
		{
			var (loader, session, batcher, command, reader) = CreateResultSetContext();
			using var cancellationTokenSource = new CancellationTokenSource();
			var failure = new OperationCanceledException(cancellationTokenSource.Token);
			reader.ReadAsync(cancellationTokenSource.Token).Returns(_ => ReadAndCancelAsync());

			var parameters = new QueryParameters { RowSelection = new RowSelection { FirstRow = 1 } };

			Assert.That(
				() => loader.ReadAsync(command, parameters, session, cancellationTokenSource.Token),
				Throws.Exception.SameAs(failure));

			batcher.Received(1).CloseCommand(command, reader);

			async Task<bool> ReadAndCancelAsync()
			{
#if NET8_0_OR_GREATER
				await cancellationTokenSource.CancelAsync();
#else
				cancellationTokenSource.Cancel();
				await Task.CompletedTask;
#endif

				throw failure;
			}
		}

		[Test]
		public void GetResultSetDoesNotCloseSuccessfulReader()
		{
			var (loader, session, batcher, command, reader) = CreateResultSetContext();

			var result = loader.Read(command, new QueryParameters(), session);

			Assert.That(result, Is.SameAs(reader));
			batcher.DidNotReceive().CloseCommand(command, Arg.Any<DbDataReader>());
		}

		[Test]
		public async Task GetResultSetAsyncDoesNotCloseSuccessfulReader()
		{
			var (loader, session, batcher, command, reader) = CreateResultSetContext();

			var result = await loader.ReadAsync(command, new QueryParameters(), session, CancellationToken.None);

			Assert.That(result, Is.SameAs(reader));
			batcher.DidNotReceive().CloseCommand(command, Arg.Any<DbDataReader>());
		}

		private static Exception CreateBindingFailure(int failureKind)
		{
			return failureKind switch
			{
				BindingThrowsHibernateException => new HibernateException("Binding failed"),
				BindingThrowsInvalidOperation => new InvalidOperationException("Binding failed"),
				_ => throw new ArgumentOutOfRangeException(nameof(failureKind))
			};
		}

		private static (TestLoader Loader, ISessionImplementor Session, IBatcher Batcher, ISqlCommand SqlCommand, DbCommand Command)
			CreateQueryCommandContext()
		{
			var provider = Substitute.For<IConnectionProvider>();
			provider.Driver.Returns(Substitute.For<IDriver>());
			
			var factory = Substitute.For<ISessionFactoryImplementor>();
			factory.ConnectionProvider.Returns(provider);

			var session = Substitute.For<ISessionImplementor>();
			var batcher = Substitute.For<IBatcher>();
			session.Batcher.Returns(batcher);

			var command = Substitute.For<DbCommand>();
			batcher.PrepareQueryCommand(CommandType.Text, Arg.Any<SqlString>(), Arg.Any<SqlType[]>()).Returns(command);

			var sqlCommand = Substitute.For<ISqlCommand>();
			sqlCommand.Query.Returns(new SqlString("select 1"));
			sqlCommand.ParameterTypes.Returns([]);

			var loader = new TestLoader(factory, sqlCommand);

			return (loader, session, batcher, sqlCommand, command);
		}

		private static (TestLoader Loader, ISessionImplementor Session, IBatcher Batcher, DbCommand Command, DbDataReader Reader)
			CreateResultSetContext()
		{
			var factory = Substitute.For<ISessionFactoryImplementor>();
			factory.Settings.Returns(new Settings());
			factory.Dialect.Returns(new MsSql2000Dialect());

			var session = Substitute.For<ISessionImplementor>();
			session.Factory.Returns(factory);
			var batcher = Substitute.For<IBatcher>();
			session.Batcher.Returns(batcher);

			var command = Substitute.For<DbCommand>();
			var reader = Substitute.For<DbDataReader>();
			batcher.ExecuteReader(command).Returns(reader);
			batcher.ExecuteReaderAsync(command, Arg.Any<CancellationToken>()).Returns(Task.FromResult(reader));

			var loader = new TestLoader(factory, null);

			return (loader, session, batcher, command, reader);
		}

		private sealed class TestLoader : NHibernate.Loader.Loader
		{
			private readonly ISqlCommand _sqlCommand;

			public TestLoader(ISessionFactoryImplementor factory, ISqlCommand sqlCommand)
				: base(factory)
			{
				_sqlCommand = sqlCommand;
			}

			public Task<DbCommand> PrepareAsync(
				QueryParameters queryParameters,
				ISessionImplementor session,
				CancellationToken cancellationToken)
			{
				return PrepareQueryCommandAsync(queryParameters, scroll: false, session, cancellationToken);
			}

			public DbCommand Prepare(QueryParameters queryParameters, ISessionImplementor session)
				=> PrepareQueryCommand(queryParameters, false, session);

			public DbDataReader Read(DbCommand command, QueryParameters parameters, ISessionImplementor session)
				=> GetResultSet(command, parameters, session, null);

			public Task<DbDataReader> ReadAsync(
				DbCommand command,
				QueryParameters parameters,
				ISessionImplementor session,
				CancellationToken cancellationToken)
				=> GetResultSetAsync(command, parameters, session, null, cancellationToken);

			public override SqlString SqlString => new("select 1");
			public override ILoadable[] EntityPersisters => [];
			protected override IEntityAliases[] EntityAliases => [];
			protected override ICollectionAliases[] CollectionAliases => [];

			public override LockMode[] GetLockModes(IDictionary<string, LockMode> lockModes) => [];

			protected override IEnumerable<IParameterSpecification> GetParameterSpecifications() => [];

			public override ISqlCommand CreateSqlCommand(
				QueryParameters queryParameters,
				ISessionImplementor session)
			{
				return _sqlCommand;
			}
		}
	}
}
