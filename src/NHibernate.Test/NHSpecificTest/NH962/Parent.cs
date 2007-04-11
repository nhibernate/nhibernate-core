using System;

using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH962
{
	public class Parent
	{
		private Guid id;
		private string name;
		private ISet children;

		public virtual Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		public virtual ISet Children
		{
			get { return children; }
			set { children = value; }
		}
	}
}