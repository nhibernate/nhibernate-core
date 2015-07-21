using System.Collections.Generic;
using System;

namespace NHibernate.Test.NHSpecificTest.NH2189
{
	public class Policy
	{
		public Policy()
		{
			Tasks = new HashSet<Task>();
		}

		public virtual Guid Id { get; protected set; }
		public virtual int PolicyNumber { get; set; }
		public virtual ISet<Task> Tasks { get; protected set; }
	}

	public class Task
	{
		public virtual Guid Id { get; protected set; }
		public virtual string TaskName { get; set; }
		public virtual Policy Policy { get; set; }
		public virtual TeamMember TeamMember { get; set; }
	}

	public class TeamMember
	{
		public virtual Guid Id { get; protected set; }
		public virtual string Name { get; set; }
	}
}
