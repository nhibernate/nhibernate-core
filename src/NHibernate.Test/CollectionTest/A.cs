using System;
using System.Collections;

namespace NHibernate.Test.CollectionTest
{
	public class A
	{
		private int? _id;
		private string _name;
		private IList _items;

		public A() { }

		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public IList Items
		{
			get { return _items; }
			set { _items = value; }
		}

	}
}
