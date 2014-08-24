using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1850
{
	using System;
	using AdoNet;
	using Environment=NHibernate.Cfg.Environment;

	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchSize, "1");
		}

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return dialect.SupportsSqlBatches;
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
				Assert.True(
					wholeLog.Contains("ExecuteNonQuery took")
					);

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
				Assert.True(
					wholeLog.Contains("ExecuteReader took")
					);
				Assert.True(
					wholeLog.Contains("DataReader was closed after")
					);

				tx.Rollback();
			}
		}
	}
}
