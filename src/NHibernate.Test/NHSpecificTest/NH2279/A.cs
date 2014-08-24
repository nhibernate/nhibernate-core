using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2279
{
	public class A
	{
		private int? _id;
		private string _name;
		private IList<string> _items = new List<string>();
		private IList<B> _bs = new List<B>();

		public A()
		{
		}

		public A(string name)
		{
			Name = name;
		}

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

		public IList<B> Bs
		{
			get { return _bs; }
			set { _bs = value; }
		}
	}
}
