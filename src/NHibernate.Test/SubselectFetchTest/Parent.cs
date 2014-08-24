using System;
using System.Collections.Generic;

namespace NHibernate.Test.SubselectFetchTest
{
	public class Parent
	{
		private string name;
		private IList<Child> children = new List<Child>();
		private IList<Child> moreChildren = new List<Child>();

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

		public virtual IList<Child> MoreChildren
		{
			get { return moreChildren; }
			set { moreChildren = value; }
		}
	}
}