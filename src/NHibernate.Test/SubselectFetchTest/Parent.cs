using System;
using System.Collections;

namespace NHibernate.Test.SubselectFetchTest
{
	public class Parent
	{
		private string name;
		private IList children = new ArrayList();
		private IList moreChildren = new ArrayList();

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

		public virtual IList Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual IList MoreChildren
		{
			get { return moreChildren; }
			set { moreChildren = value; }
		}
	}
}
