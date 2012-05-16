namespace NHibernate.Test.NHSpecificTest.NH3150
{
	public class Worker
	{
		public virtual int? Id { get; set; }

		public virtual string Name { get; set; }
		public virtual string Position { get; set; }
	}

	public class WorkerWithExplicitKey
	{
		public virtual int? Id { get; set; }

		public virtual string Name { get; set; }
		public virtual string Position { get; set; }
	}

	public class WorkerWithComponent
	{
		public virtual int? Id { get; set; }
		public virtual NidComponent Nid { get; set; }

		public class NidComponent
		{
			public virtual string Name { get; set; }
			public virtual string Position { get; set; }
			// No need to implement Equals for what the test does.
		}
	}
}
