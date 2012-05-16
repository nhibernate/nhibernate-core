using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH3150
{
	public class Worker
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }
		public virtual string Position { get; set; }
	}

	public class WorkerWithExplicitKey
	{
		public virtual int Id { get; set; }

		public virtual string Name { get; set; }
		public virtual string Position { get; set; }
	}

	public class WorkerWithComponent
	{
		public virtual int Id { get; set; }
		public virtual NidComponent Nid { get; set; }

		public class NidComponent
		{
			public virtual string Name { get; set; }
			public virtual string Position { get; set; }
			// No need to implement Equals for what the test does.
		}
	}

	public class Worker2
	{
		public virtual int Id { get; set; }
		public virtual IList<Role> Roles { get; set; }
	}

	public class Role
	{
		public virtual int Id { get; set; }
		public virtual string Description { get; set; }
	}
}
