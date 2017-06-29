using System.Collections;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3771
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.BatchVersionedData, "true");
			configuration.SetProperty(Environment.FormatSql, "false");
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.BatchSize, "10");
		}

		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.Settings.BatcherFactory is NonBatchingBatcherFactory);
		}

		[Test]
		[Description("Should be two batchs with two sentences each.")]
		public void InsertAndUpdateWithBatch()
		{
			Sfi.Statistics.Clear();

			using (var sqlLog = new SqlLogSpy())
			using (ISession s = Sfi.OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Singer vs1 = new Singer();
				vs1.Id = 1;
				vs1.Name = "Fabrizio De Andre";
				s.Save(vs1);

				Singer vs2 = new Singer();
				vs2.Id = 2;
				vs2.Name = "Vinicio Capossela";
				s.Save(vs2);

				s.Flush();

				vs1.Name = "De Andre, Fabrizio";
				vs2.Name = "Capossela, Vinicio";

				s.Flush();

				string log = sqlLog.GetWholeLog();

				string[] separator = { System.Environment.NewLine };
				string[] lines = log.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

				int batchs = 0;
				int sqls = 0;
				int batchCommands = 0;
				foreach (string line in lines)
				{
					if (line.StartsWith("NHibernate.SQL") && !line.StartsWith("NHibernate.SQL Batch commands:"))
						sqls++;

					if (line.StartsWith("NHibernate.SQL Batch commands:"))
						batchs++;

					if (line.StartsWith("command"))
						batchCommands++;
				}

				Assert.AreEqual(2, batchs);
				Assert.AreEqual(0, sqls);
				Assert.AreEqual(4, batchCommands);
				Assert.AreEqual(2, Sfi.Statistics.PrepareStatementCount);

				tx.Rollback();
			}
		}
	}
}
