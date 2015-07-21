using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2278
{
	public class CustomA
	{
		private int? _id;
		private string _name;
		private ICustomList<string> _items;

		public CustomA() { }

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

		public ICustomList<string> Items
		{
			get { return _items; }
			set { _items = value; }
		}
	}
}