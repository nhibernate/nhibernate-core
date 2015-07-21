using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2228
{
	public class Parent
	{
		public Parent()
		{
			Children = new List<Child>();
		}
		public virtual int Id { get; set; }
		public virtual IList<Child> Children { get; set; }
	}
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual Parent Parent { get; set; }
		public virtual string Description { get; set; }
	}
}