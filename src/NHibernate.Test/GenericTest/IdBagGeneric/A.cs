using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.GenericTest.IdBagGeneric
{
	public class A
	{
		private int? _id;
		private string _name;
		private IList<string> _items;

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

		public IList<string> Items
		{
			get { return _items; }
			set { _items = value; }
		}

	}
}
