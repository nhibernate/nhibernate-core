using System;
using System.Collections.Generic;

namespace NHibernate.Test.ListIndex
{
	public class A
	{
		private int _id;
		private string _name;
		private IList<B> _items = new List<B>();

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual IList<B> Items
		{
			get { return _items; }
			set { _items = value; }
		}
	}
}