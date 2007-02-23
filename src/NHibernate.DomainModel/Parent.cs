using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Parent.
	/// </summary>
	public class Parent
	{
		private long _id;
		private int _count;
		private Child _child;
		private object _any;
		private int _x;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public Child Child
		{
			get { return _child; }
			set { _child = value; }
		}

		public object Any
		{
			get { return _any; }
			set { _any = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}
	}
}