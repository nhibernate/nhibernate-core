using System.Collections.Generic;

namespace NHibernate.Test.Ondelete
{
	public class Parent
	{
		private string name;
		private IList<Child> children = new List<Child>();

		protected Parent()
		{
		}

		public Parent(string name)
		{
			this.name = name;
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual IList<Child> Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}