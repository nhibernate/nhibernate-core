using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.Ado
{
	[TestFixture]
	public class GenericBatchingBatcherFixture : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] {"Ado.VerySimple.hbm.xml"};

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
			       !(dialect is MsSqlCeDialect);
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

		private void BatchInsert(int totalRecords)
		{
			Sfi.Statistics.Clear();
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				for (var i = 0; i < totalRecords; i++)
				{
					s.Save(new VerySimple {Id = 1 + i, Name = $"Fabio{i}", Weight = 1.45 + i});
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
			using (s.BeginTransaction())
			{
				s.CreateQuery("delete from VerySimple").ExecuteUpdate();
				s.Transaction.Commit();
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
