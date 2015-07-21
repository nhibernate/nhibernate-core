using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.GenericTest.SetGeneric
{
	public class B
	{
		private int? _id;
		private string _name;

		public B() { }

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
	}
}
