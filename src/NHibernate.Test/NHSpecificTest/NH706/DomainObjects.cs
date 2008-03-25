using System;
using System.Collections;
using Iesi.Collections;

namespace NHibernate.Test.NHSpecificTest.NH706
{
	public class Parent
	{
		private int _id;
		private string _name;
		private ISet _children;
		private ISet _differentChildren;

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

		public virtual ISet Children
		{
			get { return this._children; }
			set { this._children = value; }
		}

		public virtual ISet DifferentChildren
		{
			get { return this._differentChildren; }
			set { this._differentChildren = value; }
		}

		public Parent()
		{
			this._children = new HashedSet();
			this._differentChildren = new HashedSet();
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

	public class ChildComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			RelatedObject relX = ((Child) x).RelatedObject;
			RelatedObject relY = ((Child) y).RelatedObject;

			int result = relX.Name.CompareTo(relY.Name);
			return result;
		}
	}
}