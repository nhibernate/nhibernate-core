using System;
using System.Collections.Generic;

namespace NHibernate.Test.Cascade
{
	public class JobBatch
	{
		private long id;
		private DateTime batchDate;
		private ISet<Job> jobs = new HashSet<Job>();
		public JobBatch() {}
		public JobBatch(DateTime batchDate)
		{
			this.batchDate = batchDate;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual DateTime BatchDate
		{
			get { return batchDate; }
			set { batchDate = value; }
		}

		public virtual ISet<Job> Jobs
		{
			get { return jobs; }
			set { jobs = value; }
		}

		public virtual Job CreateJob()
		{
			Job job = new Job(this);
			jobs.Add(job);
			return job;
		}
	}
}