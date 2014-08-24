using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH643
{
	[Serializable]
	public class Parent
	{
		private int _id;
		private IList<Child> _children = new List<Child>();

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual IList<Child> Children
		{
			get { return this._children; }
			set { this._children = value; }
		}

		public virtual void AddChild(Child child)
		{
			child.Parent = this;
			this.Children.Add(child);
		}
	}

	[Serializable]
	public class Child
	{
		private int _id;
		private Parent _parent;

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual Parent Parent
		{
			get { return this._parent; }
			set { this._parent = value; }
		}
	}
}