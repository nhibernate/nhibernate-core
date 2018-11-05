using System;

namespace NHibernate.Test.NHSpecificTest.NH1316
{
	public class Parent
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}

	public class ParentHistory
	{
		public virtual int HistId { get; set; }
		public virtual DateTime HistWhen { get; set; }

		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
	}
}
