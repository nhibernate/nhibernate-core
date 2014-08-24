using System;

namespace NHibernate.Test.NHSpecificTest.NH2565
{
	public class Task
	{
		public virtual Guid Id { get; set; }
		public virtual string Description { get; set; }
		public virtual TaskActivity Activity { get; set; }
	}

	public class TaskActivity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}