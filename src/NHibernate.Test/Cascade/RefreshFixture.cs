using System;
using System.Collections;
using System.Data;
using NUnit.Framework;

namespace NHibernate.Test.Cascade
{
	[TestFixture]
	public class RefreshFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "Cascade.Job.hbm.xml", "Cascade.JobBatch.hbm.xml" }; }
		}

		[Test]
		public void RefreshCascade()
		{
			using(ISession session = OpenSession())
			{
				using (ITransaction txn = session.BeginTransaction())
				{
					JobBatch batch = new JobBatch(DateTime.Now);
					batch.CreateJob().ProcessingInstructions = "Just do it!";
					batch.CreateJob().ProcessingInstructions = "I know you can do it!";

					// write the stuff to the database; at this stage all job.status values are zero
					session.Persist(batch);
					session.Flush();

					// behind the session's back, let's modify the statuses
					UpdateStatuses(session);

					// Now lets refresh the persistent batch, and see if the refresh cascaded to the jobs collection elements
					session.Refresh(batch);

					foreach (Job job in batch.Jobs)
					{
						Assert.That(job.Status, Is.EqualTo(1), "Jobs not refreshed!");
					}

					txn.Rollback();
				}
			}
		}

		private void UpdateStatuses(ISession session)
		{
			IDbConnection conn = session.Connection;
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = "UPDATE T_JOB SET JOB_STATUS = 1";
			cmd.CommandType = CommandType.Text;
			session.Transaction.Enlist(cmd);
			cmd.ExecuteNonQuery();
		}

		[Test]
		public void RefreshIgnoringTransient()
		{
			// No exception expected
			using (ISession session = OpenSession())
			{
				using (ITransaction txn = session.BeginTransaction())
				{
					var batch = new JobBatch(DateTime.Now);
					session.Refresh(batch);

					txn.Rollback();
				}
			}
		}

		[Test]
		public void RefreshIgnoringTransientInCollection()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction txn = session.BeginTransaction())
				{

					var batch = new JobBatch(DateTime.Now);
					batch.CreateJob().ProcessingInstructions = "Just do it!";
					session.Persist(batch);
					session.Flush();

					batch.CreateJob().ProcessingInstructions = "I know you can do it!";
					session.Refresh(batch);
					Assert.That(batch.Jobs.Count == 1);

					txn.Rollback();
				}
			}
		}

		[Test]
		public void RefreshNotIgnoringTransientByUnsavedValue()
		{
			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

			var batch = new JobBatch { BatchDate = DateTime.Now, Id = 1 };
			try
			{
				session.Refresh(batch);
			}
			catch (UnresolvableObjectException)
			{
				// as expected
				txn.Rollback();
				session.Close();
			}
		}

	}
}