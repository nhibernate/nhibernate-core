using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Child.
	/// </summary>
	public class Child
	{
		private Parent _parent;
		private int _count;
		private int _x;
		
		public Parent Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public int Count
		{
			get { return _count; }
			set { _count = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public long Id 
		{
			get { return _parent.Id; }
			set { }
		}

	}
}
