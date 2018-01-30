#if !NETCOREAPP2_0
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1850
{
	using System;
	using AdoNet;
	using NHibernate.Engine;
	using Environment = NHibernate.Cfg.Environment;

	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchSize, "1");
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			// This test heavily depends on the batcher implementation.
			// The MySql one likely does not issue the expected logs, but is in fact unused
			// (driver does not supply it, it needs to be explicitly configured).
			// The Oracle one logs twice, causing the expected count to not match.
			// The non batching one just does one single log instead of many.
			// (This test has never test anything else than SQL Server because it was previously
			// applied according to dialect.SupportsSqlBatches which is utterly unrelated to
			// the batcher and just tell about "GO" SQL Server Client tools convention. This was
			// causing it to fail with ODBC + SQL Server, since ODBC uses the non batching batcher.)
			return factory.Settings.BatcherFactory is SqlClientBatchingBatcherFactory;
		}

		[Test]
		public void CanGetQueryDurationForDelete()
		{
			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.CreateQuery("delete Customer").ExecuteUpdate();

				var wholeLog = spy.GetWholeLog();
				Assert.That(wholeLog.Contains("ExecuteNonQuery took"), Is.True);

				tx.Rollback();
			}
		}

		[Test]
		public void CanGetQueryDurationForBatch()
		{
			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				for (int i = 0; i < 3; i++)
				{
					var customer = new Customer
					{
						Name = "foo"
					};
					session.Save(customer);
					session.Delete(customer);
				}
				session.Flush();

				var wholeLog = spy.GetWholeLog();
				var lines = wholeLog.Split(new[]{System.Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
				int batches = 0;
				foreach (var line in lines)
				{
					if (line.Contains("ExecuteBatch for 1 statements took "))
						batches += 1;
				}
				Assert.AreEqual(3, batches);

				tx.Rollback();
			}
		}

		[Test]
		public void CanGetQueryDurationForSelect()
		{
			using (LogSpy spy = new LogSpy(typeof(AbstractBatcher)))
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.CreateQuery("from Customer").List();

				var wholeLog = spy.GetWholeLog();
				Assert.That(wholeLog.Contains("ExecuteReader took"), Is.True);
				Assert.That(wholeLog.Contains("DataReader was closed after"), Is.True);

				tx.Rollback();
			}
		}
	}
}
#endif
