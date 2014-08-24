namespace NHibernate.Test.Cascade
{
	public class Job
	{
		private long id;
		private JobBatch batch;
		private string processingInstructions;
		private int status;
		public Job() {}
		public Job(JobBatch batch)
		{
			this.batch = batch;
		}

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual JobBatch Batch
		{
			get { return batch; }
			set { batch = value; }
		}

		public virtual string ProcessingInstructions
		{
			get { return processingInstructions; }
			set { processingInstructions = value; }
		}

		public virtual int Status
		{
			get { return status; }
			set { status = value; }
		}
	}
}