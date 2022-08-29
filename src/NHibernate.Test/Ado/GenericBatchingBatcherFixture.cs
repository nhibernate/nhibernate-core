using System;
using System.Diagnostics;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.SqlCommand;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Ado
{
	[TestFixture]
	public class GenericBatchingBatcherFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "Ado.VerySimple.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchStrategy, typeof(GenericBatchingBatcherFactory).AssemblyQualifiedName);
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.BatchSize, "1000");
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return !(dialect is FirebirdDialect) &&
				   !(dialect is Oracle8iDialect) &&
				   !(dialect is MsSqlCeDialect) &&
				   !(dialect is HanaDialectBase) &&
				   // A workaround exists for SQL Anywhere, see https://stackoverflow.com/a/32860293/1178314
				   // It would imply some tweaking in the generic batcher. The same workaround could
				   // be used for enabling future support.
				   !(dialect is SybaseSQLAnywhere10Dialect);
		}

		[Test]
		public void MassiveInsertUpdateDeleteTest()
		{
			var totalRecords = 1000;
			BatchInsert(totalRecords);
			BatchUpdate(totalRecords);
			BatchDelete(totalRecords);

			DbShoudBeEmpty();
		}

		[Test]
		public void BatchSizeTest()
		{
			using (var sqlLog = new SqlLogSpy())
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.SetBatchSize(5);
				for (var i = 0; i < 20; i++)
				{
					s.Save(new VerySimple { Id = 1 + i, Name = $"Fabio{i}", Weight = 1.45 + i });
				}
				tx.Commit();

				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "Batch commands:"), Is.EqualTo(4));
			}
			Cleanup();
		}

		// Demonstrates a 50% performance gain with SQL-Server, around 40% for PostgreSQL,
		// around 15% for MySql, but around 200% performance loss for SQLite.
		// (Tested with databases on same machine for all cases.)
		[Theory, Explicit("This is a performance test, to be checked manually.")]
		public void MassivePerformanceTest(bool batched)
		{
			if (batched)
			{
				// Bring down batch size to a reasonnable value, otherwise performances are worsen.
				cfg.SetProperty(Environment.BatchSize, "50");
			}
			else
			{
				cfg.SetProperty(Environment.BatchStrategy, typeof(NonBatchingBatcherFactory).AssemblyQualifiedName);
				cfg.Properties.Remove(Environment.BatchSize);
			}
			RebuildSessionFactory();

			try
			{
				// Warm up
				MassiveInsertUpdateDeleteTest();

				var chrono = new Stopwatch();
				chrono.Start();
				MassiveInsertUpdateDeleteTest();
				Console.WriteLine($"Elapsed time: {chrono.Elapsed}");
			}
			finally
			{
				Configure(cfg);
				RebuildSessionFactory();
			}
		}

		[Test]
		public void InterceptorOnPrepareStatementTest()
		{
			var interceptor = new DatabaseInterceptor();
			using (var sqlLog = new SqlLogSpy())
			using (var s = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.SetBatchSize(5);
				for (var i = 0; i < 20; i++)
				{
					s.Save(new VerySimple { Id = 1 + i, Name = $"Fabio{i}", Weight = 1.45 + i });
				}

				tx.Commit();

				Assert.That(interceptor.TotalCalls, Is.EqualTo(1));
				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "/* TEST */"), Is.EqualTo(20));
			}

			interceptor = new DatabaseInterceptor();
			using (var sqlLog = new SqlLogSpy())
			using (var s = Sfi.WithOptions().Interceptor(interceptor).OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var future = s.Query<VerySimple>().ToFuture();
				s.Query<VerySimple>().Where(o => o.Weight > 0).ToFuture();

				using (var enumerator = future.GetEnumerable().GetEnumerator())
				{
					while (enumerator.MoveNext()) { }
				}

				tx.Commit();

				var totalCalls = Sfi.ConnectionProvider.Driver.SupportsMultipleQueries ? 1 : 2;
				Assert.That(interceptor.TotalCalls, Is.EqualTo(totalCalls));
				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "/* TEST */"), Is.EqualTo(totalCalls));
			}

			Cleanup();
		}

		private class DatabaseInterceptor : EmptyInterceptor
		{
			public int TotalCalls { get; private set; }

			public override SqlString OnPrepareStatement(SqlString sql)
			{
				TotalCalls++;
				return sql.Append("/* TEST */");
			}
		}

		private void BatchInsert(int totalRecords)
		{
			Sfi.Statistics.Clear();
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				for (var i = 0; i < totalRecords; i++)
				{
					s.Save(new VerySimple { Id = 1 + i, Name = $"Fabio{i}", Weight = 1.45 + i });
				}
				tx.Commit();
			}
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
		}

		public void BatchUpdate(int totalRecords)
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<VerySimple>().ToList();
				Assert.That(items.Count, Is.EqualTo(totalRecords));

				foreach (var item in items)
				{
					item.Weight += 5;
					s.Update(item);
				}

				Sfi.Statistics.Clear();
				tx.Commit();
			}
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
		}

		public void BatchDelete(int totalRecords)
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<VerySimple>().ToList();
				Assert.That(items.Count, Is.EqualTo(totalRecords));

				foreach (var item in items)
				{
					s.Delete(item);
				}

				Sfi.Statistics.Clear();
				tx.Commit();
			}
			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
		}

		private void DbShoudBeEmpty()
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<VerySimple>().ToList();
				Assert.That(items.Count, Is.EqualTo(0));

				tx.Commit();
			}
		}

		private void Cleanup()
		{
			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from VerySimple").ExecuteUpdate();
				t.Commit();
			}
		}

		private int FindAllOccurrences(string source, string substring)
		{
			if (source == null)
			{
				return 0;
			}
			int n = 0, count = 0;
			while ((n = source.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1)
			{
				n += substring.Length;
				++count;
			}
			return count;
		}
	}
}
