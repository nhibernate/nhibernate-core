using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH1547
{
	[TestFixture, Explicit("Contains only performances benchmark")]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);

			var driverClass = ReflectHelper.ClassForName(configuration.GetProperty(Cfg.Environment.ConnectionDriver));
			DriverForSubstitutedCommand.DriverClass = driverClass;

			configuration.SetProperty(
				Cfg.Environment.ConnectionDriver,
				typeof(DriverForSubstitutedCommand).AssemblyQualifiedName);
		}

		protected override void DropSchema()
		{
			(Sfi.ConnectionProvider.Driver as DriverForSubstitutedCommand)?.CleanUp();
			base.DropSchema();
		}

		[Test]
		public void SimpleLinqPerf()
		{
			Benchmark(
				"Simple LINQ",
				s =>
					s
						.Query<Entity>()
						.Where(e => e.Name == "Bob"));
		}

		[Test]
		public void LinqWithNonParameterizedConstantPerf()
		{
			Benchmark(
				"Non parameterized constant",
				s =>
					s
						.Query<Entity>()
						.Where(e => e.Name == "Bob")
						.Select(e => new { e, c = 2 }));
		}

		[Test]
		public void LinqWithListParameterPerf()
		{
			var names = new[] { "Bob", "Sally" };
			Benchmark(
				"List parameter",
				s =>
					s
						.Query<Entity>()
						.Where(e => names.Contains(e.Name)));
		}

		private void Benchmark<T>(string test, Func<ISession, IQueryable<T>> queryFactory)
		{
			var driver = (DriverForSubstitutedCommand) Sfi.ConnectionProvider.Driver;
			var timings = new List<double>();
			var sw = new Stopwatch();

			var cache = (SoftLimitMRUCache)
				typeof(QueryPlanCache)
					.GetField("planCache", BindingFlags.Instance | BindingFlags.NonPublic)
					.GetValue(Sfi.QueryPlanCache);

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				using (driver.SubstituteCommand())
				{
					var query = queryFactory(session);
					// Warm up.
					RunBenchmarkUnit(cache, query);

					for (var j = 0; j < 1000; j++)
					{
						sw.Restart();
						RunBenchmarkUnit(cache, query);
						sw.Stop();
						timings.Add(sw.Elapsed.TotalMilliseconds);
					}
				}

				tx.Commit();
			}

			var avg = timings.Average();
			Console.WriteLine(
				$"{test} average time: {avg}ms (s {Math.Sqrt(timings.Sum(t => Math.Pow(t - avg, 2)) / (timings.Count - 1))}ms)");
		}

		private static void RunBenchmarkUnit<T>(SoftLimitMRUCache cache, IQueryable<T> query)
		{
			// Do enough iterations for having a significant elapsed time in milliseconds.
			for (var i = 0; i < 20; i++)
			{
				// Always clear the query plan cache before running the query, otherwise the impact of 1547
				// change would be hidden by it. Simulates having many different queries run.
				cache.Clear();
				Assert.That(query.ToList, Throws.Nothing);
			}
		}
	}

	public partial class DriverForSubstitutedCommand : IDriver
	{
		internal static System.Type DriverClass { get; set; }

		private readonly IDriver _driverImplementation;
		private bool _commandSubstituted;

		public DriverForSubstitutedCommand()
		{
			_driverImplementation = (IDriver) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(DriverClass);
		}

		DbCommand IDriver.GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes)
		{
			var cmd = _driverImplementation.GenerateCommand(type, sqlString, parameterTypes);
			if (!_commandSubstituted)
				return cmd;
			return new SubstituteDbCommand(cmd);
		}

		public IDisposable SubstituteCommand()
		{
			_commandSubstituted = true;
			return new EndSubstitute(this);
		}

		#region Firebird mess

		public void CleanUp()
		{
			// Firebird will pool each connection created during the test and will marked as used any table
			// referenced by queries. It will at best delays those tables drop until connections are actually
			// closed, or immediately fail dropping them.
			// This results in other tests failing when they try to create tables with same name.
			// By clearing the connection pool the tables will get dropped. This is done by the following code.
			// Moved from NH1908 test case, contributed by Amro El-Fakharany.
			_driverImplementation.ClearPoolForFirebirdClientDriver();
		}

		#endregion

		#region Pure forwarding

		DbParameter IDriver.GenerateParameter(DbCommand command, string name, SqlType sqlType)
		{
			return _driverImplementation.GenerateParameter(command, name, sqlType);
		}

		void IDriver.Configure(IDictionary<string, string> settings)
		{
			_driverImplementation.Configure(settings);
		}

		DbConnection IDriver.CreateConnection()
		{
			return _driverImplementation.CreateConnection();
		}

		bool IDriver.SupportsMultipleOpenReaders => _driverImplementation.SupportsMultipleOpenReaders;

		void IDriver.PrepareCommand(DbCommand command)
		{
			_driverImplementation.PrepareCommand(command);
		}

		void IDriver.RemoveUnusedCommandParameters(DbCommand cmd, SqlString sqlString)
		{
			_driverImplementation.RemoveUnusedCommandParameters(cmd, sqlString);
		}

		void IDriver.ExpandQueryParameters(DbCommand cmd, SqlString sqlString, SqlType[] parameterTypes)
		{
			_driverImplementation.ExpandQueryParameters(cmd, sqlString, parameterTypes);
		}

		IResultSetsCommand IDriver.GetResultSetsCommand(ISessionImplementor session)
		{
			return _driverImplementation.GetResultSetsCommand(session);
		}

		bool IDriver.SupportsMultipleQueries => _driverImplementation.SupportsMultipleQueries;

		void IDriver.AdjustCommand(DbCommand command)
		{
			_driverImplementation.AdjustCommand(command);
		}

		bool IDriver.RequiresTimeSpanForTime => _driverImplementation.RequiresTimeSpanForTime;

		bool IDriver.SupportsSystemTransactions => _driverImplementation.SupportsSystemTransactions;

		bool IDriver.SupportsNullEnlistment => _driverImplementation.SupportsNullEnlistment;

		bool IDriver.SupportsEnlistmentWhenAutoEnlistmentIsDisabled =>
			_driverImplementation.SupportsEnlistmentWhenAutoEnlistmentIsDisabled;

		bool IDriver.HasDelayedDistributedTransactionCompletion =>
			_driverImplementation.HasDelayedDistributedTransactionCompletion;

		DateTime IDriver.MinDate => _driverImplementation.MinDate;

		#endregion

		private class EndSubstitute : IDisposable
		{
			private readonly DriverForSubstitutedCommand _driver;

			public EndSubstitute(DriverForSubstitutedCommand driver)
			{
				_driver = driver;
			}

			public void Dispose()
			{
				_driver._commandSubstituted = false;
			}
		}

		private partial class SubstituteDbCommand : DbCommand
		{
			private static readonly DbDataReader _substituteReader = Substitute.For<DbDataReader>();
			private readonly DbCommand _concreteCommand;

			public SubstituteDbCommand(DbCommand concreteCommand)
			{
				_concreteCommand = concreteCommand;
			}

			protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
			{
				return _substituteReader;
			}

			public override void Prepare()
			{
			}

			public override int ExecuteNonQuery()
			{
				return 0;
			}

			public override object ExecuteScalar()
			{
				return null;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing)
				{
					_concreteCommand.Dispose();
				}
			}

			#region Pure forwarding

			public override string CommandText
			{
				get => _concreteCommand.CommandText;
				set => _concreteCommand.CommandText = value;
			}

			public override int CommandTimeout
			{
				get => _concreteCommand.CommandTimeout;
				set => _concreteCommand.CommandTimeout = value;
			}

			public override CommandType CommandType
			{
				get => _concreteCommand.CommandType;
				set => _concreteCommand.CommandType = value;
			}

			public override UpdateRowSource UpdatedRowSource
			{
				get => _concreteCommand.UpdatedRowSource;
				set => _concreteCommand.UpdatedRowSource = value;
			}

			protected override DbConnection DbConnection
			{
				get => _concreteCommand.Connection;
				set => _concreteCommand.Connection = value;
			}

			protected override DbParameterCollection DbParameterCollection => _concreteCommand.Parameters;

			protected override DbTransaction DbTransaction
			{
				get => _concreteCommand.Transaction;
				set => _concreteCommand.Transaction = value;
			}

			public override bool DesignTimeVisible
			{
				get => _concreteCommand.DesignTimeVisible;
				set => _concreteCommand.DesignTimeVisible = value;
			}

			public override void Cancel()
			{
				_concreteCommand.Cancel();
			}

			protected override DbParameter CreateDbParameter()
			{
				return _concreteCommand.CreateParameter();
			}

			#endregion
		}
	}
}
