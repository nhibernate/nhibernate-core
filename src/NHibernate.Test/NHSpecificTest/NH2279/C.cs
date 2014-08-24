using System;
using System.Collections;

namespace NHibernate.Test.NHSpecificTest.NH2279
{
	public class C
	{
		private int? _id;
		private string _name;

		public C()
		{
		}

		public C(string name)
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
	}
}
