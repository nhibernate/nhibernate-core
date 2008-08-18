using System;
using System.Collections;
using System.Data;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
			get { return new string[] { "Cascade.Job.hbm.xml", "Cascade.JobBatch.hbm.xml" }; }
		}

		[Test]
		public void RefreshCascade()
		{
			ISession session = OpenSession();
			ITransaction txn = session.BeginTransaction();

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
			session.Close();
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
	}
}