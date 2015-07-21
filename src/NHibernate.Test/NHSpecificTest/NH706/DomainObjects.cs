using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH706
{
	public class Parent
	{
		private int _id;
		private string _name;
		private ISet<Child> _children;
		private ISet<DifferentChild> _differentChildren;

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public virtual ISet<Child> Children
		{
			get { return this._children; }
			set { this._children = value; }
		}

		public virtual ISet<DifferentChild> DifferentChildren
		{
			get { return this._differentChildren; }
			set { this._differentChildren = value; }
		}

		public Parent()
		{
			this._children = new HashSet<Child>();
			this._differentChildren = new HashSet<DifferentChild>();
		}
	}

	public class Child
	{
		private int _id;
		private string _name;
		private Parent _parent;
		private RelatedObject _relatedObject;

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public virtual Parent Parent
		{
			get { return this._parent; }
			set { this._parent = value; }
		}

		public virtual RelatedObject RelatedObject
		{
			get { return this._relatedObject; }
			set { this._relatedObject = value; }
		}
	}

	public class DifferentChild
	{
		private int _id;
		private string _name;
		private Parent _parent;
		private Child _child;

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}

		public virtual Parent Parent
		{
			get { return this._parent; }
			set { this._parent = value; }
		}

		public virtual Child Child
		{
			get { return this._child; }
			set { this._child = value; }
		}
	}

	public class RelatedObject
	{
		private int _id;
		private string _name;

		public virtual int ID
		{
			get { return this._id; }
			set { this._id = value; }
		}

		public virtual string Name
		{
			get { return this._name; }
			set { this._name = value; }
		}
	}

	public class ChildComparer : IComparer<Child>
	{
		public int Compare(Child x, Child y)
		{
			RelatedObject relX = x.RelatedObject;
			RelatedObject relY = y.RelatedObject;

			return relX.Name.CompareTo(relY.Name);
		}
	}
}