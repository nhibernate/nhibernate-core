using System;

namespace NHibernate.Test.NHSpecificTest.GH2454
{
	public class Project
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class Component
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Project Project { get; set; }
	}

	public class Tag
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Component Component1 { get; set; }
		public virtual Component Component2 { get; set; }
	}
}
