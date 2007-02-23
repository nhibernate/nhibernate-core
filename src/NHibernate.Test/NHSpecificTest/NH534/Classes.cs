using System;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH534
{
	public class Base
	{
		private int objectId;
		private int versionCounter;

		public virtual int ObjectId
		{
			get { return objectId; }
			set { objectId = value; }
		}

		public virtual int VersionCounter
		{
			get { return versionCounter; }
			set { versionCounter = value; }
		}
	}

	public class Parent : Base
	{
		private ISet children;

		public virtual ISet Children
		{
			get { return children; }
			set { children = value; }
		}

		public virtual void AddChild(Child child)
		{
			if (children == null)
			{
				children = new HashedSet();
			}
			children.Add(child);
		}
	}

	public class Child : Base
	{
		private Parent owner;

		public Child()
		{
		}

		public Child(Parent parent)
		{
			parent.AddChild(this);
		}

		public virtual Parent Owner
		{
			get { return owner; }
			set { owner = value; }
		}
	}
}