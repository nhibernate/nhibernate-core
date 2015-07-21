using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2279
{
	public class B
	{
		private int? _id;
		private string _name;
		private IList<C> _cs = new List<C>();

		public B()
		{
		}

		public B(string name)
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

		public IList<C> Cs
		{
			get { return _cs; }
			set { _cs = value; }
		}
	}
}
