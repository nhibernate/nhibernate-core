using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Ado
{
#if NET6_0_OR_GREATER
	[TestFixture(true)]
#endif
	[TestFixture(false)]
	public class BatcherFixture: TestCase
	{
		private readonly bool _useDbBatch;

		public BatcherFixture(bool useDbBatch)
		{
			_useDbBatch = useDbBatch;
		}
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Ado.VerySimple.hbm.xml", "Ado.AlmostSimple.hbm.xml" }; }
		}

		private const int BatchSize = 10;

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.FormatSql, "true");
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.BatchSize, BatchSize.ToString());
			#if NET6_0_OR_GREATER
			if (_useDbBatch)
			{
				configuration.SetProperty(Environment.BatchStrategy, typeof(DbBatchBatcherFactory).AssemblyQualifiedName);
			}
			#endif
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
#if NET6_0_OR_GREATER
			if (_useDbBatch)
			{
				return factory.Settings.BatcherFactory is DbBatchBatcherFactory && factory.Settings.ConnectionProvider.Driver is Driver.DriverBase driverBase && driverBase.CanCreateBatch;
			}
#endif
			return !(factory.Settings.BatcherFactory is NonBatchingBatcherFactory);
		}

		[Test]
		[Description("The batcher should run all INSERT queries in only one roundtrip.")]
		public void OneRoundTripInserts()
		{
			Sfi.Statistics.Clear();
			FillDb();

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		private void Cleanup()
		{
			using (ISession s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from VerySimple").ExecuteUpdate();
				s.CreateQuery("delete from AlmostSimple").ExecuteUpdate();
				t.Commit();
			}
		}

		private void FillDb()
		{
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new VerySimple {Id = 1, Name = "Fabio", Weight = 119.5});
				s.Save(new VerySimple {Id = 2, Name = "Fiamma", Weight = 9.8});
				s.Save(new VerySimple {Id = 3, Name = "Roberto", Weight = 98.8 });
				tx.Commit();
			}
		}

		[Test]
		[Description("The batcher should run all UPDATE queries in only one roundtrip.")]
		public void OneRoundTripUpdate()
		{
			FillDb();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var vs1 = s.Get<VerySimple>(1);
				var vs2 = s.Get<VerySimple>(2);
				var vs3 = s.Get<VerySimple>(3);
				vs1.Weight -= 10;
				vs2.Weight -= 1;
				vs3.Weight -= 5;
				Sfi.Statistics.Clear();
				s.Update(vs1);
				s.Update(vs2);
				s.Update(vs3);
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test, Ignore("Not fixed yet."), NetFxOnly]
		[Description("SqlClient: The batcher should run all different INSERT queries in only one roundtrip.")]
		public void SqlClientOneRoundTripForUpdateAndInsert()
		{
#if NETFX
			if (Sfi.Settings.BatcherFactory is SqlClientBatchingBatcherFactory == false)
				Assert.Ignore("This test is for SqlClientBatchingBatcher only");
#endif

			FillDb();

			using (var sqlLog = new SqlLogSpy())
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Save(new VerySimple
				{
					Name = "test441",
					Weight = 894
				});

				s.Save(new AlmostSimple
				{
					Name = "test441",
					Weight = 894
				});

				tx.Commit();

				var log = sqlLog.GetWholeLog();
				//log should only contain NHibernate.SQL once, because that means 
				//that we ony generated a single batch (NHibernate.SQL log will output
				//once per batch)
				Assert.AreEqual(0, log.IndexOf("NHibernate.SQL"), "log should start with NHibernate.SQL");
				Assert.AreEqual(-1, log.IndexOf("NHibernate.SQL", "NHibernate.SQL".Length), "NHibernate.SQL should only appear once in the log");
			}

			Cleanup();
		}

		[Test]
		[Description("SqlClient: The batcher log output should be formatted")]
		public void BatchedoutputShouldBeFormatted()
		{
#if NETFX
			if (Sfi.Settings.BatcherFactory is SqlClientBatchingBatcherFactory == false)
				Assert.Ignore("This test is for SqlClientBatchingBatcher only");
#elif NET6_0_OR_GREATER
			if (Sfi.Settings.BatcherFactory is DbBatchBatcherFactory == false)
				Assert.Ignore("This test is for DbBatchBatcherFactory only");
#else
			Assert.Ignore("This test is for NETFX and NET6_0_OR_GREATER only");
#endif

			using (var sqlLog = new SqlLogSpy())
			{
				FillDb();
				var log = sqlLog.GetWholeLog();
				Assert.That(log, Does.Contain("INSERT \n    INTO").IgnoreCase);
			}

			Cleanup();
		}

		[Test]
		[Description("The batcher should run all DELETE queries in only one roundtrip.")]
		public void OneRoundTripDelete()
		{
			FillDb();

			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				var vs1 = s.Get<VerySimple>(1);
				var vs2 = s.Get<VerySimple>(2);
				var vs3 = s.Get<VerySimple>(3);
				Sfi.Statistics.Clear();
				s.Delete(vs1);
				s.Delete(vs2);
				s.Delete(vs3);
				tx.Commit();
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test]
		[Description(@"Activating the SQL and turning off the batcher's log the log stream:
-should not contains adding to batch
-should contain batch command
-the batcher should work.")]
		public void SqlLog()
		{
			using (new LogSpy(typeof(AbstractBatcher), true))
			{
				using (var sl = new SqlLogSpy())
				{
					Sfi.Statistics.Clear();
					FillDb();
					string logs = sl.GetWholeLog();
					Assert.That(logs, Does.Not.Contain("Adding to batch").IgnoreCase);
					Assert.That(logs, Does.Contain("Batch command").IgnoreCase);
					Assert.That(logs, Does.Contain("INSERT").IgnoreCase);
				}
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test]
		[Description(@"Activating the AbstractBatcher's log the log stream:
-should not contains batch info 
-should contain SQL log info only regarding batcher (SQL log should not be duplicated)
-the batcher should work.")]
		public void AbstractBatcherLog()
		{
			using (new LogSpy(typeof(AbstractBatcher)))
			{
				using (var sl = new SqlLogSpy())
				{
					Sfi.Statistics.Clear();
					FillDb();
					string logs = sl.GetWholeLog();
					Assert.That(logs, Does.Contain("batch").IgnoreCase);
					foreach (var loggingEvent in sl.Appender.GetEvents())
					{
						string message = loggingEvent.RenderedMessage;
						if(message.Contains("insert"))
						{
							Assert.That(message, Does.Contain("batch").IgnoreCase);
						}
					}
				}
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test]
		public void SqlLogShouldGetBatchCommandNotification()
		{
			using (new LogSpy(typeof(AbstractBatcher)))
			{
				using (var sl = new SqlLogSpy())
				{
					Sfi.Statistics.Clear();
					FillDb();
					string logs = sl.GetWholeLog();
					Assert.That(logs, Does.Contain("Batch commands:").IgnoreCase);
				}
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test]
		[Description(@"Activating the AbstractBatcher's log the log stream:
-should contain well formatted SQL log info")]
		public void AbstractBatcherLogFormattedSql()
		{
			using (new LogSpy(typeof(AbstractBatcher)))
			{
				using (var sl = new SqlLogSpy())
				{
					Sfi.Statistics.Clear();
					FillDb();
					foreach (var loggingEvent in sl.Appender.GetEvents())
					{
						string message = loggingEvent.RenderedMessage;
						if(message.StartsWith("Adding"))
						{
							// should be the line with the formatted SQL
							var strings = message.Split(System.Environment.NewLine.ToCharArray());
							foreach (var sqlLine in strings)
							{
								if(sqlLine.Contains("p0"))
								{
									Assert.That(sqlLine, Does.Contain("p1"));
									Assert.That(sqlLine, Does.Contain("p2"));
								}
							}
						}
					}
				}
			}

			Assert.That(Sfi.Statistics.PrepareStatementCount, Is.EqualTo(1));
			Cleanup();
		}

		[Test]
		[Description("Inserting exactly BatchSize entities should not throw on commit. See GH-3725.")]
		public void InsertExactlyBatchSizeEntitiesShouldNotThrowOnCommit()
		{
			// This test verifies that DbBatchBatcher handles empty batches correctly.
			// The bug (GH-3725): When inserting exactly BatchSize entities, the batch auto-executes
			// when full (via ExecuteBatchWithTiming), which clears _currentBatch but NOT _batchCommand.
			// On commit, ExecuteBatch() is called, sees _batchCommand is set, and calls DoExecuteBatch
			// on an empty _currentBatch, causing InvalidOperationException.

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				// Insert exactly BatchSize entities - this fills the batch and triggers auto-execution.
				for (int i = 0; i < BatchSize; i++)
				{
					session.Save(new VerySimple { Id = 1000 + i, Name = $"Test{i}", Weight = i * 1.1 });
				}

				// Commit triggers ExecuteBatch() which would fail on empty batch without the fix,
				// depending on the driver. It fails with Microsoft.Data.SqlClient by example, not with
				// System.Data.SqlClient.
				transaction.Commit();
			}

			Cleanup();
		}

		[Test]
		[Description("Inserting a multiple of BatchSize entities should not throw on commit. See GH-3725.")]
		public void InsertMultipleOfBatchSizeEntitiesShouldNotThrowOnCommit()
		{
			// Same issue as above but with multiple full batches
			const int batchSize = 10;
			const int multiplier = 3;

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				for (int i = 0; i < batchSize * multiplier; i++)
				{
					session.Save(new VerySimple { Id = 2000 + i, Name = $"Test{i}", Weight = i * 1.1 });
				}

				transaction.Commit();
			}

			Cleanup();
		}
	}
}
